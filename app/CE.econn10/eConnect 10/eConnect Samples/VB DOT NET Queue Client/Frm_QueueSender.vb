'---------------------------------------------------------------------
' 
' Summary: This Sample allows you to load or modify an xml document that can then be sent to a transactional Message Queue. 
'
' Sample: QueueClientForDotNet.
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
Imports System.IO
Imports System.Messaging
Imports System.Xml
Imports System.Text
Imports System.Environment
Imports System.Reflection
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Public Class FrmQueueSender
    Inherits System.Windows.Forms.Form

    Private myMessage As New System.Messaging.Message
    Private myMSMQueue As New MessageQueue
    Private myMSMQs() As MessageQueue
    Private myCriteria As New MessageQueueCriteria
    Private myQueTrans As New MessageQueueTransaction
    Private myFormatter As New ActiveXMessageFormatter
    Private myXmlDoc As New XmlDocument
    Private TextChanged1 As Boolean


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
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents Browser_View As System.Windows.Forms.TabPage
    Friend WithEvents Text_View As System.Windows.Forms.TabPage
    Friend WithEvents AxWebBrowser1 As AxSHDocVw.AxWebBrowser
    Friend WithEvents RTBox As System.Windows.Forms.RichTextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents RB_PrivateQ As System.Windows.Forms.RadioButton
    Friend WithEvents RB_PublicQ As System.Windows.Forms.RadioButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CmboQ As System.Windows.Forms.ComboBox
    Friend WithEvents bttn_Load As System.Windows.Forms.Button
    Friend WithEvents txtDocPath As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents bttn_Send As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txt_Label As System.Windows.Forms.TextBox
    Friend WithEvents bttn_exit As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(FrmQueueSender))
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.Browser_View = New System.Windows.Forms.TabPage
        Me.AxWebBrowser1 = New AxSHDocVw.AxWebBrowser
        Me.Text_View = New System.Windows.Forms.TabPage
        Me.RTBox = New System.Windows.Forms.RichTextBox
        Me.bttn_Send = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.txt_Label = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtDocPath = New System.Windows.Forms.TextBox
        Me.bttn_Load = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.CmboQ = New System.Windows.Forms.ComboBox
        Me.RB_PublicQ = New System.Windows.Forms.RadioButton
        Me.RB_PrivateQ = New System.Windows.Forms.RadioButton
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        Me.bttn_exit = New System.Windows.Forms.Button
        Me.TabControl1.SuspendLayout()
        Me.Browser_View.SuspendLayout()
        CType(Me.AxWebBrowser1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Text_View.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.Browser_View)
        Me.TabControl1.Controls.Add(Me.Text_View)
        Me.TabControl1.Location = New System.Drawing.Point(8, 192)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(620, 400)
        Me.TabControl1.TabIndex = 5
        '
        'Browser_View
        '
        Me.Browser_View.Controls.Add(Me.AxWebBrowser1)
        Me.Browser_View.Location = New System.Drawing.Point(4, 22)
        Me.Browser_View.Name = "Browser_View"
        Me.Browser_View.Size = New System.Drawing.Size(612, 374)
        Me.Browser_View.TabIndex = 0
        Me.Browser_View.Tag = "Browser_View"
        Me.Browser_View.Text = "Browser View"
        '
        'AxWebBrowser1
        '
        Me.AxWebBrowser1.ContainingControl = Me
        Me.AxWebBrowser1.Enabled = True
        Me.AxWebBrowser1.Location = New System.Drawing.Point(0, 0)
        Me.AxWebBrowser1.OcxState = CType(resources.GetObject("AxWebBrowser1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxWebBrowser1.Size = New System.Drawing.Size(610, 380)
        Me.AxWebBrowser1.TabIndex = 0
        '
        'Text_View
        '
        Me.Text_View.Controls.Add(Me.RTBox)
        Me.Text_View.Location = New System.Drawing.Point(4, 22)
        Me.Text_View.Name = "Text_View"
        Me.Text_View.Size = New System.Drawing.Size(612, 374)
        Me.Text_View.TabIndex = 1
        Me.Text_View.Tag = "Text_View"
        Me.Text_View.Text = "Text View"
        '
        'RTBox
        '
        Me.RTBox.Location = New System.Drawing.Point(0, 0)
        Me.RTBox.Name = "RTBox"
        Me.RTBox.Size = New System.Drawing.Size(610, 380)
        Me.RTBox.TabIndex = 0
        Me.RTBox.Text = ""
        '
        'bttn_Send
        '
        Me.bttn_Send.Cursor = System.Windows.Forms.Cursors.Default
        Me.bttn_Send.Location = New System.Drawing.Point(8, 160)
        Me.bttn_Send.Name = "bttn_Send"
        Me.bttn_Send.Size = New System.Drawing.Size(136, 23)
        Me.bttn_Send.TabIndex = 1
        Me.bttn_Send.Text = "Send to Queue"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txt_Label)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.txtDocPath)
        Me.GroupBox1.Controls.Add(Me.bttn_Load)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.CmboQ)
        Me.GroupBox1.Controls.Add(Me.RB_PublicQ)
        Me.GroupBox1.Controls.Add(Me.RB_PrivateQ)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(620, 152)
        Me.GroupBox1.TabIndex = 7
        Me.GroupBox1.TabStop = False
        '
        'txt_Label
        '
        Me.txt_Label.Location = New System.Drawing.Point(152, 88)
        Me.txt_Label.Name = "txt_Label"
        Me.txt_Label.Size = New System.Drawing.Size(448, 20)
        Me.txt_Label.TabIndex = 15
        Me.txt_Label.Text = ""
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(16, 88)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(104, 23)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Message Label:"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(16, 120)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(112, 23)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "XML Document Path"
        '
        'txtDocPath
        '
        Me.txtDocPath.Location = New System.Drawing.Point(152, 120)
        Me.txtDocPath.Name = "txtDocPath"
        Me.txtDocPath.Size = New System.Drawing.Size(448, 20)
        Me.txtDocPath.TabIndex = 12
        Me.txtDocPath.Text = ""
        '
        'bttn_Load
        '
        Me.bttn_Load.Location = New System.Drawing.Point(128, 120)
        Me.bttn_Load.Name = "bttn_Load"
        Me.bttn_Load.Size = New System.Drawing.Size(20, 20)
        Me.bttn_Load.TabIndex = 11
        Me.bttn_Load.Text = "..."
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(16, 56)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(136, 24)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "Local Private Queues:"
        '
        'CmboQ
        '
        Me.CmboQ.Location = New System.Drawing.Point(152, 56)
        Me.CmboQ.Name = "CmboQ"
        Me.CmboQ.Size = New System.Drawing.Size(448, 21)
        Me.CmboQ.TabIndex = 9
        '
        'RB_PublicQ
        '
        Me.RB_PublicQ.Location = New System.Drawing.Point(152, 16)
        Me.RB_PublicQ.Name = "RB_PublicQ"
        Me.RB_PublicQ.Size = New System.Drawing.Size(144, 24)
        Me.RB_PublicQ.TabIndex = 8
        Me.RB_PublicQ.Text = "Public Queues"
        '
        'RB_PrivateQ
        '
        Me.RB_PrivateQ.Checked = True
        Me.RB_PrivateQ.Location = New System.Drawing.Point(24, 16)
        Me.RB_PrivateQ.Name = "RB_PrivateQ"
        Me.RB_PrivateQ.Size = New System.Drawing.Size(112, 24)
        Me.RB_PrivateQ.TabIndex = 7
        Me.RB_PrivateQ.TabStop = True
        Me.RB_PrivateQ.Text = "Private Queues"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.Filter = "XML files (*.xml)|*.xml"
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.Filter = "XML files (*.xml)|*.xml"
        '
        'bttn_exit
        '
        Me.bttn_exit.Location = New System.Drawing.Point(152, 160)
        Me.bttn_exit.Name = "bttn_exit"
        Me.bttn_exit.Size = New System.Drawing.Size(136, 23)
        Me.bttn_exit.TabIndex = 8
        Me.bttn_exit.Text = "Exit"
        '
        'Frm_QueueSender
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(632, 593)
        Me.Controls.Add(Me.bttn_exit)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.bttn_Send)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Name = "Frm_QueueSender"
        Me.Text = "MSMQ Document Sender"
        Me.TabControl1.ResumeLayout(False)
        Me.Browser_View.ResumeLayout(False)
        CType(Me.AxWebBrowser1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Text_View.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Shared Sub Main()
        Application.Run(New FrmQueueSender)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Populate_CmboQ()

    End Sub

    Private Sub GroupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub RB_PublicQ_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RB_PublicQ.CheckedChanged

        Populate_CmboQ()

    End Sub

    Private Sub Populate_CmboQ()

        CmboQ.Items.Clear()

        If RB_PrivateQ.Checked = True Then
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            myMSMQs = MessageQueue.GetPrivateQueuesByMachine(Environment.MachineName)
            For Each myMSMQueue In myMSMQs
                CmboQ.Items.Add(myMSMQueue.Path())
            Next
        Else
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            myCriteria.MachineName = Environment.MachineName
            myMSMQs = MessageQueue.GetPublicQueues(myCriteria)
            For Each myMSMQueue In myMSMQs
                CmboQ.Items.Add(myMSMQueue.Path())
            Next
        End If
        Windows.Forms.Cursor.Current = Cursors.Default

    End Sub


    Private Sub bttn_Load_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bttn_Load.Click
        Dim DialogResult As Windows.Forms.DialogResult

        Try
            DialogResult = OpenFileDialog1.ShowDialog()
            If DialogResult = DialogResult.OK Then
                txtDocPath.Text = OpenFileDialog1.FileName()
                myXmlDoc.Load(Trim(txtDocPath.Text))
                RTBox.Text = myXmlDoc.OuterXml
                AxWebBrowser1.Navigate("file://" & txtDocPath.Text)
                OpenFileDialog1.Dispose()
            End If
        Catch ex As FileNotFoundException
            MsgBox(ex.ToString)
        End Try

    End Sub

    Private Sub RTBox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RTBox.TextChanged
        TextChanged1 = True
    End Sub

    Private Sub RTBox_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles RTBox.Leave
        Dim MyResult As Microsoft.VisualBasic.MsgBoxResult
        Dim MyDialogResult As System.Windows.Forms.DialogResult
        Dim sMyDir As String

        Try
            If TextChanged1 = True Then
                MyResult = MsgBox("Would you like to save changes to this xml document?", MsgBoxStyle.YesNo, "Save Changes")
                If MyResult = MsgBoxResult.No Then
                    myXmlDoc.LoadXml(RTBox.Text)
                    sMyDir = Directory.GetCurrentDirectory
                    If InStr(Len(sMyDir), sMyDir, "\", CompareMethod.Text) = 0 Then
                        sMyDir = sMyDir & "\"
                    End If
                    myXmlDoc.Save(sMyDir & "temp.xml")
                    AxWebBrowser1.Navigate("file://" & sMyDir & "temp.xml")
                    Exit Sub
                ElseIf MyResult = MsgBoxResult.Yes Then
                    MyDialogResult = SaveFileDialog1.ShowDialog()
                    If MyDialogResult = Windows.Forms.DialogResult.OK Then
                        myXmlDoc.LoadXml(RTBox.Text)
                        txtDocPath.Text = SaveFileDialog1.FileName
                        myXmlDoc.Save(txtDocPath.Text)
                        AxWebBrowser1.Navigate("file://" & txtDocPath.Text)
                    End If
                End If
            End If
        Catch ex As FileNotFoundException
            MsgBox(ex.ToString)
        End Try
    End Sub


    Private Sub bttn_Send_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bttn_Send.Click
        Try
            If CmboQ.SelectedItem <> vbNullString Then
                myMSMQueue.Path = CmboQ.SelectedItem
                If txt_Label.Text = vbNullString Then
                    MsgBox("Please enter a label for this message.")
                    Exit Sub
                End If
                myMessage.Label() = Trim(txt_Label.Text)
                myMessage.Formatter = myFormatter
                myMessage.Body = RTBox.Text.ToString
                myFormatter.Write(myMessage, RTBox.Text.ToString)
                myQueTrans.Begin()
                myMSMQueue.Send(myMessage, myQueTrans)
                myQueTrans.Commit()
                myMSMQueue.Close()
                MsgBox("Messeage Successfully sent to " & CmboQ.SelectedItem)
            Else
                MsgBox("Please select a queue.")
            End If
        Catch ex As MessageQueueException
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub bttn_exit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bttn_exit.Click
        myQueTrans = Nothing
        myMessage = Nothing
        myMSMQueue = Nothing
        End
    End Sub

    Private Sub Frm_QueueSender_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize

        If Me.Visible = True Then
            TabControl1.Width = Me.Width - 20
            TabControl1.Height() = Me.Height - 240
            RTBox.Width = TabControl1.Width - 5
            RTBox.Height = TabControl1.Height - 20
            AxWebBrowser1.Width = TabControl1.Width - 5
            AxWebBrowser1.Height = TabControl1.Height - 20
        End If

    End Sub
End Class
