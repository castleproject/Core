using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Castle.Rook.Compiler.AST;
using Castle.Rook.Compiler.Services;
using Castle.Rook.Compiler.Services.Passes;
using Castle.Rook.Compiler.Visitors;

namespace ASTViewer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.RichTextBox sourceCode;
		private System.Windows.Forms.TreeView rawAST;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private UICompilerContainer container = new UICompilerContainer();
		private System.Windows.Forms.TreeView resultingAST;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		
		private UIErrorReport errorReport;

		public Form1()
		{
			InitializeComponent();

			errorReport = container[ typeof(IErrorReport) ] as UIErrorReport;
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.sourceCode = new System.Windows.Forms.RichTextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.rawAST = new System.Windows.Forms.TreeView();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.resultingAST = new System.Windows.Forms.TreeView();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.panel1 = new System.Windows.Forms.Panel();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(880, 574);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.sourceCode);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(872, 548);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Source";
			// 
			// sourceCode
			// 
			this.sourceCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sourceCode.Font = new System.Drawing.Font("Lucida Console", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.sourceCode.Location = new System.Drawing.Point(0, 0);
			this.sourceCode.Name = "sourceCode";
			this.sourceCode.Size = new System.Drawing.Size(872, 548);
			this.sourceCode.TabIndex = 0;
			this.sourceCode.Text = "";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.rawAST);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(872, 548);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Raw AST";
			// 
			// rawAST
			// 
			this.rawAST.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rawAST.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rawAST.ImageIndex = -1;
			this.rawAST.Location = new System.Drawing.Point(0, 0);
			this.rawAST.Name = "rawAST";
			this.rawAST.SelectedImageIndex = -1;
			this.rawAST.Size = new System.Drawing.Size(872, 548);
			this.rawAST.TabIndex = 0;
			this.rawAST.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.rawAST_AfterSelect);
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.resultingAST);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(872, 548);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "After passes";
			// 
			// resultingAST
			// 
			this.resultingAST.Dock = System.Windows.Forms.DockStyle.Fill;
			this.resultingAST.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.resultingAST.ImageIndex = -1;
			this.resultingAST.Location = new System.Drawing.Point(0, 0);
			this.resultingAST.Name = "resultingAST";
			this.resultingAST.SelectedImageIndex = -1;
			this.resultingAST.Size = new System.Drawing.Size(872, 548);
			this.resultingAST.TabIndex = 1;
			this.resultingAST.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.resultingAST_AfterSelect);
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3,
																						this.columnHeader4});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listView1.Location = new System.Drawing.Point(0, 486);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(880, 88);
			this.listView1.TabIndex = 4;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "file";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "position";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "cat";
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Message";
			this.columnHeader4.Width = 400;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.propertyGrid1);
			this.panel1.Controls.Add(this.checkBox1);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(640, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(240, 486);
			this.panel1.TabIndex = 12;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 142);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(240, 344);
			this.propertyGrid1.TabIndex = 15;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// checkBox1
			// 
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Location = new System.Drawing.Point(12, 16);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(112, 24);
			this.checkBox1.TabIndex = 14;
			this.checkBox1.Text = "Declaration Pass";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(28, 88);
			this.button2.Name = "button2";
			this.button2.TabIndex = 13;
			this.button2.Text = "Reset";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(28, 56);
			this.button1.Name = "button1";
			this.button1.TabIndex = 12;
			this.button1.Text = "Compile";
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(880, 574);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.tabControl1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void button1_Click_1(object sender, System.EventArgs e)
		{
			SourceUnit unit = container.ParserService.Parse(sourceCode.Text);

			CreateAst(unit, rawAST.Nodes);

			if (checkBox1.Enabled)
			{
				DeclarationBinding sb = container[ typeof(DeclarationBinding) ] as DeclarationBinding;

				sb.ExecutePass(unit.CompilationUnit);

				CreateAst(unit, resultingAST.Nodes);
			}

			ShowErrors();
		}

		private void ShowErrors()
		{
			listView1.Items.Clear();

			foreach(ErrorEntry entry in errorReport.List)
			{
				ListViewItem item = listView1.Items.Add( entry.Filename );
				item.SubItems.Add( entry.Pos );
				item.SubItems.Add( entry.IsError ? "error" : "warning" );
				item.SubItems.Add( entry.Contents );
			}
		}

		private void CreateAst(SourceUnit unit, TreeNodeCollection nodes)
		{
			nodes.Clear(); new TreeWalker(unit, nodes);
		}

		private void rawAST_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid1.SelectedObject = e.Node.Tag;
		}

		private void resultingAST_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid1.SelectedObject = e.Node.Tag;
		}
	}


	public class TreeWalker : DepthFirstVisitor
	{
		private TreeNodeCollection nodes;
		private Stack nodeStack = new Stack();

		public TreeWalker(SourceUnit unit, TreeNodeCollection nodes)
		{
			this.nodes = nodes;

			VisitNode(unit.CompilationUnit);
		}

		public TreeNode CurrentNode
		{
			get { return nodeStack.Peek() as TreeNode; }
		}

		public override void VisitCompilationUnit(CompilationUnit compilationUnit)
		{
			nodeStack.Push( nodes.Add("CompilationUnit") );
			CurrentNode.Tag = compilationUnit;

			base.VisitCompilationUnit(compilationUnit);

			nodeStack.Pop();
		}

		public override bool VisitSourceUnit(SourceUnit unit)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("SourceUnit") );
			CurrentNode.Tag = unit;

			base.VisitSourceUnit(unit);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitNamespace(NamespaceDeclaration ns)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Namespace " + ns.Name) );
			CurrentNode.Tag = ns;

			CurrentNode.EnsureVisible();

			base.VisitNamespace(ns);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitTypeDefinitionStatement(TypeDefinitionStatement typeDef)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Type " + typeDef.Name + " (" + typeDef.FullName + ")") );
			CurrentNode.Tag = typeDef;

			CurrentNode.EnsureVisible();

			base.VisitTypeDefinitionStatement(typeDef);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitMethodDefinitionStatement(MethodDefinitionStatement methodDef)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Method " + methodDef.Name + "[ret " + methodDef.ReturnType + "]" ));
			CurrentNode.Tag = methodDef;

			CurrentNode.EnsureVisible();

			base.VisitMethodDefinitionStatement(methodDef);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitSingleVariableDeclarationStatement(SingleVariableDeclarationStatement varDecl)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("SingleVariableDeclarationStatement"));
			CurrentNode.Tag = varDecl;

			CurrentNode.EnsureVisible();

			base.VisitSingleVariableDeclarationStatement(varDecl);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitMultipleVariableDeclarationStatement(MultipleVariableDeclarationStatement varDecl)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("MultipleVariableDeclarationStatement"));
			CurrentNode.Tag = varDecl;

			CurrentNode.EnsureVisible();

			base.VisitMultipleVariableDeclarationStatement(varDecl);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitTypeReference(TypeReference reference)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("TypeReference " + reference.Symbol));
			CurrentNode.Tag = reference;

			CurrentNode.EnsureVisible();

			base.VisitTypeReference(reference);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitIdentifier(Identifier identifier)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("Identifier " + identifier.Name + " - " + identifier.Type.ToString()));
			CurrentNode.Tag = identifier;

			CurrentNode.EnsureVisible();

			base.VisitIdentifier(identifier);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitParameterIdentifier(ParameterIdentifier parameterIdentifier)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("ParameterIdentifier " + parameterIdentifier.Name + " - " + parameterIdentifier.Type.ToString()));
			CurrentNode.Tag = parameterIdentifier;

			CurrentNode.EnsureVisible();

			base.VisitParameterIdentifier(parameterIdentifier);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitExpressionStatement(ExpressionStatement statement)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("ExpressionStatement "));
			CurrentNode.Tag = statement;

			CurrentNode.EnsureVisible();

			base.VisitExpressionStatement(statement);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitRequireStatement(RequireStatement statement)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("RequireStatement"));
			CurrentNode.Tag = statement;

			CurrentNode.EnsureVisible();

			base.VisitRequireStatement(statement);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitAssignmentExpression(AssignmentExpression assignExp)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("AssignmentExpression"));
			CurrentNode.Tag = assignExp;

			CurrentNode.EnsureVisible();

			base.VisitAssignmentExpression(assignExp);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitAugAssignmentExpression(AugAssignmentExpression auAssignExp)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("AugAssignmentExpression"));
			CurrentNode.Tag = auAssignExp;

			CurrentNode.EnsureVisible();

			base.VisitAugAssignmentExpression(auAssignExp);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitYieldExpression(YieldExpression yieldExpression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("YieldExpression"));
			CurrentNode.Tag = yieldExpression;

			CurrentNode.EnsureVisible();

			base.VisitYieldExpression(yieldExpression);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("VariableReferenceExpression"));
			CurrentNode.Tag = variableReferenceExpression;

			CurrentNode.EnsureVisible();

			base.VisitVariableReferenceExpression(variableReferenceExpression);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitUnaryExpression(UnaryExpression unaryExpression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("UnaryExpression"));
			CurrentNode.Tag = unaryExpression;

			CurrentNode.EnsureVisible();

			base.VisitUnaryExpression(unaryExpression);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitMethodInvocationExpression(MethodInvocationExpression invocationExpression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("MethodInvocationExpression"));
			CurrentNode.Tag = invocationExpression;

			CurrentNode.EnsureVisible();

			base.VisitMethodInvocationExpression(invocationExpression);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitMemberAccessExpression(MemberAccessExpression accessExpression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("MemberAccessExpression " + accessExpression.Member));
			CurrentNode.Tag = accessExpression;

			CurrentNode.EnsureVisible();

			base.VisitMemberAccessExpression(accessExpression);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitLiteralReferenceExpression(LiteralReferenceExpression expression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("LiteralReferenceExpression " + expression.Content + " " + expression.Type.ToString()));
			CurrentNode.Tag = expression;

			CurrentNode.EnsureVisible();

			base.VisitLiteralReferenceExpression(expression);

			nodeStack.Pop();

			return true;
		}

		public override bool VisitListExpression(ListExpression expression)
		{
			nodeStack.Push( CurrentNode.Nodes.Add("ListExpression"));
			CurrentNode.Tag = expression;

			CurrentNode.EnsureVisible();

			base.VisitListExpression(expression);

			nodeStack.Pop();

			return true;
		}
	}
}
