namespace Castle.MonoRail.Framework.Views.Aspx.Design
{
	partial class ActionArgumentsEditorForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOk = new System.Windows.Forms.Button();
			this.gbRemoveActionArgument = new System.Windows.Forms.GroupBox();
			this.ddlName = new System.Windows.Forms.ComboBox();
			this.cmdRemove = new System.Windows.Forms.Button();
			this.lblRemoveName = new System.Windows.Forms.Label();
			this.gbAddActionArgument = new System.Windows.Forms.GroupBox();
			this.lblExpression = new System.Windows.Forms.Label();
			this.lblTarget = new System.Windows.Forms.Label();
			this.ckIgnoreErrors = new System.Windows.Forms.CheckBox();
			this.cbPropertyOrValue = new System.Windows.Forms.ComboBox();
			this.cbTarget = new System.Windows.Forms.ComboBox();
			this.txtName = new System.Windows.Forms.TextBox();
			this.cmdAdd = new System.Windows.Forms.Button();
			this.lblAddName = new System.Windows.Forms.Label();
			this.pgActionArguments = new System.Windows.Forms.PropertyGrid();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbRemoveActionArgument.SuspendLayout();
			this.gbAddActionArgument.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(339, 297);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(67, 23);
			this.btnOk.TabIndex = 8;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// gbRemoveActionArgument
			// 
			this.gbRemoveActionArgument.Controls.Add(this.ddlName);
			this.gbRemoveActionArgument.Controls.Add(this.cmdRemove);
			this.gbRemoveActionArgument.Controls.Add(this.lblRemoveName);
			this.gbRemoveActionArgument.Location = new System.Drawing.Point(235, 191);
			this.gbRemoveActionArgument.Name = "gbRemoveActionArgument";
			this.gbRemoveActionArgument.Size = new System.Drawing.Size(249, 93);
			this.gbRemoveActionArgument.TabIndex = 7;
			this.gbRemoveActionArgument.TabStop = false;
			this.gbRemoveActionArgument.Text = "Remove Action Argument";
			// 
			// ddlName
			// 
			this.ddlName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlName.FormattingEnabled = true;
			this.ddlName.Location = new System.Drawing.Point(54, 25);
			this.ddlName.Name = "ddlName";
			this.ddlName.Size = new System.Drawing.Size(186, 21);
			this.ddlName.TabIndex = 4;
			// 
			// cmdRemove
			// 
			this.cmdRemove.Location = new System.Drawing.Point(80, 60);
			this.cmdRemove.Name = "cmdRemove";
			this.cmdRemove.Size = new System.Drawing.Size(96, 24);
			this.cmdRemove.TabIndex = 3;
			this.cmdRemove.Text = "Remove";
			this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
			// 
			// lblRemoveName
			// 
			this.lblRemoveName.Location = new System.Drawing.Point(8, 28);
			this.lblRemoveName.Name = "lblRemoveName";
			this.lblRemoveName.Size = new System.Drawing.Size(40, 16);
			this.lblRemoveName.TabIndex = 0;
			this.lblRemoveName.Text = "Name:";
			// 
			// gbAddActionArgument
			// 
			this.gbAddActionArgument.Controls.Add(this.lblExpression);
			this.gbAddActionArgument.Controls.Add(this.lblTarget);
			this.gbAddActionArgument.Controls.Add(this.ckIgnoreErrors);
			this.gbAddActionArgument.Controls.Add(this.cbPropertyOrValue);
			this.gbAddActionArgument.Controls.Add(this.cbTarget);
			this.gbAddActionArgument.Controls.Add(this.txtName);
			this.gbAddActionArgument.Controls.Add(this.cmdAdd);
			this.gbAddActionArgument.Controls.Add(this.lblAddName);
			this.gbAddActionArgument.Location = new System.Drawing.Point(235, 12);
			this.gbAddActionArgument.Name = "gbAddActionArgument";
			this.gbAddActionArgument.Size = new System.Drawing.Size(249, 160);
			this.gbAddActionArgument.TabIndex = 6;
			this.gbAddActionArgument.TabStop = false;
			this.gbAddActionArgument.Text = "Add Action Argument";
			// 
			// lblExpression
			// 
			this.lblExpression.AutoSize = true;
			this.lblExpression.Location = new System.Drawing.Point(126, 55);
			this.lblExpression.Name = "lblExpression";
			this.lblExpression.Size = new System.Drawing.Size(61, 13);
			this.lblExpression.TabIndex = 10;
			this.lblExpression.Text = "Expression:";
			// 
			// lblTarget
			// 
			this.lblTarget.AutoSize = true;
			this.lblTarget.Location = new System.Drawing.Point(8, 55);
			this.lblTarget.Name = "lblTarget";
			this.lblTarget.Size = new System.Drawing.Size(94, 13);
			this.lblTarget.TabIndex = 9;
			this.lblTarget.Text = "Target Reference:";
			// 
			// ckIgnoreErrors
			// 
			this.ckIgnoreErrors.AutoSize = true;
			this.ckIgnoreErrors.Location = new System.Drawing.Point(86, 97);
			this.ckIgnoreErrors.Name = "ckIgnoreErrors";
			this.ckIgnoreErrors.Size = new System.Drawing.Size(86, 17);
			this.ckIgnoreErrors.TabIndex = 8;
			this.ckIgnoreErrors.Text = "Ignore Errors";
			this.ckIgnoreErrors.UseVisualStyleBackColor = true;
			// 
			// cbPropertyOrValue
			// 
			this.cbPropertyOrValue.FormattingEnabled = true;
			this.cbPropertyOrValue.Location = new System.Drawing.Point(129, 71);
			this.cbPropertyOrValue.Name = "cbPropertyOrValue";
			this.cbPropertyOrValue.Size = new System.Drawing.Size(111, 21);
			this.cbPropertyOrValue.TabIndex = 7;
			// 
			// cbTarget
			// 
			this.cbTarget.FormattingEnabled = true;
			this.cbTarget.Location = new System.Drawing.Point(11, 71);
			this.cbTarget.Name = "cbTarget";
			this.cbTarget.Size = new System.Drawing.Size(111, 21);
			this.cbTarget.TabIndex = 6;
			this.cbTarget.SelectedIndexChanged += new System.EventHandler(this.cbTarget_SelectedIndexChanged);
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(54, 24);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(186, 20);
			this.txtName.TabIndex = 4;
			// 
			// cmdAdd
			// 
			this.cmdAdd.Location = new System.Drawing.Point(80, 126);
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.Size = new System.Drawing.Size(96, 24);
			this.cmdAdd.TabIndex = 3;
			this.cmdAdd.Text = "Add";
			this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			// 
			// lblAddName
			// 
			this.lblAddName.Location = new System.Drawing.Point(8, 28);
			this.lblAddName.Name = "lblAddName";
			this.lblAddName.Size = new System.Drawing.Size(40, 16);
			this.lblAddName.TabIndex = 0;
			this.lblAddName.Text = "Name:";
			// 
			// pgActionArguments
			// 
			this.pgActionArguments.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pgActionArguments.Location = new System.Drawing.Point(3, 12);
			this.pgActionArguments.Name = "pgActionArguments";
			this.pgActionArguments.Size = new System.Drawing.Size(224, 287);
			this.pgActionArguments.TabIndex = 5;
			this.pgActionArguments.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.pgActionArguments_SelectedGridItemChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(416, 297);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(67, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// ActionArgumentsEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(490, 325);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.gbRemoveActionArgument);
			this.Controls.Add(this.gbAddActionArgument);
			this.Controls.Add(this.pgActionArguments);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ActionArgumentsEditorForm";
			this.Text = "Action Arguments Editor";
			this.Load += new System.EventHandler(this.ActionArgumentsEditorForm_Load);
			this.gbRemoveActionArgument.ResumeLayout(false);
			this.gbAddActionArgument.ResumeLayout(false);
			this.gbAddActionArgument.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.GroupBox gbRemoveActionArgument;
		private System.Windows.Forms.ComboBox ddlName;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.Label lblRemoveName;
		private System.Windows.Forms.GroupBox gbAddActionArgument;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Button cmdAdd;
		private System.Windows.Forms.Label lblAddName;
		private System.Windows.Forms.PropertyGrid pgActionArguments;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ComboBox cbTarget;
		private System.Windows.Forms.ComboBox cbPropertyOrValue;
		private System.Windows.Forms.CheckBox ckIgnoreErrors;
		private System.Windows.Forms.Label lblTarget;
		private System.Windows.Forms.Label lblExpression;
	}
}