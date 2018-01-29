using System;
using System.Net.Http;
using System.Threading.Tasks;
using Android.Security;
using Java.Security;
using Java.Security.Cert;
using MTP.Droid;
using Plugin.CurrentActivity;
using Xamarin.Android.Net;

namespace MTP.Services
{
    public static class CloudDataStoreExtension
    {
        public static HttpClient Extend()
        {
            var alias = MainApplication.Current.getCertificateAlias();
            if (alias == null)
            {
                return GetClient();
            }
            else
            {
                var ks = KeyStore.GetInstance("AndroidKeyStore");
                ks.Load(null);
                return installCertificate((X509Certificate) ks.GetCertificate(alias));
            }
        }

        private static HttpClient installCertificate(X509Certificate certificate)
        {
            var crt = new System.Security.Cryptography.X509Certificates.X509Certificate(certificate.GetEncoded());
            var androidClientHandler = new AndroidClientHandler();
            androidClientHandler.ClientCertificates.Add(crt);
            return new HttpClient(androidClientHandler) {BaseAddress = new Uri($"{App.BackendUrl}")};
        }

        private static HttpClient GetClient()
        {
            var kcac = new KCAC();
            KeyChain.ChoosePrivateKeyAlias(CrossCurrentActivity.Current.Activity, kcac, null, null, null, null);
            return installCertificate(kcac.GetCertificate());
        }


        private class KCAC : Java.Lang.Object, IKeyChainAliasCallback
        {
            private Task<X509Certificate[]> _task;

            public X509Certificate GetCertificate() => _task.Result[0];

            public void Alias(string alias)
            {
                _task = Task<X509Certificate[]>.Run(() =>
                {
                    var crt = KeyChain.GetCertificateChain(MainApplication.Current, alias);
                    var ks = KeyStore.GetInstance("AndroidKeyStore");
                    ks.Load(null);
                    ks.SetCertificateEntry(alias, crt[0]);
                    MainApplication.Current.SaveCertificateAlias(alias);
                    return crt;
                });
            }
        }
    }
}