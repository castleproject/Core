
namespace Castle.Goodies.Viewer
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Reflection;
	using System.Windows.Forms;
	using System.Data;
	using System.Security.Policy;

	using Netron.GraphLib;

	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.Windsor;

	using Castle.Goodies.Viewer.Shapes;


	public class MainForm : Form
	{
		private Random rnd = new Random();
	
		private System.ComponentModel.Container components = null;

		private Netron.GraphLib.UI.GraphControl graphControl1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Panel panel1;
		private Netron.GraphLib.UI.Stamper stamper1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.Splitter splitter1;

		private String appPath = @"E:\dev\projects\digitalgravity\castle\trunk\Samples\MindDump\bin";


		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			AppDomain currentDomain = AppDomain.CurrentDomain;
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

			AppDomain app = AppDomain.CreateDomain("minddump", new Evidence(currentDomain.Evidence), appPath, null, false);

//			app.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

			object remoteInstance = null;

			try
			{
				remoteInstance = app.CreateInstanceAndUnwrap(
					"Castle.Applications.MindDump", "Castle.Applications.MindDump.MindDumpContainer");
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.StackTrace);
			}

			IKernel kernel = null;

			if (remoteInstance is IWindsorContainer)
			{
				kernel = (remoteInstance as IWindsorContainer).Kernel;
			}
			else
			{
				kernel = (remoteInstance as IKernel);
			}

			graphControl1.AddLibrary("BasicShapes.dll");
			graphControl1.AddLibrary("Castle.Goodies.Viewer.exe");

			stamper1.GraphControl = graphControl1;

			graphControl1.AllowAddShape = false;

			if (kernel == null) return;

			GraphNode[] nodes = kernel.GraphNodes;

			Hashtable node2Shape = new Hashtable();

			foreach(ComponentModel node in nodes)
			{
				ComponentNode shape = (ComponentNode) 
					graphControl1.AddShape("castle.comp.node", new PointF(80,20)); 
				
				shape.Key = node.Name;
				shape.Text = GetPossibleNullName(node.Service);
				shape.Implementation = GetPossibleNullName(node.Implementation);

				shape.Tag = node;
				node2Shape[node] = shape;
				SetShape(shape);
			}

			foreach(ComponentModel node in nodes)
			{
				Shape parentShape = node2Shape[node] as Shape; 

				foreach(ComponentModel dep in node.Adjacencies)
				{
					Shape childShape = node2Shape[dep] as Shape; 
					Connect(parentShape, childShape);
				}
			}
		}

		private string GetPossibleNullName(Type type)
		{
			if (type == null)
			{
				return "<Custom Activator>";
			}
			return type.Name;
		}

		private void SetShape(Shape shape)
		{
			shape.FitSize(false);
			shape.X = rnd.Next(50,this.graphControl1.Width-100);
			shape.Y = rnd.Next(50,this.graphControl1.Height-20);
		}

		private Connection Connect(Shape childShape, Shape parentShape)
		{
			Connection conn = graphControl1.AddEdge(childShape.Connectors[1], parentShape.Connectors[0]);	

//			conn.From.ConnectorLocation = ConnectorLocations.South;
//			conn.To.ConnectorLocation = ConnectorLocations.North;
			conn.LineEnd = ConnectionEnds.RightOpenArrow;
			return conn;
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
			this.graphControl1 = new Netron.GraphLib.UI.GraphControl();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.stamper1 = new Netron.GraphLib.UI.Stamper();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// graphControl1
			// 
			this.graphControl1.AllowAddConnection = true;
			this.graphControl1.AllowAddShape = true;
			this.graphControl1.AllowDeleteShape = true;
			this.graphControl1.AllowDrop = true;
			this.graphControl1.AllowMoveShape = true;
			this.graphControl1.AutomataPulse = 10;
			this.graphControl1.AutoScroll = true;
			this.graphControl1.BackgroundColor = System.Drawing.Color.Gray;
			this.graphControl1.BackgroundImagePath = null;
			this.graphControl1.BackgroundType = Netron.GraphLib.CanvasBackgroundTypes.FlatColor;
			this.graphControl1.DefaultConnectionPath = "Default";
			this.graphControl1.DefaultLineEnd = Netron.GraphLib.ConnectionEnds.NoEnds;
			this.graphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphControl1.DoTrack = false;
			this.graphControl1.EnableContextMenu = false;
			this.graphControl1.EnableLayout = false;
			this.graphControl1.FileName = null;
			this.graphControl1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.graphControl1.GradientBottom = System.Drawing.Color.White;
			this.graphControl1.GradientTop = System.Drawing.Color.LightSteelBlue;
			this.graphControl1.GraphLayoutAlgorithm = Netron.GraphLib.GraphLayoutAlgorithms.SpringEmbedder;
			this.graphControl1.GridSize = 20;
			this.graphControl1.Location = new System.Drawing.Point(0, 0);
			this.graphControl1.Name = "graphControl1";
			this.graphControl1.RestrictToCanvas = false;
			this.graphControl1.ShowGrid = true;
			this.graphControl1.Size = new System.Drawing.Size(424, 445);
			this.graphControl1.Snap = false;
			this.graphControl1.TabIndex = 4;
			this.graphControl1.Text = "graphControl1";
			this.graphControl1.Zoom = 1F;
			this.graphControl1.ShowNodeProperties += new Netron.GraphLib.ShowPropsDelegate(this.graphControl1_ShowNodeProperties);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 471);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(728, 22);
			this.statusBar1.TabIndex = 2;
			this.statusBar1.Text = "statusBar1";
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2});
			this.menuItem1.Text = "File";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Exit";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(432, 471);
			this.tabControl1.TabIndex = 5;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.graphControl1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(424, 445);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Configuration";
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(424, 445);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Facilities";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.propertyGrid1);
			this.panel1.Controls.Add(this.splitter2);
			this.panel1.Controls.Add(this.stamper1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(432, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(296, 471);
			this.panel1.TabIndex = 7;
			// 
			// stamper1
			// 
			this.stamper1.AutoScroll = true;
			this.stamper1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.stamper1.Location = new System.Drawing.Point(0, 259);
			this.stamper1.Name = "stamper1";
			this.stamper1.Size = new System.Drawing.Size(292, 208);
			this.stamper1.TabIndex = 2;
			this.stamper1.Zoom = 0.2F;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(0, 256);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(292, 3);
			this.splitter2.TabIndex = 3;
			this.splitter2.TabStop = false;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(292, 256);
			this.propertyGrid1.TabIndex = 4;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(432, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 471);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(728, 493);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.statusBar1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "Form1";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
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
			Application.Run(new MainForm());
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			// Is full assemblyName?

			String name = args.Name;
			int index = name.IndexOf(',');

			if (index != -1)
			{
				name = name.Substring(0, index);
			}

			name = Path.Combine( appPath, name );

			String nameVar1 = String.Format( "{0}.dll", name );
			String nameVar2 = String.Format( "{0}.exe", name );

			FileInfo info1 = new FileInfo( nameVar1 );
			FileInfo info2 = new FileInfo( nameVar2 );

			if (info1.Exists)
			{
				return Assembly.LoadFile( info1.FullName );
			}
			else if (info2.Exists)
			{
				return Assembly.LoadFile( info2.FullName );
			}

			// Could not find ... 
			return null;
		}

		private void graphControl1_ShowNodeProperties(object sender, Netron.GraphLib.PropertyBag props)
		{
			propertyGrid1.SelectedObject = props;
		}
	}
}
