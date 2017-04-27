//---------------------------------------------------------------------
// 
// Summary: This Sample creates requester document and passes it to the Microsoft.Dynamics.GP.eConnect assembly. 
//			Which in turn will return a stream of data based on the requester document (customer request).
//
// Sample: eConnect_CSharp_ConsoleApplication.
//
//---------------------------------------------------------------------
// This file is part of the Microsoft Dynamics GP eConnect Samples.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// This source code is intended only as a supplement to Microsoft Dynamcis GP
// eConnect version 10.0.0.0 release and/or on-line documentation. See these other
// materials for detailed information regarding Microsoft code samples.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;

namespace RequesterConsoleApplication
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class ClassReq
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				// Create an eConnect document type object
                eConnectType myEConnectType = new eConnectType();

                // Create a RQeConnectOutType schema object
				RQeConnectOutType myReqType = new RQeConnectOutType();
				
                // Create an eConnectOut XML node object
                eConnectOut myeConnectOut = new eConnectOut();

                // Populate the eConnectOut XML node elements
				myeConnectOut.ACTION = 1;
				myeConnectOut.DOCTYPE = "Customer";
				myeConnectOut.OUTPUTTYPE = 2;
				myeConnectOut.INDEX1FROM = "AARONFIT0001";
				myeConnectOut.INDEX1TO = "AARONFIT0001";
				myeConnectOut.FORLIST = 1;
			
				// Add the eConnectOut XML node object to the RQeConnectOutType schema object
                myReqType.eConnectOut = myeConnectOut;

                // Add the RQeConnectOutType schema object to the eConnect document object
				RQeConnectOutType [] myReqOutType = {myReqType};
				myEConnectType.RQeConnectOutType = myReqOutType;

                // Serialize the eConnect document object to a memory stream
				MemoryStream myMemStream = new MemoryStream();
				XmlSerializer mySerializer = new XmlSerializer(myEConnectType.GetType());
				mySerializer.Serialize(myMemStream, myEConnectType);
				myMemStream.Position = 0;

				// Load the serialized eConnect document object into an XML document object
                XmlTextReader xmlreader = new XmlTextReader(myMemStream);
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(xmlreader);

				// Create a connection string to specify the Microsoft Dynamics GP server and database
                // Change the data source and initial catalog to specify your server and database
                string sConnectionString = @"data source=MYSERVER;initial catalog=TWO;integrated security=SSPI;persist security info=False;packet size=4096";
				
                // Create an eConnectMethods object
                eConnectMethods requester = new eConnectMethods();
				
                // Call the eConnect_Requester method of the eConnectMethods object to retrieve specified XML data
                string reqDoc = requester.eConnect_Requester(sConnectionString, EnumTypes.ConnectionStringType.SqlClient, myXmlDocument.OuterXml);

                // Display the result of the eConnect_Requester method call
                Console.Write(reqDoc);
			}
			catch (Exception ex)
			{// Dislay any errors that occur to the console
				Console.Write(ex.ToString());
			}
		}
	}
}
