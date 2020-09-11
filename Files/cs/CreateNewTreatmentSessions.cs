using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using Terrasoft.Core;
using Terrasoft.Web.Common;
using Terrasoft.Core.Entities;
using Terrasoft.Messaging.Common;
using global::Common.Logging;


namespace CertificationClio.Files.cs
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class CreateNewTreatmentSessions : BaseService
	{
		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
		ResponseFormat = WebMessageFormat.Json)]
		public void TreatmentSessions(Guid tpID, string tpFrequency, int tsessionCount)
		{
			//treatment sessions system setting value 8
			int recordsCount = tsessionCount;
			int days = 0;
			if (tpFrequency == "Daily")
			{
				days = 1;
			}
			else if (tpFrequency == "Weekly")
			{
				days = 7;
			}
			else if (tpFrequency == "Monthly")
			{
				days = 30;
			}
			DateTime? date = null;

			//loop to create 8 records
			for (int i = 1; i <= recordsCount; i++)
			{
				date = (i == 1) ? DateTime.Now : Convert.ToDateTime(date).AddDays(days);

				//Get treatment session detail to add records
				var entity = UserConnection.EntitySchemaManager.GetInstanceByName("UsrTreatmentsessions");

				var assignersEntity = entity.CreateEntity(UserConnection);
				assignersEntity.SetDefColumnValues();
				assignersEntity.SetColumnValue("UsrTreatmentProgramsId", tpID);
				assignersEntity.SetColumnValue("UsrTreatmentsessiondate", date);
				assignersEntity.SetColumnValue("UsrOperator", "Dr J");

				//save the records
				assignersEntity.Save();
			}

			//Post a message to client side entity schema detail
			MsgChannelUtilities.PostMessage(UserConnection, "JKR", "RefereshUI");
		}
	}
}

