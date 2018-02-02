using System;
using System.Threading.Tasks;
using Android.Security;
using Java.Security;
using Java.Security.Cert;
using MTP.Droid;
using Plugin.CurrentActivity;

namespace MTP.Services
{
    public static class CloudDataStoreExtension
    {
        public static void Extend(CloudDataStore store, Action action)
        {
            var alias = MainApplication.Current.getCertificateAlias();
            if (alias == null)
            {
                GetCertificate(new HttpCallback(store, action));
            }
            else
            {
                var ks = KeyStore.GetInstance("AndroidKeyStore");
                ks.Load(null);
                var certificate = ks.GetCertificate(alias);
                store.Certificate =
                    new System.Security.Cryptography.X509Certificates.X509Certificate(certificate.GetEncoded());
                action.Invoke();
            }
        }

        private static void GetCertificate(HttpCallback callback)
        {
            KeyChain.ChoosePrivateKeyAlias(CrossCurrentActivity.Current.Activity, new KeyChainAliasCallback(callback),
                null, null, null, null);
        }

        private class HttpCallback
        {
            private readonly CloudDataStore _store;
            private readonly Action _action;

            public HttpCallback(CloudDataStore store, Action action)
            {
                _store = store;
                _action = action;
            }

            public void Run(Certificate certificate)
            {
                if (certificate != null)
                {
                    var crt =
                        new System.Security.Cryptography.X509Certificates.X509Certificate(certificate.GetEncoded());
                    _store.Certificate = crt;
                }

                _action.Invoke();
            }
        }

        private class KeyChainAliasCallback : Java.Lang.Object, IKeyChainAliasCallback
        {
            private readonly HttpCallback _callback;

            public KeyChainAliasCallback(HttpCallback callback)
            {
                _callback = callback;
            }

            public void Alias(string alias)
            {
                _callback.Run(Task.Factory.StartNew(() =>
                {
                    if (alias == null) return null;
                    var crt = KeyChain.GetCertificateChain(MainApplication.Current, alias);
                    var ks = KeyStore.GetInstance("AndroidKeyStore");
                    ks.Load(null);
                    ks.SetCertificateEntry(alias, crt[0]);
                    MainApplication.Current.preference.Edit()
                        .PutString(MainApplication.CERT_NAME, alias)
                        .PutInt(MainApplication.LOGIN_COUNT, 0)
                        .Apply();

                    return crt;
                }).Result[0]);
            }
        }

        public static void Revert(CloudDataStore store)
        {
            var alias = MainApplication.Current.getCertificateAlias();
            if (alias == null) return;
            var ks = KeyStore.GetInstance("AndroidKeyStore");
            ks.Load(null);
            ks.DeleteEntry(alias);
            store.Certificate = null;
            MainApplication.Current.preference.Edit()
                .PutString(MainApplication.CERT_NAME, null)
                .PutInt(MainApplication.LOGIN_COUNT, 0)
                .Apply();
        }
    }
}