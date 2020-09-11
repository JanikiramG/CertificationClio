using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using Terrasoft.Core;
using Terrasoft.Web.Common;
using Terrasoft.Core.Entities;

namespace CertificationClio.Files.cs
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class GetPlannedSessions : BaseService
	{

		[OperationContract]
		[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
		ResponseFormat = WebMessageFormat.Json)]
		public decimal GetTotalPrice(string Code)
		{
			decimal result = 0;
			var esq = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "UsrTreatmentprograms");
			var colId = esq.AddColumn("Id");
			var colCode = esq.AddColumn("UsrCode");
			esq.AddAllSchemaColumns();
			var esqFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "UsrCode", Code);
			esq.Filters.Add(esqFilter);
			var entities = esq.GetEntityCollection(UserConnection);

			//Treatment progarms record count
			if (entities.Count > 0)
			{
				foreach (var item in entities)
				{
					Guid cpId = item.GetTypedColumnValue<Guid>("Id");
					var esqTS = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "UsrTreatmentsessions");
					var perforId = esqTS.AddColumn("Id");
					esqTS.AddAllSchemaColumns();

					//Treatmentprogram filter
					var esqTpId = esqTS.CreateFilterWithParameters(FilterComparisonType.Equal, "UsrTreatmentPrograms.Id", cpId);

					//Planned sessions filter
					var esqTsFilter = esqTS.CreateFilterWithParameters(FilterComparisonType.Equal, "UsrSessionstatus.Id", new Guid("D0E3C3A6-CD29-4110-A828-8E948D3885CB"));

					esqTS.Filters.Add(esqTpId);
					esqTS.Filters.Add(esqTsFilter);
					var tsEntities = esqTS.GetEntityCollection(UserConnection);

					//Treatment sessions record count 
					if (tsEntities.Count > 0)
					{
						foreach (var ts in tsEntities)
						{
							decimal x = Convert.ToDecimal(ts.GetColumnValue("UsrPrice"));
							result = result + x;
						}
					}
				}
				return result;
			}
			return -1;
		}
	}
}
