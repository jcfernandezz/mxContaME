//---------------------------------------------------------------------
// 
// Summary: This Sample allows you to load or modify an xml document that 
//          will be sent to the Microsoft.Dynamics.GP.eConnect assembly. 
//
// Sample: DirectDocSenderDotNet.
//
//---------------------------------------------------------------------
// This file is part of the Microsoft Dynamics GP eConnect Samples.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// This source code is intended only as a supplement to Microsoft Dynamics GP
// eConnect version 10.0.0.0 release and/or on-line documentation. See these other
// materials for detailed information regarding Microsoft code samples.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Configuration;
using Microsoft.Win32;
using System.IO;
using Microsoft.Dynamics.GP.eConnect;

namespace DirectDocSenderDotNet
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FrmDocSender : System.Windows.Forms.Form
	{
		internal System.Windows.Forms.GroupBox GroupBox1;
		internal System.Windows.Forms.Button btnExit;
		internal System.Windows.Forms.Button btnSend;
		internal System.Windows.Forms.TextBox txtPath;
		internal System.Windows.Forms.TextBox txtConnString;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Button btnConn;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Button btnFileOpen;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		internal System.Windows.Forms.TextBox txtResults;
		private System.Windows.Forms.CheckBox checkBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.GroupBox groupbox2;
		internal System.Windows.Forms.TextBox txtXml;
		private System.Windows.Forms.Button btnSave;
		internal System.Windows.Forms.Label Label1;

		public FrmDocSender()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.txtResults = new System.Windows.Forms.TextBox();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnSend = new System.Windows.Forms.Button();
			this.txtPath = new System.Windows.Forms.TextBox();
			this.txtConnString = new System.Windows.Forms.TextBox();
			this.Label3 = new System.Windows.Forms.Label();
			this.btnConn = new System.Windows.Forms.Button();
			this.Label2 = new System.Windows.Forms.Label();
			this.btnFileOpen = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.groupbox2 = new System.Windows.Forms.GroupBox();
			this.txtXml = new System.Windows.Forms.TextBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.GroupBox1.SuspendLayout();
			this.groupbox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// GroupBox1
			// 
			this.GroupBox1.Controls.Add(this.checkBox1);
			this.GroupBox1.Controls.Add(this.txtResults);
			this.GroupBox1.Controls.Add(this.btnExit);
			this.GroupBox1.Controls.Add(this.btnSend);
			this.GroupBox1.Controls.Add(this.txtPath);
			this.GroupBox1.Controls.Add(this.txtConnString);
			this.GroupBox1.Controls.Add(this.Label3);
			this.GroupBox1.Controls.Add(this.btnConn);
			this.GroupBox1.Controls.Add(this.Label2);
			this.GroupBox1.Controls.Add(this.btnFileOpen);
			this.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.GroupBox1.Location = new System.Drawing.Point(0, 0);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(576, 208);
			this.GroupBox1.TabIndex = 31;
			this.GroupBox1.TabStop = false;
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(16, 64);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.TabIndex = 31;
			this.checkBox1.Text = "Clear Results";
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// txtResults
			// 
			this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtResults.BackColor = System.Drawing.Color.White;
			this.txtResults.Location = new System.Drawing.Point(8, 88);
			this.txtResults.Multiline = true;
			this.txtResults.Name = "txtResults";
			this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtResults.Size = new System.Drawing.Size(560, 110);
			this.txtResults.TabIndex = 30;
			this.txtResults.Text = "";
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(432, 40);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(128, 23);
			this.btnExit.TabIndex = 29;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnSend
			// 
			this.btnSend.Location = new System.Drawing.Point(432, 16);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(128, 23);
			this.btnSend.TabIndex = 28;
			this.btnSend.Text = "Send Xml Document";
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// txtPath
			// 
			this.txtPath.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.txtPath.Location = new System.Drawing.Point(104, 40);
			this.txtPath.Name = "txtPath";
			this.txtPath.Size = new System.Drawing.Size(280, 20);
			this.txtPath.TabIndex = 27;
			this.txtPath.Text = "";
			// 
			// txtConnString
			// 
			this.txtConnString.Location = new System.Drawing.Point(104, 16);
			this.txtConnString.Name = "txtConnString";
			this.txtConnString.Size = new System.Drawing.Size(280, 20);
			this.txtConnString.TabIndex = 26;
			this.txtConnString.Text = "";
			// 
			// Label3
			// 
			this.Label3.Location = new System.Drawing.Point(8, 16);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(100, 16);
			this.Label3.TabIndex = 25;
			this.Label3.Text = "Connection String:";
			// 
			// btnConn
			// 
			this.btnConn.Location = new System.Drawing.Point(392, 16);
			this.btnConn.Name = "btnConn";
			this.btnConn.Size = new System.Drawing.Size(24, 23);
			this.btnConn.TabIndex = 24;
			this.btnConn.Text = "...";
			this.btnConn.Click += new System.EventHandler(this.btnConn_Click);
			// 
			// Label2
			// 
			this.Label2.Location = new System.Drawing.Point(8, 40);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(88, 16);
			this.Label2.TabIndex = 23;
			this.Label2.Text = "Load XML Doc:";
			// 
			// btnFileOpen
			// 
			this.btnFileOpen.Location = new System.Drawing.Point(392, 40);
			this.btnFileOpen.Name = "btnFileOpen";
			this.btnFileOpen.Size = new System.Drawing.Size(24, 23);
			this.btnFileOpen.TabIndex = 22;
			this.btnFileOpen.Text = "...";
			this.btnFileOpen.Click += new System.EventHandler(this.btnFileOpen_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "xml Files|*.xml";
			// 
			// groupbox2
			// 
			this.groupbox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupbox2.Controls.Add(this.txtXml);
			this.groupbox2.Location = new System.Drawing.Point(0, 240);
			this.groupbox2.Name = "groupbox2";
			this.groupbox2.Size = new System.Drawing.Size(576, 384);
			this.groupbox2.TabIndex = 38;
			this.groupbox2.TabStop = false;
			// 
			// txtXml
			// 
			this.txtXml.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtXml.Location = new System.Drawing.Point(8, 16);
			this.txtXml.Multiline = true;
			this.txtXml.Name = "txtXml";
			this.txtXml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtXml.Size = new System.Drawing.Size(560, 360);
			this.txtXml.TabIndex = 37;
			this.txtXml.Text = "";
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(424, 216);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(128, 23);
			this.btnSave.TabIndex = 40;
			this.btnSave.Text = "Save Xml Document";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
			// 
			// Label1
			// 
			this.Label1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Label1.Location = new System.Drawing.Point(16, 224);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(120, 16);
			this.Label1.TabIndex = 41;
			this.Label1.Text = "Xml document:";
			// 
			// frmDocSender
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(576, 621);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.groupbox2);
			this.Controls.Add(this.GroupBox1);
			this.MinimumSize = new System.Drawing.Size(584, 648);
			this.Name = "frmDocSender";
			this.Text = "Direct Document Sender .Net";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.GroupBox1.ResumeLayout(false);
			this.groupbox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FrmDocSender());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			RegistryKey regkeyLocal = Registry.Users;
			regkeyLocal = regkeyLocal.OpenSubKey(@".DEFAULT\Software\Microsoft\eConnect");
			if (regkeyLocal != null)
			{
				object oConnString = regkeyLocal.GetValue("DirectDocSenderDotNet","data source=.;initial catalog=TWO;integrated security=SSPI;persist security info=False;packet size=4096");
				txtConnString.Text = oConnString.ToString();
			}
			else
			{
				txtConnString.Text = "data source=.;initial catalog=TWO;integrated security=SSPI;persist security info=False;packet size=4096";
			}
		}

		private void btnConn_Click(object sender, System.EventArgs e)
		{
			ADODB._Connection cn;
			MSDASC.DataLinks dl = new MSDASC.DataLinksClass();

			try
			{
				cn = new ADODB.ConnectionClass();
				cn.ConnectionString = this.txtConnString.Text + ";Provider=SQLOLEDB.1";
				object oConnection = cn;
				if ((bool)dl.PromptEdit(ref oConnection))
				{
					this.txtConnString.Text = cn.ConnectionString.Replace("Provider=SQLOLEDB.1;",string.Empty);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void btnFileOpen_Click(object sender, System.EventArgs e)
		{
			XmlDocument myXmlDoc = new XmlDocument();

			try
			{
				openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
				
				myXmlDoc.PreserveWhitespace = true;
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					if (string.IsNullOrEmpty(openFileDialog1.FileName) == false) 
					{
						myXmlDoc.Load(openFileDialog1.FileName);
						txtPath.Text = openFileDialog1.FileName;
						txtXml.Text = myXmlDoc.InnerXml;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void btnSend_Click(object sender, System.EventArgs e)
		{
			eConnectMethods eConnect = new eConnectMethods();
			XmlDocument myXmlDoc = new XmlDocument();
			XmlNode eConnectProcessInfoOutgoing;
            
			try
			{
				checkBox1.Checked = false;
				myXmlDoc.LoadXml(txtXml.Text);
				eConnectProcessInfoOutgoing = myXmlDoc.SelectSingleNode("//Outgoing");
				if ((eConnectProcessInfoOutgoing == null) || (string.IsNullOrEmpty(eConnectProcessInfoOutgoing.InnerText) == true))
				{
					eConnect.eConnect_EntryPoint(txtConnString.Text, EnumTypes.ConnectionStringType.SqlClient, myXmlDoc.OuterXml, EnumTypes.SchemaValidationType.None, string.Empty);
					this.txtResults.Text = "Message successfully entered into the BackOffice.";
				}
				else
				{
					if (eConnectProcessInfoOutgoing.InnerText == "TRUE")
					{
						this.txtResults.Text = eConnect.eConnect_Requester(txtConnString.Text,EnumTypes.ConnectionStringType.SqlClient,myXmlDoc.OuterXml);
					}
				}
			}
			catch (Exception ex)
			{
				this.txtResults.Text = ex.ToString();
			}
			finally
			{
				eConnect.Dispose();
			}
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			try
			{
				RegistryKey regkeyLocal = Registry.Users;
				regkeyLocal = regkeyLocal.OpenSubKey(@".DEFAULT\Software\Microsoft",true);	
				RegistryKey myKey = regkeyLocal.CreateSubKey("eConnect");	
				myKey.SetValue("DirectDocSenderDotNet",this.txtConnString.Text); 
				regkeyLocal.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				this.Dispose();
			}
		}

		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			if (checkBox1.Checked)
			{
				this.txtResults.Text = string.Empty;
			}
		}

		private void btnSave_Click_1(object sender, System.EventArgs e)
		{
			try
			{
				FileStream fs;
				saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
				saveFileDialog1.FilterIndex = 2 ;
				saveFileDialog1.RestoreDirectory = true ;
 
				if(saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					if((fs = (FileStream)saveFileDialog1.OpenFile()) != null)
					{
						StreamWriter sw = new StreamWriter(fs);
						sw.Write(txtXml.Text);
						sw.Close();
						fs.Close();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
	}
}
