using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;

namespace Manager
{
	public class ServiceCertValidator : X509CertificateValidator
	{
		/// <summary>
		/// Implementation of a custom certificate validation on the service side.
		/// Service should consider certificate valid if its issuer is the same as the issuer of the service.
		/// If validation fails, throw an exception with an adequate message.
		/// </summary>
		/// <param name="certificate"> certificate to be validate </param>
		public override void Validate(X509Certificate2 certificate)
		{
			/// This will take service's certificate from storage
			var name = "compService";
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, name);

			if (!certificate.Issuer.Equals(srvCert.Issuer))
			{
				throw new Exception("Certificate is not from the valid issuer.");
			}
			//System.Diagnostics.Debugger.Launch();
			var banManager = new BanManager("banned_cert.xml");
			if (banManager.BannedList.Contains($"{certificate.Subject}; {certificate.Thumbprint}"))
			{
				throw new FaultException("Certificate is banned.");
			}
		}
	}
}
