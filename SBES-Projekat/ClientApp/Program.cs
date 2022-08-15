using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using Manager;

namespace ClientApp
{
	public class Program
	{
		static void Main(string[] args)
		{
			//System.Diagnostics.Debugger.Launch();
			/// Define the expected service certificate. It is required to establish cmmunication using certificates.
			string srvCertCN = "compService";

			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

			/// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
			EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/test"),
									  new X509CertificateEndpointIdentity(srvCert));
            
            using (WCFClient proxy = new WCFClient(binding, address))
			{
				/// 1. Communication test
				var input = 0;
				do
                {
                    Console.WriteLine("1. Send complaint. (Only for complainters.)");
					Console.WriteLine("2. Get all complaints. (Only for supervisor.)");
					Console.WriteLine("3. Get all complaints with swear. (Only for supervisor.)");
					Console.WriteLine("4. Ban complainter. (Only for supervisor.)");
					Console.WriteLine("5. Unban complainter. (Only for supervisor.)");
					Console.WriteLine("Enter option:");
					int.TryParse(Console.ReadLine(), out input);

                    switch (input)
                    {
						case 1:
							{
								Console.WriteLine("Enter message: ");
								proxy.SendComplaint(Console.ReadLine());
								break;
							}
						case 2:
                            {
								Console.WriteLine("Complaints");
								var list = proxy.GetAllComplaint();
								if (list == null) break;
								if (list.Count == 0)
								{
									Console.WriteLine("No complaint.");
									break;
								}
								foreach (var item in list)
								{
									Console.WriteLine($"Complanter: {item.Complainter} Text: {item.Text}");
								}
								break;
							}
						case 3:
							{
								Console.WriteLine("Complaints");
								var list = proxy.GetAllComplaintWithSwear();
								if (list == null) break;
								if(list.Count == 0)
                                {
                                    Console.WriteLine("No complaint.");
									break;
                                }
								foreach (var item in list)
								{
									Console.WriteLine($"Complanter: {item.Complainter} Text: {item.Text}");
								}
								break;
							}
						case 4:
                            {
								Console.WriteLine("Complaints");
								var list1 = proxy.GetAllComplaintWithSwear();
								if (list1 == null) break;
								int i = 1;
								foreach (var item in list1)
								{
									Console.WriteLine($"{i++}.Complanter: {item.Complainter} Text: {item.Text}");
								}
								Console.WriteLine("Enter number: ");
								int num = -1;
								int.TryParse((string)Console.ReadLine(), out num);
								try
								{
									proxy.BanUser(list1[num - 1].Complainter);
								}
								catch (Exception ex) { }
								break;
							}
						case 5:
                            {
								Console.WriteLine("Complaints");
								var list = proxy.GetAllBanedComplainters();
								if (list == null) break;
								int i = 1;
								foreach (var item in list)
								{
									Console.WriteLine($"{i++}.Complanter: {item}");
								}
								Console.WriteLine("Enter number: ");
								int num = -1;
								int.TryParse((string)Console.ReadLine(), out num);
								try
								{
									proxy.UnbanUser(list[num - 1]);
								}
								catch (Exception ex) { }
								break;
							}
					}
				} while(input >=0);
				Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");
				Console.ReadLine();
            }
		}
	}
}
