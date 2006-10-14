// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.VSNetIntegration.CastleWizards.Dialogs.Panels
{
	using System;

	using Castle.VSNetIntegration.CastleWizards.Shared;

	/// <summary>
	/// Summary description for LoggingPanel.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(false)]
	public class LoggingPanel : WizardPanel
	{
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox logingApi;
		private System.ComponentModel.Container components = null;

		public LoggingPanel()
		{
			InitializeComponent();

			logingApi.SelectedIndex = 0;
		}

		public String LogingApi
		{
			get { return logingApi.SelectedItem.ToString(); }
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public override bool WantsToShow(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) == true &&
				context.Properties.Contains("Logging") &&
				((bool) context.Properties["Logging"]) == true;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.logingApi = new System.Windows.Forms.ComboBox();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.logingApi);
			this.groupBox6.Controls.Add(this.label8);
			this.groupBox6.Location = new System.Drawing.Point(-8, 8);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(480, 320);
			this.groupBox6.TabIndex = 46;
			this.groupBox6.TabStop = false;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(40, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(349, 16);
			this.label8.TabIndex = 44;
			this.label8.Text = "Logging type:";
			// 
			// logingApi
			// 
			this.logingApi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.logingApi.Items.AddRange(new object[] {
														   "Log4net",
														   "NLog",
														   "Console",
														   "Null"});
			this.logingApi.Location = new System.Drawing.Point(120, 150);
			this.logingApi.Name = "logingApi";
			this.logingApi.Size = new System.Drawing.Size(240, 21);
			this.logingApi.TabIndex = 49;
			// 
			// LoggingPanel
			// 
			this.Controls.Add(this.groupBox6);
			this.Name = "LoggingPanel";
			this.Size = new System.Drawing.Size(464, 336);
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
