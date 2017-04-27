'---------------------------------------------------------------------
' Summary: This Sample creates a customer.xml document and passes it to 
' the Microsoft.Dynamics.GP.eConnect assembly. 
'
' Sample: eConnect_VB_ConsoleApplication.
'
'---------------------------------------------------------------------
' This file is part of the Microsoft Dynamics GP eConnect Samples.
'
' Copyright (c) Microsoft Corporation. All rights reserved.
'
' This source code is intended only as a supplement to Microsoft Dynamics GP
' eConnect version 10.0.0.0 release and/or on-line documentation. See these other
' materials for detailed information regarding Microsoft code samples.
'
' THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
' KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
' IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
' PURPOSE.
'---------------------------------------------------------------------

Imports System
Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports System.EnterpriseServices
Imports System.Text
Imports Microsoft.Dynamics.GP.eConnect
Imports Microsoft.Dynamics.GP.eConnect.Serialization

Public NotInheritable Class Test
    Private Sub New()
    End Sub

    Public Shared Sub Main()

        Dim sCustomerDocument As String
        Dim XsdSchema As String
        Dim sConnectionString As String

        Using e As New eConnectMethods
            Try
                'Create the customer XML document
                SerializeCustomerObject("Customer.xml")

                'Load the customer XML into an XML document object
                Dim xmldoc As New Xml.XmlDocument
                xmldoc.Load("Customer.xml")

                'Use the XML document to create a string
                sCustomerDocument = xmldoc.OuterXml

                'Create a connection string to the Micrsoft Dynamics GP database
                sConnectionString = "data source=127.0.0.1\GP510;initial catalog=TWO;integrated security=SSPI;persist security info=False;packet size=4096"

                'Default path to the eConnect.xsd file, change if eConnect is installed in a location other than the default.
                Dim sXsdDoc As New Xml.XmlDocument
                sXsdDoc.Load("\Archivos de programa\Archivos comunes\Microsoft Shared\eConnect 10\XML Sample Documents\Incoming XSD Schemas\eConnect.xsd")
                XsdSchema = sXsdDoc.OuterXml

                'Pass in xsdSchema to validate against.
                e.eConnect_EntryPoint(sConnectionString, EnumTypes.ConnectionStringType.SqlClient, sCustomerDocument, EnumTypes.SchemaValidationType.XSD, XsdSchema)

            Catch exc As eConnectException
                Console.Write("An eConnect error occurred")
                Console.Write(exc.ToString)
            Catch ex As System.Exception
                Console.Write(ex.ToString)
            End Try
        End Using
    End Sub

    Public Shared Sub SerializeCustomerObject(ByVal filename As String)

        Dim serializer As New XmlSerializer(GetType(eConnectType))
        Dim eConnect As New eConnectType
        Dim customer As New taUpdateCreateCustomerRcd
        Dim customertype As New RMCustomerMasterType

        Try
            'Populate the taUpdateCreateCustomerRcd XML node with customer data
            With customer
                .CUSTNMBR = "Customer001"
                .CUSTNAME = "Customer 1"
                .ADDRESS1 = "2002 60th St SW"
                .ADRSCODE = "Primary"
                .CITY = "NewCity"
                .ZIPCODE = "52302"
            End With

            'Populate the RMCustomerMasterType schema with the taUpdateCreateCustomerRcd XML node object
            customertype.taUpdateCreateCustomerRcd = customer

            'Populate the eConnect XML document object with the RMCustomerMasterType schema object
            ReDim eConnect.RMCustomerMasterType(0)
            eConnect.RMCustomerMasterType(0) = customertype

            'Create file and XML writer objects to serialize the eConnect XML document 
            Dim fs As New FileStream(filename, FileMode.Create)
            Dim writer As New XmlTextWriter(fs, New UTF8Encoding)

            ' Use the XmlTextWriter to serialize the eConnect XML document object to the file
            serializer.Serialize(writer, eConnect)
            writer.Close()

        Catch ex As System.ApplicationException
            Console.Write("Error occurred while creating customer XML document and file")
            Console.Write(ex.ToString)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class


