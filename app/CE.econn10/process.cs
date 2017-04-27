using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using Microsoft.Dynamics.GP.eConnect.MiscRoutines;

namespace CE.econn10
{
    public class process
    {
        private string connectionString = "";
        private string _pre = "";

        public process(string pre)
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[pre].ToString();
            _pre = pre;
        }

        public void Execute(string xml)
        {
            eConnectMethods eConnectObject = new eConnectMethods();
            
            eConnectObject.eConnect_EntryPoint(connectionString, EnumTypes.ConnectionStringType.SqlClient, xml, EnumTypes.SchemaValidationType.None);
        }

        public string getNum(int metodo)
        {
            GetNextDocNumbers myDocNumbers = new GetNextDocNumbers();
            GetSopNumber mySopNumber = new GetSopNumber();

            //n.Add(mySopNumber.GetNextSopNumber(3, "STDINV", connString));

            // Use each method of the GetNextDocNumber object to retrieve the next document number 
            // for the available Microsoft Dynamics GP document types
            if (metodo == 1)
                return myDocNumbers.GetPMNextVoucherNumber(GetNextDocNumbers.IncrementDecrement.Increment, connectionString);
            else
                if (metodo == 2)
                    return myDocNumbers.GetNextPOPReceiptNumber(GetNextDocNumbers.IncrementDecrement.Increment, connectionString);
                else
                    return null;
        }
    }
}
