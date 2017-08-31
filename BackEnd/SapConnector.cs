/*using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SAP.Middleware.Connector;
using AdminPortal.Common;

namespace AdminPortal.Controllers
{
    public class SapConnector
    {
        public string DEFAULT_NAME = "QNL";

        public string DEFAULT_SYSTEM_NUMBER = "20";
        public string DEFAULT_ABAP_DEBUG = "1";
        RfcRepository rfcRep = null;
        RfcDestination rfcDest = null;
        public IRfcFunction GetFunction(string rfcFunctionName)
        {

            RfcConfigParameters rfc = new RfcConfigParameters();
            rfc.Add(RfcConfigParameters.Name, ConfigSettings.ReadConfigValue("Name", DEFAULT_NAME));
            rfc.Add(RfcConfigParameters.AppServerHost, ConfigSettings.ReadConfigValue("AppServerHost", DEFAULT_APP_SERVICE_HOST));
            rfc.Add(RfcConfigParameters.Client, ConfigSettings.ReadConfigValue("Client", DEFAULT_CLIENT));
            rfc.Add(RfcConfigParameters.User, ConfigSettings.ReadConfigValue("User", DEFAULT_USER));
            rfc.Add(RfcConfigParameters.Password, ConfigSettings.ReadConfigValue("Password", DEFAULT_PASSWORD));
            rfc.Add(RfcConfigParameters.SystemNumber, ConfigSettings.ReadConfigValue("SystemNumber", DEFAULT_SYSTEM_NUMBER));
            rfc.Add(RfcConfigParameters.AbapDebug, ConfigSettings.ReadConfigValue("AbapDebug", DEFAULT_ABAP_DEBUG));


            rfcDest = RfcDestinationManager.GetDestination(rfc);
            rfcRep = rfcDest.Repository;
            IRfcFunction function = rfcRep.CreateFunction(ConfigSettings.ReadConfigValue("RfcName", rfcFunctionName));

            return function;

            //function.SetValue("DYE_PROD_ORD", dyeProductionOrdNumber);
        }

        public List<IRfcTable> ExecuteFunction(IRfcFunction function, string[] tableNames)
        {

            function.Invoke(rfcDest);
            List<IRfcTable> tableList = new List<IRfcTable>();
            foreach (string table in tableNames)
            {
                IRfcTable returnedTable = function[table].GetTable();
                tableList.Add(returnedTable);
            }

            return tableList;

            //foreach (IRfcStructure row in PROD_ORDER)
            //{
            //    header.CustomerReferenceNo = row.GetString("CUST_REF");
            //    header.EndCustomer = row.GetString("END_CUST");
            //    header.MaterialNo = row.GetString("MATERIAL");
            //    header.MaterialDesc = row.GetString("MAT_DESC");
            //    header.Colourreference = row.GetString("COLOUR");
            //    header.DyeProductionOrderNo = dyeProductionOrdNumber;
            //    header.HaveProcessed = false;
            //    header.AddedDate = DateTime.Now;
            //    i = 1;
            //    break;
            //}
        }
    }
}*/