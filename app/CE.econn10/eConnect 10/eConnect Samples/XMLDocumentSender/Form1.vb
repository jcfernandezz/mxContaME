'---------------------------------------------------------------------
' 
' Summary: This Sample allows you send an eConnect XML document and to 
' view the result of the operation. 
'
' Sample: XMLDocumentSender.
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

Imports System.IO
Imports System.Xml
Imports Microsoft.Dynamics.GP.eConnect

'The application allows a user to send XML documents to Microsoft Dynamics GP using the 
'Microsoft.Dynamics.GP.eConnect .NET assembly. The XML can be retrieved from and saved to 
'a file.  It also allows the XML to be edited from the UI.  The result of the operation is 
'displayed by the UI.  The application requires a connection string to be created to the 
'Microsoft Dynamics GP SQL Server.  The application can send XML to perform operations 
'on Microsoft Dynamics GP or to retrieve Xml documents from Microsoft Dynamics GP.
Public Class Form1
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Send_Button As System.Windows.Forms.Button
    Friend WithEvents Save_Button As System.Windows.Forms.Button
    Friend WithEvents Exit_Button As System.Windows.Forms.Button
    Friend WithEvents Connection_Button As System.Windows.Forms.Button
    Friend WithEvents FileSelect_Button As System.Windows.Forms.Button
    Friend WithEvents ReturnData_TextBox As System.Windows.Forms.TextBox
    Friend WithEvents XmlDoc_TextBox As System.Windows.Forms.TextBox
    Friend WithEvents XmlFile_TextBox As System.Windows.Forms.TextBox
    Friend WithEvents Connection_TextBox As System.Windows.Forms.TextBox
    Friend WithEvents View_CheckBox As System.Windows.Forms.CheckBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.FileSelect_Button = New System.Windows.Forms.Button
        Me.Connection_Button = New System.Windows.Forms.Button
        Me.XmlFile_TextBox = New System.Windows.Forms.TextBox
        Me.Connection_TextBox = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.ReturnData_TextBox = New System.Windows.Forms.TextBox
        Me.View_CheckBox = New System.Windows.Forms.CheckBox
        Me.Exit_Button = New System.Windows.Forms.Button
        Me.Send_Button = New System.Windows.Forms.Button
        Me.Save_Button = New System.Windows.Forms.Button
        Me.XmlDoc_TextBox = New System.Windows.Forms.TextBox
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.FileSelect_Button)
        Me.GroupBox1.Controls.Add(Me.Connection_Button)
        Me.GroupBox1.Controls.Add(Me.XmlFile_TextBox)
        Me.GroupBox1.Controls.Add(Me.Connection_TextBox)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 8)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(592, 96)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Required:"
        '
        'FileSelect_Button
        '
        Me.FileSelect_Button.Location = New System.Drawing.Point(552, 56)
        Me.FileSelect_Button.Name = "FileSelect_Button"
        Me.FileSelect_Button.Size = New System.Drawing.Size(24, 23)
        Me.FileSelect_Button.TabIndex = 7
        Me.FileSelect_Button.Text = "..."
        '
        'Connection_Button
        '
        Me.Connection_Button.Location = New System.Drawing.Point(552, 24)
        Me.Connection_Button.Name = "Connection_Button"
        Me.Connection_Button.Size = New System.Drawing.Size(24, 24)
        Me.Connection_Button.TabIndex = 6
        Me.Connection_Button.Text = "..."
        '
        'XmlFile_TextBox
        '
        Me.XmlFile_TextBox.Location = New System.Drawing.Point(120, 56)
        Me.XmlFile_TextBox.Name = "XmlFile_TextBox"
        Me.XmlFile_TextBox.Size = New System.Drawing.Size(432, 20)
        Me.XmlFile_TextBox.TabIndex = 5
        Me.XmlFile_TextBox.Text = ""
        '
        'Connection_TextBox
        '
        Me.Connection_TextBox.Location = New System.Drawing.Point(120, 24)
        Me.Connection_TextBox.Name = "Connection_TextBox"
        Me.Connection_TextBox.Size = New System.Drawing.Size(432, 20)
        Me.Connection_TextBox.TabIndex = 4
        Me.Connection_TextBox.Text = ""
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(16, 56)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(104, 23)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Select XML File:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(16, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(104, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Connection String:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ReturnData_TextBox
        '
        Me.ReturnData_TextBox.ForeColor = System.Drawing.SystemColors.WindowText
        Me.ReturnData_TextBox.Location = New System.Drawing.Point(16, 368)
        Me.ReturnData_TextBox.Multiline = True
        Me.ReturnData_TextBox.Name = "ReturnData_TextBox"
        Me.ReturnData_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.ReturnData_TextBox.Size = New System.Drawing.Size(576, 120)
        Me.ReturnData_TextBox.TabIndex = 10
        Me.ReturnData_TextBox.Text = ""
        '
        'View_CheckBox
        '
        Me.View_CheckBox.Checked = True
        Me.View_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.View_CheckBox.Location = New System.Drawing.Point(24, 144)
        Me.View_CheckBox.Name = "View_CheckBox"
        Me.View_CheckBox.Size = New System.Drawing.Size(120, 16)
        Me.View_CheckBox.TabIndex = 9
        Me.View_CheckBox.Text = "View Return Data"
        '
        'Exit_Button
        '
        Me.Exit_Button.Location = New System.Drawing.Point(267, 528)
        Me.Exit_Button.Name = "Exit_Button"
        Me.Exit_Button.TabIndex = 3
        Me.Exit_Button.Text = "Exit"
        '
        'Send_Button
        '
        Me.Send_Button.Enabled = False
        Me.Send_Button.Location = New System.Drawing.Point(219, 216)
        Me.Send_Button.Name = "Send_Button"
        Me.Send_Button.TabIndex = 2
        Me.Send_Button.Text = "Send XML"
        '
        'Save_Button
        '
        Me.Save_Button.Enabled = False
        Me.Save_Button.Location = New System.Drawing.Point(299, 216)
        Me.Save_Button.Name = "Save_Button"
        Me.Save_Button.TabIndex = 4
        Me.Save_Button.Text = "Save XML"
        '
        'XmlDoc_TextBox
        '
        Me.XmlDoc_TextBox.Location = New System.Drawing.Point(16, 120)
        Me.XmlDoc_TextBox.Multiline = True
        Me.XmlDoc_TextBox.Name = "XmlDoc_TextBox"
        Me.XmlDoc_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.XmlDoc_TextBox.Size = New System.Drawing.Size(576, 192)
        Me.XmlDoc_TextBox.TabIndex = 11
        Me.XmlDoc_TextBox.Text = ""
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Send_Button)
        Me.GroupBox2.Controls.Add(Me.Save_Button)
        Me.GroupBox2.Location = New System.Drawing.Point(8, 104)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(592, 248)
        Me.GroupBox2.TabIndex = 12
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "XML Document:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.View_CheckBox)
        Me.GroupBox3.Location = New System.Drawing.Point(8, 352)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(592, 168)
        Me.GroupBox3.TabIndex = 13
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Return Information:"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(608, 558)
        Me.Controls.Add(Me.XmlDoc_TextBox)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Exit_Button)
        Me.Controls.Add(Me.ReturnData_TextBox)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox3)
        Me.Name = "Form1"
        Me.Text = "eConnect Document Sending Utility"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private ConnectionString As String
    Private xmlDoc As XmlDocument

    '''''''''''''''''
    '' UI Controls ''
    '''''''''''''''''
    'When the application loads, initialize with a basic connection string
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ConnectionString = "Data Source=127.0.0.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=;"
    End Sub

    'Create or modify the connection string
    Private Sub Connection_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Connection_Button.Click
        'Create a Form2 object to manage the creation of a new connection string
        Dim EditConnStr As New Form2

        'Set the Form2 object ConnectionString property using the current connection string
        EditConnStr.ConnectionString = ConnectionString

        'Display the Form2 objects UI to allow the user to create a new connection string
        If EditConnStr.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            'Retrieve the connection string created in the modal dialog window and set this
            'class' ConnectionString variable to use the new value
            ConnectionString = EditConnStr.ConnectionString
            Connection_TextBox.Text = ConnectionString
        End If

        'Dispose of the Form2 object
        EditConnStr.Dispose()
    End Sub

    'Open file dialog to specify an xml file that contains the Xml document to send 
    'to Microsoft Dynamics GP
    Private Sub FileSelect_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileSelect_Button.Click
        'Create the file dialog object
        Dim openFileDialog As New OpenFileDialog

        'specify the types of files to be retrieved
        openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"

        Try
            'When the file dialog closes, use the specified filepath to retrieve the Xml from the file
            If openFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                'Display the filename and filepath in the textbox control
                XmlFile_TextBox.Text = openFileDialog.FileName

                'Enable the button to send the Xml to the server
                Send_Button.Enabled = True
                'Disable the Save button until the user edits the XML in the textbox
                Save_Button.Enabled = False

                'Create an XmlDocument object and set it to preserve the doc structure
                xmlDoc = New XmlDocument
                xmlDoc.PreserveWhitespace = True

                'Load the contents of the specified file into the Xml document object
                xmlDoc.Load(XmlFile_TextBox.Text)

                'Populate the textbox control with the XML
                XmlDoc_TextBox.Text = xmlDoc.OuterXml

                'Determine whether the Xml document contains a request for XML
                'Adjust the label of the View_Checkbox control to reflect the expected 
                'type of return data
                If IsRequesterDoc(xmlDoc) = True Then
                    View_CheckBox.Text = "View Return XML"
                Else
                    View_CheckBox.Text = "View Return Data"
                End If

            End If
        Catch fileEx As FileNotFoundException
            ReturnData_TextBox.Text = "Requested file could not be located"
        Catch ex As ApplicationException
            ReturnData_TextBox.Text = ex.Message
        End Try
    End Sub

    'Uses the contents of the Xml file to perform an operation on Microsoft Dynamics GP or 
    'to request an XML document.
    Private Sub Send_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Send_Button.Click
        Dim eConnResult As Boolean

        'Instantiate an eConnectMethods object
        Dim eConnObject As New eConnectMethods

        Try
            'Clear the textbox that displays the result
            ReturnData_TextBox.Text = ""

            'Load the current contents of the XmlDoc_TextBox control into the xmlDoc object
            xmlDoc.LoadXml(XmlDoc_TextBox.Text)

            'Determine whether or not the Xml is requesting an XML document or is updating 
            'the data in Microsoft Dynamics GP
            If IsRequesterDoc(xmlDoc) = True Then
                'Display the returned XML document in the output textbox
                ReturnData_TextBox.Text = eConnObject.eConnect_Requester(ConnectionString, EnumTypes.ConnectionStringType.SqlClient, xmlDoc.OuterXml)
            Else
                'If the update returned TRUE, it was successfully completed
                eConnResult = eConnObject.eConnect_EntryPoint(ConnectionString, EnumTypes.ConnectionStringType.SqlClient, xmlDoc.OuterXml, EnumTypes.SchemaValidationType.None)
                If eConnResult = True Then
                    'If the method returned TRUE, notify the user of a successful operation.
                    ReturnData_TextBox.Text = "XML document was successfully submitted by eConnect"
                Else
                    'If the method returned FALSE, notify the user
                    'Typically, an exception would also occur which should be trapped by the Catch block
                    ReturnData_TextBox.Text = "XML document could not be submitted"
                End If
            End If
        Catch eConnErr As eConnectException
            ReturnData_TextBox.Text = eConnErr.Message
        Catch ex As ApplicationException
            ReturnData_TextBox.Text = ex.Message
        End Try
    End Sub

    'Save XML data to a specified file.
    Private Sub Save_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save_Button.Click
        Try
            'Create and configure the dialog box
            Dim saveFileDialog As New SaveFileDialog
            saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"

            'Update the document to reflect changes made by the user
            xmlDoc.LoadXml(XmlDoc_TextBox.Text)

            'Have the user specify a file and save the contents of the Xml document to the file.
            If saveFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                xmlDoc.Save(saveFileDialog.FileName)
            End If
        Catch ex As ApplicationException
            ReturnData_TextBox.Text = ex.Message
        End Try
    End Sub

    'On exit, close the window
    Private Sub Exit_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Exit_Button.Click
        ActiveForm.Close()
    End Sub

    'Enable the Save XML button when a change is made to the XML in the XmlDoc_TextBox control.
    'This allows the user to save updated XML to a file.
    Private Sub XmlDoc_TextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XmlDoc_TextBox.TextChanged
        If XmlDoc_TextBox.Text <> xmlDoc.OuterXml Then
            Save_Button.Enabled = True
        End If
    End Sub

    'Update the connection string to reflect edits made in the textbox control
    Private Sub Connection_TextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Connection_TextBox.TextChanged
        ConnectionString = Connection_TextBox.Text
    End Sub

    'The checkbox can be used to specify whether the ReturnData_TextBox control displays the result of 
    'sending the XML to the server.
    Private Sub View_CheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles View_CheckBox.CheckedChanged
        If View_CheckBox.Checked = True Then
            ReturnData_TextBox.Enabled = True
        Else
            ReturnData_TextBox.Enabled = False
        End If
    End Sub

    '''''''''''''''
    '' Functions ''
    '''''''''''''''
    'Specifies whether the XML document is requesting data from eConnect
    'Requests contain an <Outgoing> node with a value of TRUE in the <eConnectProcessInfo> section 
    'of the XML document.
    'Returns TRUE when the document is requesting an XML document from eConnect
    'Returns FALSE in all other circumstances
    Private Shared Function IsRequesterDoc(ByVal doc As XmlDocument) As Boolean
        'Create an XML node and query the document for the <Outgoing> node
        Dim RequesterMessage As XmlNode
        RequesterMessage = doc.SelectSingleNode("//Outgoing")

        'Return False if the <Outgoing> node is not found or its value is not equal to TRUE
        'Return True when the <Outgoing> node exists and it's value is equal to TRUE
        If RequesterMessage Is Nothing Then
            Return False
        Else
            If RequesterMessage.InnerText = "TRUE" Then
                Return True
            Else
                Return False
            End If
        End If
    End Function
End Class
