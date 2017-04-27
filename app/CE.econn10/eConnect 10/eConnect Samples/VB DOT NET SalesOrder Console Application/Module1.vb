'---------------------------------------------------------------------
' 
' Summary:  This Sample creates a SalesOrder.xml document and passes it 
'           to the Microsoft.Dynamics.GP.eConnect assembly. The assembly 
'           will then create the SalesOrder document in the BackOffice DB.
'
'			Note: The connection string will need to be changed to your sql server and db.
'
' Sample: eConnectSalesOrder_VB_ConsoleApplication.
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
Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.Dynamics.GP.eConnect
Imports Microsoft.Dynamics.GP.eConnect.Serialization

Public NotInheritable Class Test
    Private Sub New()

    End Sub

    Public Shared Sub Main()

        'Dim t As New Test
        Dim salesOrderDocument As String
        Dim sConnectionString As String

        Using eConCall As New eConnectMethods
            Try
                'Create a serialized eConnect XML Sales Order document
                SerializeSalesOrderObject("SalesOrder.xml ")
                't.SerializeSalesOrderObject("SalesOrder.xml ")

                'Load the eConnect XML document into an XMLDocument object
                Dim xmldoc As New Xml.XmlDocument
                xmldoc.Load("SalesOrder.xml ")

                'Use the XMLDocument object to create a string representation of the eConnect
                'XML Sales Order document
                salesOrderDocument = xmldoc.OuterXml

                'Create a connection string to the Microsoft Dynamics GP database server
                'Integrated Security is required. Integrated security=SSPI
                sConnectionString = "data source=NTBK-XX\GP510;initial catalog=TWO;integrated security=SSPI;persist security info=False;packet size=4096"

                'Use the eConnect_EntryPoint to create the sales order document in Microsoft Dynamics GP
                eConCall.eConnect_EntryPoint(sConnectionString, EnumTypes.ConnectionStringType.SqlClient, salesOrderDocument, EnumTypes.SchemaValidationType.None)

            Catch exp As eConnectException
                Console.Write(exp.ToString)

            Catch ex As System.Exception
                Console.Write(ex.ToString)
            End Try
        End Using

    End Sub

    Public Shared Sub SerializeSalesOrderObject(ByVal filename As String)

        Try
            'Create an array that holds two taSopLineIvcInsert_ItemsTaSopLineIvcInsert XML node objects
            Dim LineItems(1) As taSopLineIvcInsert_ItemsTaSopLineIvcInsert

            'Create a taSopLineIvcInsert_ItemsTaSopLineIvcInsert XML node object
            Dim salesLine As New taSopLineIvcInsert_ItemsTaSopLineIvcInsert

            'Populate the XML node object
            With salesLine
                .ADDRESS1 = "2345 Main St."
                .CUSTNMBR = "CONTOSOL0001"
                .SOPNUMBE = "INV2001"
                .CITY = "Aurora"
                .SOPTYPE = 3
                .DOCID = "STDINV"
                .QUANTITY = 2
                .ITEMNMBR = "ACCS-CRD-12WH"
                .ITEMDESC = "Phone Cord - 12' White"
                .UNITPRCE = 10.95
                .XTNDPRCE = 21.9
                .LOCNCODE = "WAREHOUSE"
                .DOCDATE = DateString   'Today 
            End With

            'Add the taSopLineIvcInsert_ItemsTaSopLineIvcInsert XML node object to the array
            LineItems(0) = salesLine

            'Create a second taSopLineIvcInsert_ItemsTaSopLineIvcInsert XML node object
            Dim salesLine2 As New taSopLineIvcInsert_ItemsTaSopLineIvcInsert

            'Populate the XML node object
            With salesLine2
                .ADDRESS1 = "2345 Main St."
                .CUSTNMBR = "CONTOSOL0001"
                .SOPNUMBE = "INV2001"
                .CITY = "Aurora"
                .SOPTYPE = 3
                .DOCID = "STDINV"
                .QUANTITY = 2
                .ITEMNMBR = "ACCS-CRD-25BK"
                .ITEMDESC = "Phone Cord - 25' Black "
                .UNITPRCE = 15.95
                .XTNDPRCE = 31.9
                .LOCNCODE = "WAREHOUSE"
                .DOCDATE = DateString   'Today 
            End With

            'Add the taSopLineIvcInsert_ItemsTaSopLineIvcInsert XML node object to the array
            LineItems(1) = salesLine2

            'Create a SOPTransactionType schema object and populate its taSopLineIvcInsert_Items poperty
            'with the array of taSopLineIvcInsert_ItemsTaSopLineIvcInsert XML node objects
            Dim salesOrder As New SOPTransactionType
            ReDim Preserve salesOrder.taSopLineIvcInsert_Items(1)
            salesOrder.taSopLineIvcInsert_Items = LineItems

            'Create a taSopHdrIvcInsert XML node object
            Dim salesHdr As New taSopHdrIvcInsert

            'Populate the properties of the taSopHdrIvcInsert XML node object
            With salesHdr
                .SOPTYPE = 3
                .SOPNUMBE = "INV2001"
                .DOCID = "STDINV"
                .BACHNUMB = "eConnect "
                .TAXSCHID = "USASTCITY-6*"
                .FRTSCHID = "USASTCITY-6*"
                .MSCSCHID = "USASTCITY-6*"
                .LOCNCODE = "WAREHOUSE"
                .DOCDATE = DateString 'Today
                .CUSTNMBR = "CONTOSOL0001"
                .CUSTNAME = "Contoso, Ltd."
                .ShipToName = "WAREHOUSE"
                .ADDRESS1 = "2345 Main St."
                .CNTCPRSN = "Joe Healy"
                .FAXNUMBR = "13125550150"
                .CITY = "Aurora"
                .STATE = "IL"
                .ZIPCODE = "65700"
                .COUNTRY = "USA"
                .SUBTOTAL = 53.8
                .DOCAMNT = 53.8
                .USINGHEADERLEVELTAXES = 0
                .PYMTRMID = "Net 30"
            End With

            'Populate the SOPTransactionType schema object's taSopHdrIvcInsert property with the 
            'taSopHdrIvcInsert XML node object
            salesOrder.taSopHdrIvcInsert = salesHdr

            'Create an eConnect XML document object and populate its SOPTransactionType property with
            'the SOPTransactionType schema object
            Dim eConnect As New eConnectType
            ReDim Preserve eConnect.SOPTransactionType(0)
            eConnect.SOPTransactionType(0) = salesOrder

            'Create a file and stream to use to serialize the XML document to a file
            Dim fs As New FileStream(filename, FileMode.Create)
            Dim writer As New XmlTextWriter(fs, New UTF8Encoding)

            'Serialize the XML document to the file
            Dim serializer As New XmlSerializer(GetType(eConnectType))
            serializer.Serialize(writer, eConnect)
            writer.Close()

        Catch ex As ApplicationException
            Console.Write(ex.ToString)
        End Try

    End Sub

End Class


