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
        public static void Extend(CloudDataStore store)
        {
            var alias = MainApplication.Current.getCertificateAlias();
            if (alias == null)
            {
                GetCertificate(new HttpCallback(store));
            }
            else
            {
                var ks = KeyStore.GetInstance("AndroidKeyStore");
                ks.Load(null);
                var certificate = ks.GetCertificate(alias);
                store.Certificate =
                    new System.Security.Cryptography.X509Certificates.X509Certificate(certificate.GetEncoded());
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

            public HttpCallback(CloudDataStore store)
            {
                _store = store;
            }

            public void Run(Certificate certificate)
            {
                var crt = new System.Security.Cryptography.X509Certificates.X509Certificate(certificate.GetEncoded());
                _store.Certificate = crt;
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
                    var crt = KeyChain.GetCertificateChain(MainApplication.Current, alias);
                    var ks = KeyStore.GetInstance("AndroidKeyStore");
                    ks.Load(null);
                    ks.SetCertificateEntry(alias, crt[0]);
                    MainApplication.Current.SaveCertificateAlias(alias);
                    return crt;
                }).Result[0]);
            }
        }
    }
}