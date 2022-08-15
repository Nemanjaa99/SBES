using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using Common;

namespace ClientApp
{
	public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract, IDisposable
	{
		IWCFContract factory;

		public WCFClient(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            Console.WriteLine(cltCertCN);
						
			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

			if(CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN) == null)
            {
                Console.WriteLine($"Unable to find client certificate with name {cltCertCN}");
            }

			factory = this.CreateChannel();
		}

		public void SendComplaint(string msg)
		{
			try
			{
				string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
				X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
					StoreLocation.LocalMachine, signCertCN);

				byte[] signature = DigitalSignature.Create(msg, HashAlgorithm.SHA1, certificateSign);
				factory.SendComplaint(msg, signature);
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}

		public List<Common.Complaint> GetAllComplaint()
		{
			try
			{
				return factory.GetAllComplaint();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
			return null;
		}

		public void BanUser(string name)
		{
			try
			{
				factory.BanUser(name);
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}

		public void UnbanUser(string name)
		{
			try
			{

				factory.UnbanUser(name);
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}
		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

        public void SendComplaint(string msg, byte[] sign)
        {
            throw new NotImplementedException();
        }

        public List<Complaint> GetAllComplaintWithSwear()
        {
			try
			{
				return factory.GetAllComplaintWithSwear();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
			return null;
		}

        public List<string> GetAllBanedComplainters()
        {
			try
			{
				return factory.GetAllBanedComplainters();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
			return null;
		}
    }
}
