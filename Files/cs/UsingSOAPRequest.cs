using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml;
using Terrasoft.Core.Entities.Events;
using Terrasoft.Core.Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CertificationClio.Files.cs
{
    [EntityEventListener(SchemaName = "UsrTreatmentprograms")]
    public class UsingSOAPRequest: BaseEntityEventListener
	{
       public override void OnSaving(object sender, EntityBeforeEventArgs e)
        {
            //Calling CreateSOAPWebRequest method test1
            HttpWebRequest request = CreateSOAPWebRequest();

            string val = "";
            //Geting response from request  
            using (WebResponse Serviceres = request.GetResponse())  
            {  
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))  
                {  
                    //reading stream  
                    var ServiceResult = rd.ReadToEnd();
                    //var myDetails = JsonConvert.DeserializeObject<MyDetail>(ServiceResult);
                    var myDetails = JObject.Parse(ServiceResult);
                    val = (string)myDetails["rates"]["AED"];
                }  
            }
            base.OnSaving(sender, e);
            Entity entity = (Entity)sender;
            entity.SetColumnValue("UsrNotes1", $"Currency Value {val}");
            entity.Save();
        }

        public HttpWebRequest CreateSOAPWebRequest()
        {
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(@"https://currencyapi.net/api/v1/rates?key=2j29394xu40vStVHgUWyvGKRi0YG0DyomCHu&base=USD");
            Req.Headers.Add(@"SOAP: Action");
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            Req.Method = "POST";
            return Req;
        }
    }
   
}
