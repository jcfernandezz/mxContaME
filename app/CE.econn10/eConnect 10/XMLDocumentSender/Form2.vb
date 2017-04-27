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

Imports System.Text

Public Class Form2
    Inherits System.Windows.Forms.Form

    'Private class variables
    Dim ConnectionObject As OleDb.OleDbConnection
    Dim MyConnectionString As String

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
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox
    Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Button2 As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.TextBox3 = New System.Windows.Forms.TextBox
        Me.TextBox4 = New System.Windows.Forms.TextBox
        Me.CheckBox1 = New System.Windows.Forms.CheckBox
        Me.CheckBox2 = New System.Windows.Forms.CheckBox
        Me.RadioButton2 = New System.Windows.Forms.RadioButton
        Me.RadioButton1 = New System.Windows.Forms.RadioButton
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Button2 = New System.Windows.Forms.Button
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(64, 56)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(248, 20)
        Me.TextBox1.TabIndex = 2
        Me.TextBox1.Text = ""
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(64, 304)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(248, 20)
        Me.TextBox2.TabIndex = 3
        Me.TextBox2.Text = ""
        '
        'Button1
        '
        Me.Button1.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Button1.Location = New System.Drawing.Point(82, 344)
        Me.Button1.Name = "Button1"
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "OK"
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(16, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(296, 16)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Specify the following to connect to SQL Servr data:"
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(32, 32)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(280, 16)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "1. Enter the server name:"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(32, 88)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(280, 16)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "2. Enter information to log on to the server:"
        '
        'Label5
        '
        Me.Label5.Enabled = False
        Me.Label5.Location = New System.Drawing.Point(40, 176)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(64, 23)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "User name:"
        '
        'Label6
        '
        Me.Label6.Enabled = False
        Me.Label6.Location = New System.Drawing.Point(40, 208)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(64, 23)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "Password:"
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(24, 272)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(288, 23)
        Me.Label7.TabIndex = 11
        Me.Label7.Text = "3. Specify the database on the server:"
        '
        'TextBox3
        '
        Me.TextBox3.Enabled = False
        Me.TextBox3.Location = New System.Drawing.Point(104, 176)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(208, 20)
        Me.TextBox3.TabIndex = 12
        Me.TextBox3.Text = ""
        '
        'TextBox4
        '
        Me.TextBox4.Enabled = False
        Me.TextBox4.Location = New System.Drawing.Point(104, 208)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.PasswordChar = Microsoft.VisualBasic.ChrW(42)
        Me.TextBox4.Size = New System.Drawing.Size(208, 20)
        Me.TextBox4.TabIndex = 13
        Me.TextBox4.Text = ""
        '
        'CheckBox1
        '
        Me.CheckBox1.Enabled = False
        Me.CheckBox1.Location = New System.Drawing.Point(40, 240)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.TabIndex = 14
        Me.CheckBox1.Text = "Blank password"
        '
        'CheckBox2
        '
        Me.CheckBox2.Enabled = False
        Me.CheckBox2.Location = New System.Drawing.Point(160, 240)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(152, 24)
        Me.CheckBox2.TabIndex = 15
        Me.CheckBox2.Text = "Allow saving password"
        '
        'RadioButton2
        '
        Me.RadioButton2.Location = New System.Drawing.Point(8, 40)
        Me.RadioButton2.Name = "RadioButton2"
        Me.RadioButton2.Size = New System.Drawing.Size(264, 24)
        Me.RadioButton2.TabIndex = 1
        Me.RadioButton2.Text = "Use a specific user name and password "
        '
        'RadioButton1
        '
        Me.RadioButton1.Checked = True
        Me.RadioButton1.Location = New System.Drawing.Point(8, 8)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(264, 24)
        Me.RadioButton1.TabIndex = 0
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "Use Windows Integrated security"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.RadioButton1)
        Me.Panel1.Controls.Add(Me.RadioButton2)
        Me.Panel1.Location = New System.Drawing.Point(32, 104)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(280, 72)
        Me.Panel1.TabIndex = 16
        '
        'Button2
        '
        Me.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button2.Location = New System.Drawing.Point(171, 344)
        Me.Button2.Name = "Button2"
        Me.Button2.TabIndex = 17
        Me.Button2.Text = "Cancel"
        '
        'Form2
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(328, 374)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.CheckBox2)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.TextBox4)
        Me.Controls.Add(Me.TextBox3)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Form2"
        Me.Text = "Connection String Parameters"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    'Property used to set or retrieve the connection string value
    Public Property ConnectionString() As String
        Get
            'Return the current value of MyConnectionString
            Return MyConnectionString
        End Get

        Set(ByVal Value As String)
            'Add the Provider key to the connection string
            'This allows the string to initialize the OleDbConnection object
            MyConnectionString = "Provider=SQLOLEDB.1;" + Value

            'Use a OleDbConnection object to parse the connection string
            ConnectionObject = New OleDb.OleDbConnection(MyConnectionString)
        End Set
    End Property

    'When the form loads, set the values in the UI's Data Source and Database textboxes
    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TextBox1.Text = ConnectionObject.DataSource
        TextBox2.Text = ConnectionObject.Database
    End Sub

    'When user clicks the OK button, use the information supplied in the UI to build a connection string
    'If the user clicks the Cancel button, the form is closed and the original connection string is not changed
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        'Use a stringbuilder object to construct the connection string
        Dim local As StringBuilder

        Try
            local = New StringBuilder
            'Add the datasource and initial catalog keys to the connection string
            local.Append("Data Source=" + TextBox1.Text + ";")
            local.Append("Initial Catalog=" + TextBox2.Text + ";")

            'If Allow Saving Password is checked Persist Security Info key must be set to True
            'otherwise Persist Security Info should be False
            If CheckBox2.Checked = True Then
                local.Append("Persist Security Info=True;")
            Else
                local.Append("Persist Security Info=False;")
            End If

            'If the user has selected to Integrated Security, set the Integrated Security key to SSPI
            'to complete the connection string
            'If the user has elected to supply a username and password, use the supplied data to 
            'complete the connection string
            If RadioButton1.Checked = True Then

                local.Append("Integrated Security=SSPI;")

            Else

                'Add the User ID key with the specified username to the connection string
                local.Append("User ID=" + TextBox3.Text + ";")

                'Only use the Blank Password setting if Allow Saving Password is checked
                'If Allow Saving Password is not checked, the Password key is not added 
                'to the connection string
                If CheckBox2.Checked = True Then

                    'If Blank Password is checked, add an empty Password key to the connection string
                    'If Blank Password is not checked, add the Password key and the supplied value to 
                    'the connection string
                    If CheckBox1.Checked = True Then
                        local.Append("Password="""";")
                    Else
                        local.Append("Password=" + TextBox4.Text + ";")
                    End If

                End If

            End If

            'Save the connection string to the MyConnectionString property so it can be accessed 
            'by the parent form.
            MyConnectionString = local.ToString()

        Catch err As ApplicationException
            'Display error information to the user
            MsgBox("An error occurred creating the connection string" + vbCrLf + vbCrLf + _
                "Error Description = " + err.Message)
        Finally
            'Always close the form
            ActiveForm.Close()
        End Try
    End Sub

    'If the user elects to supply a specific username and password, enable the accompanying UI controls
    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked = True Then
            TextBox3.Enabled = True
            TextBox4.Enabled = True
            CheckBox1.Enabled = True
            CheckBox2.Enabled = True
        End If
    End Sub

    'If the user elects to use Integrated Security, disable all the controls for specifying a username
    'and password
    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            TextBox3.Enabled = False
            TextBox4.Enabled = False
            CheckBox1.Enabled = False
            CheckBox2.Enabled = False
        End If
    End Sub

    'If the user selects the Blank Password option, disable the Password textbox. If the Blank Password
    'option is unchecked, enable the Password textbox.
    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            TextBox4.Enabled = False
        Else
            TextBox4.Enabled = True
        End If
    End Sub
End Class
