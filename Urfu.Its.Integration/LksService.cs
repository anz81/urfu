using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Urfu.Its.Integration.Models;

namespace Urfu.Its.Integration
{
    public class LksService
    {
        readonly string _restService = ConfigurationManager.AppSettings["LksServiceAddress"];
        readonly string _user = ConfigurationManager.AppSettings["LksServiceUser"];
        readonly string _password = ConfigurationManager.AppSettings["LksServicePassword"];
        readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        private static readonly object GlobalLocker = new object();

        public List<StudentSelectionDto> GetSelection()
        {
            lock (GlobalLocker)// this is very very ugly, I know
            {
                var oldCallBack = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
                try
                {
                    var wc = new CookieWebClient
                    {
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(_user, _password)
                    };
                    using (var data = wc.OpenRead(_restService))
                    using (var streamReader = new StreamReader(data))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        return _jsonSerializer.Deserialize<List<StudentSelectionDto>>(jsonTextReader);
                    }
                }
                finally
                {
                    if (ServicePointManager.ServerCertificateValidationCallback == CertificateValidationCallBack)
                        ServicePointManager.ServerCertificateValidationCallback = oldCallBack;
                }
            }
        }
        internal static bool CertificateValidationCallBack(
         object sender,
         System.Security.Cryptography.X509Certificates.X509Certificate certificate,
         System.Security.Cryptography.X509Certificates.X509Chain chain,
         System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
    }
}