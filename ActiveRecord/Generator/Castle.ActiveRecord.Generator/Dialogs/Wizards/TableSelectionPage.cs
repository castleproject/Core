using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Castle.ActiveRecord.Generator.Components.Database;

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	/// <summary>
	/// Summary description for TableSelectionPage.
	/// </summary>
	public class TableSelectionPage : AbstractControlPage
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ListBox listBox1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TableSelectionPage() : base("Table Selection")
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.SuspendLayout();

			// 
			// textBox1
			// 
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(16, 56);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(584, 32);
			this.textBox1.TabIndex = 2;
			this.textBox1.TabStop = false;
			this.textBox1.Text = "Please select the table that this ActiveRecord class will map:";
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(152, 104);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(304, 212);
			this.listBox1.TabIndex = 3;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// TableSelectionPage
			// 
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.textBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "TableSelectionPage";
			this.Size = new System.Drawing.Size(616, 344);
			this.ResumeLayout(false);

		}
		#endregion

		public override void Activated(IDictionary context)
		{
			DatabaseDefinition db =  context["selecteddb"] as DatabaseDefinition;

			if (db == null)
			{
				throw new ApplicationException("No database definition selected");
			}

			listBox1.Items.Clear();
			listBox1.ValueMember = "Name";

			foreach(TableDefinition table in db.Tables)
			{
				listBox1.Items.Add(table);
			}
		}

		public override void Deactivated(System.Collections.IDictionary context)
		{
			context["selectedtable"] = listBox1.SelectedItem;
		}

		public override bool IsValid()
		{
			return listBox1.SelectedIndex != -1;
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RaiseChange(this, e);
		}

		public override String ErrorMessage
		{
			get
			{
				return "Please select a table";
			}
		}
	}
}
