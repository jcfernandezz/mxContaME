'---------------------------------------------------------------------
' 
' Summary: This Sample allows you to the next document number for a SOP document
' or return the previously retrieved number. 
'
' Sample: NextNum_TestHarness.
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

Imports Microsoft.Dynamics.GP.eConnect.MiscRoutines

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
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label01 As System.Windows.Forms.Label
    Friend WithEvents Label02 As System.Windows.Forms.Label
    Friend WithEvents Option2 As System.Windows.Forms.RadioButton
    Friend WithEvents Option1 As System.Windows.Forms.RadioButton
    Friend WithEvents ConnectionLookup As System.Windows.Forms.Button
    Friend WithEvents Command1 As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Label01 = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.ConnectionLookup = New System.Windows.Forms.Button
        Me.Option1 = New System.Windows.Forms.RadioButton
        Me.Option2 = New System.Windows.Forms.RadioButton
        Me.Label02 = New System.Windows.Forms.Label
        Me.Command1 = New System.Windows.Forms.Button
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label01
        '
        Me.Label01.Location = New System.Drawing.Point(16, 8)
        Me.Label01.Name = "Label01"
        Me.Label01.Size = New System.Drawing.Size(80, 23)
        Me.Label01.TabIndex = 0
        Me.Label01.Text = "SOPNUMBER:"
        Me.Label01.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.ConnectionLookup)
        Me.GroupBox1.Controls.Add(Me.Option1)
        Me.GroupBox1.Controls.Add(Me.Option2)
        Me.GroupBox1.Controls.Add(Me.Label02)
        Me.GroupBox1.Controls.Add(Me.Label01)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 8)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(280, 112)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        '
        'Label11
        '
        Me.Label11.Location = New System.Drawing.Point(88, 72)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(152, 23)
        Me.Label11.TabIndex = 13
        Me.Label11.Text = "BackOffice Connection String"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ConnectionLookup
        '
        Me.ConnectionLookup.Location = New System.Drawing.Point(40, 72)
        Me.ConnectionLookup.Name = "ConnectionLookup"
        Me.ConnectionLookup.Size = New System.Drawing.Size(40, 23)
        Me.ConnectionLookup.TabIndex = 12
        Me.ConnectionLookup.Text = "..."
        '
        'Option1
        '
        Me.Option1.Checked = True
        Me.Option1.Location = New System.Drawing.Point(20, 40)
        Me.Option1.Name = "Option1"
        Me.Option1.Size = New System.Drawing.Size(112, 24)
        Me.Option1.TabIndex = 11
        Me.Option1.TabStop = True
        Me.Option1.Text = "Get next number"
        '
        'Option2
        '
        Me.Option2.Location = New System.Drawing.Point(148, 40)
        Me.Option2.Name = "Option2"
        Me.Option2.Size = New System.Drawing.Size(112, 24)
        Me.Option2.TabIndex = 10
        Me.Option2.Text = "Rollback number"
        '
        'Label02
        '
        Me.Label02.Location = New System.Drawing.Point(104, 8)
        Me.Label02.Name = "Label02"
        Me.Label02.Size = New System.Drawing.Size(168, 23)
        Me.Label02.TabIndex = 5
        '
        'Command1
        '
        Me.Command1.Location = New System.Drawing.Point(105, 128)
        Me.Command1.Name = "Command1"
        Me.Command1.Size = New System.Drawing.Size(82, 24)
        Me.Command1.TabIndex = 6
        Me.Command1.Text = "Run"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(292, 158)
        Me.Controls.Add(Me.Command1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "Form1"
        Me.Text = "Get Next Document Number Sample"
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    'Private class variables
    Private ConnectionString As String
    Private strSopNumber As String

    'When the form loads, set the connection string to the default
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ConnectionString = "Data Source=127.0.0.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=;"
    End Sub

    'When the Run button is clicked, perform the GetNextSopNumber or RollBackSopNumber operation.
    Private Sub Command1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Command1.Click
        'Use GetSopNumber from the eConnect.MiscRoutines assembly
        Dim MyTestComponent As New GetSopNumber

        Try
            If Option1.Checked = True Then
                'If the Get next number option is selected, call the GetSopNumber object's
                'GetNextSopNumber method.
                'Parameters: 3 is for SOPTYPE for Invoice, STDINV is the DOCID, ConnectionString is 
                'the current connection string value. 
                'Updates the UI to show the returned number
                strSopNumber = MyTestComponent.GetNextSopNumber(3, "STDINV", ConnectionString)
                Label02.Text = strSopNumber
            Else
                'If the Rollback number option is selected, call the GetSopNumber object's 
                'RollBackSopNumber method.
                'Parameters: strSopNumber is the currently diplayed SOP number, 3 is for SOPTYPE 
                'for Invoice, STDINV is the DOCID, ConnectionString is the current connection string value. 
                'Updates the UI to show the number was put back
                MyTestComponent.RollBackSopNumber(strSopNumber, 3, "STDINV", ConnectionString)
                Label02.Text = "Put back " + strSopNumber
            End If
        Catch AppErr As ApplicationException
            MsgBox("Error occurred connecting to SQL Server" + vbCrLf + vbCrLf + "Error Description: " + _
                AppErr.Message)
        Catch Err As Exception
            'Display error information to the user
            MsgBox("Error Description = " + Err.Message)
        Finally
            'Dispose of the GetSopNumber object
            MyTestComponent.Dispose()
        End Try

    End Sub

    'Use a modal dialog to manage the construction of a new connection string
    Private Sub ConnectionLookup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConnectionLookup.Click
        'Create a Form2 object to manage the creation of a new connection string
        Dim EditConnStr As New Form2

        'Set the Form2 object ConnectionString property using the current connection string
        EditConnStr.ConnectionString = ConnectionString

        'Display the Form2 objects UI to allow the user to create a new connection string
        If EditConnStr.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            'Retrieve the connection string created in the modal dialog window and set this
            'class' ConnectionString variable to use the new value
            ConnectionString = EditConnStr.ConnectionString
        End If

        'Dispose of the Form2 object
        EditConnStr.Dispose()
    End Sub
End Class
