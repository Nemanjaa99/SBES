using Common;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
	public interface IWCFContract
	{

		[OperationContract]
		[FaultContract(typeof(Exception))]
		void SendComplaint(string msg, byte[] sign);

		[OperationContract]
		[FaultContract(typeof(Exception))]
		List<Complaint> GetAllComplaint();

		[OperationContract]
		[FaultContract(typeof(Exception))]
		List<Complaint> GetAllComplaintWithSwear();

		[OperationContract]
		[FaultContract(typeof(Exception))]
		void BanUser(string name);

		[OperationContract]
		[FaultContract(typeof(Exception))]
		void UnbanUser(string name);
		[OperationContract]
		[FaultContract(typeof(Exception))]
		List<string> GetAllBanedComplainters();

	}
}
