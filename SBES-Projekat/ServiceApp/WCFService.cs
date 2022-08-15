using System;
using System.Collections.Generic;
using Contracts;
using System.Threading;
using System.ServiceModel;
using Common;
using Manager;
using System.IO;
using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace ServiceApp
{
    public class WCFService : IWCFContract
	{
        private static List<Complaint> complaints = new List<Complaint>();
        private static BanManager BanManager = new BanManager("banned_cert.xml");

        [PrincipalPermission(SecurityAction.Demand, Role = "Supervisor")]
        public void BanUser(string name)
        {
            BanManager.Ban(name);
            Logger.Log($"Complainter with name: {name} banned.", System.Diagnostics.EventLogEntryType.Information);
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Supervisor")]
        public List<Complaint> GetAllComplaint()
        {
            return complaints;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Complainter")]
        public void SendComplaint(string msg, byte[] sign)
        {
            string clienName = Formatter.ParseNameFromCert(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            //System.Diagnostics.Debugger.Launch();
            var banManager = new BanManager("banned_cert.xml");
            if (banManager.BannedList.Contains(Thread.CurrentPrincipal.Identity.Name))
            {
                throw new FaultException("You are banned.");
            }

            string clientNameSign = clienName + "_sign";
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);

            /// Verify signature using SHA1 hash algorithm
            if (DigitalSignature.Verify(msg, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("Sign is valid");
                var complaint = new Complaint();
                complaint.Complainter = Thread.CurrentPrincipal.Identity.Name;
                complaint.Text = msg;
                complaints.Add(complaint);
            }
            else
            {
                Console.WriteLine("Sign is invalid");
                throw new FaultException("Certificate sign is invalid!!!");
            }
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Supervisor")]
        public void UnbanUser(string name)
        {
            BanManager.Unban(name);
            Logger.Log($"Complainter with name: {name} unbanned.", System.Diagnostics.EventLogEntryType.Information);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Supervisor")]
        public List<Complaint> GetAllComplaintWithSwear()
        {
            var swearList = new string[0];
            if (File.Exists("swear_list.txt"))
            {
                swearList = File.ReadAllLines("swear_list.txt");
            }
            else
            {
                Console.WriteLine("Please add swear_list.txt!");
            }
            var swearComplaints = new List<Complaint>();
            foreach (var complaint in complaints)
            {
                foreach (var swear in swearList)
                {
                    if (complaint.Text.Contains(swear))
                    {
                        swearComplaints.Add(complaint);
                        break;
                    }
                }
            }
            return swearComplaints;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Supervisor")]
        public List<string> GetAllBanedComplainters()
        {
            return BanManager.BannedList;
        }
    }
}
