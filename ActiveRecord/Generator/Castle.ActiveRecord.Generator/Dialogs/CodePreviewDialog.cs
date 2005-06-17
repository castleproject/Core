namespace Castle.ActiveRecord.Generator.Dialogs
{
	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.IO;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Components.CodeGenerator;
	using Castle.ActiveRecord.Generator.Components.Database;

	/// <summary>
	/// Summary description for CodePreviewDialog.
	/// </summary>
	public class CodePreviewDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.ComboBox language;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ICodeProviderFactory codeproviderFactory;
		private System.Windows.Forms.Button button1;
		private CodeTypeDeclaration typeDecl;


		public CodePreviewDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public CodePreviewDialog(IActiveRecordDescriptor descriptor) : this()
		{
			codeproviderFactory = 
				ServiceRegistry.Instance[ typeof(ICodeProviderFactory) ] as ICodeProviderFactory;
			ICodeDomGenerator codeDomGen = 
				ServiceRegistry.Instance[ typeof(ICodeDomGenerator) ] as ICodeDomGenerator;

			typeDecl = codeDomGen.Generate(descriptor);

			language.ValueMember = "Label";

			foreach(CodeProviderInfo info in codeproviderFactory.GetAvailableProviders())
			{
				language.Items.Add(info);
			}

			language.SelectedIndex = 0;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button1 = new System.Windows.Forms.Button();
			this.language = new System.Windows.Forms.ComboBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.pictureBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Controls.Add(this.button1);
			this.pictureBox1.Controls.Add(this.language);
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pictureBox1.Location = new System.Drawing.Point(0, 344);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(616, 50);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// button1
			// 
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(500, 12);
			this.button1.Name = "button1";
			this.button1.TabIndex = 4;
			this.button1.Text = "Close";
			// 
			// language
			// 
			this.language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.language.Location = new System.Drawing.Point(16, 12);
			this.language.Name = "language";
			this.language.TabIndex = 1;
			this.language.SelectedIndexChanged += new System.EventHandler(this.language_SelectedIndexChanged);
			// 
			// richTextBox1
			// 
			this.richTextBox1.DetectUrls = false;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBox1.ForeColor = System.Drawing.Color.Navy;
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(616, 344);
			this.richTextBox1.TabIndex = 3;
			this.richTextBox1.Text = "";
			// 
			// CodePreviewDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(616, 394);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.pictureBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "CodePreviewDialog";
			this.Text = "Preview";
			this.pictureBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void language_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			CodeProviderInfo info = (CodeProviderInfo) language.SelectedItem;

			CodeDomProvider provider = codeproviderFactory.GetProvider( info );

			StringWriter writer = new StringWriter();
			CodeGeneratorOptions opts = new CodeGeneratorOptions();
			opts.BracingStyle = "C";
			opts.BlankLinesBetweenMembers = true;

			provider.CreateGenerator().GenerateCodeFromType(typeDecl, writer, opts);

			richTextBox1.Text = writer.GetStringBuilder().ToString();
		}
	}
}
