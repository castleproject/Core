namespace Castle.ActiveRecord.Generator.Parts
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using Netron.GraphLib;
	using WeifenLuo.WinFormsUI;

	using Castle.ActiveRecord.Generator.Parts.Shapes;
	using Castle.ActiveRecord.Generator.Dialogs;
	using Castle.ActiveRecord.Generator.Dialogs.Wizards;
	using Castle.ActiveRecord.Generator.Components.Database;
	using Castle.ActiveRecord.Generator.Actions;
	using Castle.ActiveRecord.Generator.Components;


	/// <summary>
	/// Summary description for ActiveRecordGraphView
	/// </summary>
	public class ActiveRecordGraphView : Content, ISubWorkspace
	{
		private Netron.GraphLib.UI.GraphControl graphControl1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Model _model;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem addSubClassMenu;
		private System.Windows.Forms.MenuItem addJoinedSubclassMenu;
		private System.Windows.Forms.MenuItem showCodePreviewMenu;
		private System.Windows.Forms.MenuItem showPropertiesMenu;
		private Hashtable _desc2Shape = new Hashtable();
		private IWorkspace _parentWorkspace;
		private ActiveRecordGraphViewActionSet _actionSet;

		public ActiveRecordGraphView()
		{
			InitializeComponent();

			graphControl1.AddLibrary("Castle.ActiveRecord.Generator.exe");

			EnableWizard();
		}

		public ActiveRecordGraphView(Model model) : this()
		{
			_model = model;

			_model.OnProjectReplaced += new ProjectReplaceDelegate(_model_OnProjectReplaced);
			_model.OnProjectChanged += new ProjectDelegate(OnProjectChange);
		}

		private void ActiveRecordGraphView_Load(object sender, System.EventArgs e)
		{
			_actionSet = new ActiveRecordGraphViewActionSet();
			_actionSet.Init(_model);
			_actionSet.Install(this);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
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
			this.graphControl1 = new Netron.GraphLib.UI.GraphControl();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.addSubClassMenu = new System.Windows.Forms.MenuItem();
			this.addJoinedSubclassMenu = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.showCodePreviewMenu = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.showPropertiesMenu = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// graphControl1
			// 
			this.graphControl1.AllowAddConnection = true;
			this.graphControl1.AllowAddShape = true;
			this.graphControl1.AllowDeleteShape = false;
			this.graphControl1.AllowDrop = true;
			this.graphControl1.AllowMoveShape = true;
			this.graphControl1.AutomataPulse = 10;
			this.graphControl1.AutoScroll = true;
			this.graphControl1.BackgroundColor = System.Drawing.Color.Gray;
			this.graphControl1.BackgroundImagePath = null;
			this.graphControl1.BackgroundType = Netron.GraphLib.CanvasBackgroundTypes.Gradient;
			this.graphControl1.DefaultConnectionPath = "Default";
			this.graphControl1.DefaultLineEnd = Netron.GraphLib.ConnectionEnds.NoEnds;
			this.graphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphControl1.DoTrack = false;
			this.graphControl1.EnableContextMenu = false;
			this.graphControl1.EnableLayout = false;
			this.graphControl1.FileName = null;
			this.graphControl1.GradientBottom = System.Drawing.Color.DarkGray;
			this.graphControl1.GradientTop = System.Drawing.Color.DimGray;
			this.graphControl1.GraphLayoutAlgorithm = Netron.GraphLib.GraphLayoutAlgorithms.SpringEmbedder;
			this.graphControl1.GridSize = 15;
			this.graphControl1.Location = new System.Drawing.Point(2, 2);
			this.graphControl1.Name = "graphControl1";
			this.graphControl1.RestrictToCanvas = true;
			this.graphControl1.ShowGrid = false;
			this.graphControl1.Size = new System.Drawing.Size(844, 390);
			this.graphControl1.Snap = false;
			this.graphControl1.TabIndex = 0;
			this.graphControl1.Text = "graphControl1";
			this.graphControl1.Zoom = 1F;
			this.graphControl1.ShowNodeProperties += new Netron.GraphLib.ShowPropsDelegate(this.graphControl1_ShowNodeProperties);
			this.graphControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.graphControl1_MouseUp);
			this.graphControl1.OnNewConnection += new Netron.GraphLib.NewConnection(this.graphControl1_OnNewConnection);
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.addSubClassMenu,
																						this.addJoinedSubclassMenu,
																						this.menuItem5,
																						this.showCodePreviewMenu,
																						this.menuItem4,
																						this.showPropertiesMenu});
			// 
			// addSubClassMenu
			// 
			this.addSubClassMenu.Index = 0;
			this.addSubClassMenu.Text = "Add SubClass";
			this.addSubClassMenu.Click += new System.EventHandler(this.addSubClassMenu_Click);
			// 
			// addJoinedSubclassMenu
			// 
			this.addJoinedSubclassMenu.Index = 1;
			this.addJoinedSubclassMenu.Text = "Add Joined SubClass";
			this.addJoinedSubclassMenu.Click += new System.EventHandler(this.addJoinedSubclassMenu_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.Text = "-";
			// 
			// showCodePreviewMenu
			// 
			this.showCodePreviewMenu.Index = 3;
			this.showCodePreviewMenu.Text = "Preview code";
			this.showCodePreviewMenu.Click += new System.EventHandler(this.showCodePreviewMenu_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 4;
			this.menuItem4.Text = "-";
			// 
			// showPropertiesMenu
			// 
			this.showPropertiesMenu.Index = 5;
			this.showPropertiesMenu.Text = "Properties";
			this.showPropertiesMenu.Click += new System.EventHandler(this.showPropertiesMenu_Click);
			// 
			// ActiveRecordGraphView
			// 
			this.AllowedStates = WeifenLuo.WinFormsUI.ContentStates.Document;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(848, 394);
			this.CloseButton = false;
			this.Controls.Add(this.graphControl1);
			this.DockPadding.All = 2;
			this.Name = "ActiveRecordGraphView";
			this.Text = "ActiveRecord Graph";
			this.Load += new System.EventHandler(this.ActiveRecordGraphView_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private bool graphControl1_OnNewConnection(object sender, Netron.GraphLib.ConnectionEventArgs e)
		{
			return true;
		}

		private void graphControl1_OnShapeAdded(object sender, Netron.GraphLib.Shape shape)
		{
			graphControl1.Nodes.Remove(shape);

			if (shape is ActiveRecordShape)
			{
				ActiveRecordShape arshape = shape as ActiveRecordShape;
				
				if (_actionSet.DoNewARWizard())
				{
//					ConnectSubToSuperClass(
//						ObtainBaseShape(arshape), 
//						arshape);
				}
			}
			else if (shape is ActiveRecordBaseClassShape)
			{
				ActiveRecordBaseClassShape arshape = shape as ActiveRecordBaseClassShape;

//				if (_actionSet.DoAssociateDatabase(arshape))
//				{
//				}
			}
		}

		private Shape ObtainBaseShape(ActiveRecordShape arshape)
		{
			return _desc2Shape[arshape.ActiveRecordDescriptor.Table.DatabaseDefinition.ActiveRecordBaseDescriptor] as Shape;
		}

		private Shape ObtainShape(IActiveRecordDescriptor desc)
		{
			return _desc2Shape[desc] as Shape;
		}

		private void graphControl1_ShowNodeProperties(object sender, Netron.GraphLib.PropertyBag props)
		{
			ShowShapeProperties(props.Owner as Shape);
		}

		private void ShowShapeProperties(Shape shape)
		{
			if (shape is ActiveRecordBaseClassShape)
			{
				ActiveRecordBasePropertiesDialog d = new ActiveRecordBasePropertiesDialog();
				d.ShowDialog(this);
			}
			else if (shape is ActiveRecordShape)
			{
				ActiveRecordDescriptor ar = (shape as ActiveRecordShape).ActiveRecordDescriptor;

				ActiveRecordPropertiesDialog d = new ActiveRecordPropertiesDialog(ar);
				d.ShowDialog(this);
			}
		}

		private Connection ConnectSubToSuperClass(Shape parentShape, Shape childShape)
		{
			Connection conn = graphControl1.AddEdge(parentShape.Connectors[1], childShape.Connectors[0]);

			conn.LineEnd = ConnectionEnds.RightOpenArrow;

			return conn;
		}

		private void EnableWizard()
		{
			graphControl1.OnShapeAdded += new Netron.GraphLib.NewShape(graphControl1_OnShapeAdded);
		}

		private void DisableWizard()
		{
			graphControl1.OnShapeAdded -= new Netron.GraphLib.NewShape(graphControl1_OnShapeAdded);
		}

		private void graphControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (SelectedShape == null) return;

			if (e.Button == MouseButtons.Right)
			{
				contextMenu.Show(sender as Control, new Point(e.X, e.Y));
			}
		}

		private Shape SelectedShape
		{
			get
			{
				if (graphControl1.SelectedShapes == null || graphControl1.SelectedShapes.Count != 1) return null;
				return graphControl1.SelectedShapes[0];
			}
		}

		private void showPropertiesMenu_Click(object sender, System.EventArgs e)
		{
			ShowShapeProperties(SelectedShape);
		}

		#region ISubWorkspace Members

		public IWorkspace ParentWorkspace
		{
			get { return _parentWorkspace; }
			set { _parentWorkspace = value; }
		}

		#endregion

		#region IWorkspace Members

		public IWin32Window ActiveWindow
		{
			get { return this; }
		}

		public MainMenu MainMenu
		{
			get { return _parentWorkspace.MainMenu; }
		}

		public ToolBar MainToolBar
		{
			get { return _parentWorkspace.MainToolBar; }
		}

		public StatusBar MainStatusBar
		{
			get { return _parentWorkspace.MainStatusBar; }
		}

		public DockManager MainDockManager
		{
			get { return null; }
		}

		#endregion

		private void OnProjectChange(object sender, Project project)
		{
			DisableWizard();

			Random rnd = new Random(1);

			foreach(IActiveRecordDescriptor descriptor in project.Descriptors)
			{
				Shape shape = ObtainShape(descriptor);

				if (graphControl1.Nodes.Contains(shape)) continue;

				shape = CreateShapeFrom(descriptor);

				if (shape.X == 0 && shape.Y == 0)
				{
					shape.X = rnd.Next(50, graphControl1.Width - 100);
					shape.Y = rnd.Next(50, graphControl1.Height - 20);
				}

				CreateConnectionsIfNecessary(shape);
			}

			RefreshView();

			EnableWizard();
		}

		private void _model_OnProjectReplaced(object sender, Project oldProject, Project newProject)
		{
			// Clear!

			graphControl1.Nodes.Clear();
			this._desc2Shape.Clear();
			
			OnProjectChange(sender, newProject);
		}

		private void RefreshView()
		{
			graphControl1.Invalidate();
		}

		private Shape CreateShapeFrom(IActiveRecordDescriptor descriptor)
		{
			Shape shape = null;

			if (descriptor is ActiveRecordBaseDescriptor)
			{
				ActiveRecordBaseClassShape arshape = (ActiveRecordBaseClassShape)
					graphControl1.AddShape("castle.ar.base.shape", new PointF(80, 20));
				arshape.RelatedDescriptor = descriptor as ActiveRecordBaseDescriptor;

				arshape.FitSize(false);

				shape = arshape;
			}
			else
			{
				ActiveRecordShape arshape = (ActiveRecordShape)
					graphControl1.AddShape("castle.ar.shape", new PointF(80, 20));
				
				arshape.ActiveRecordDescriptor = (ActiveRecordDescriptor) descriptor;

				shape = arshape;
			}

			System.Diagnostics.Debug.Assert( shape != null );

			shape.X = descriptor.PositionInView.X;
			shape.Y = descriptor.PositionInView.Y;

			_desc2Shape[descriptor] = shape;

			return shape;
		}

		private void CreateConnectionsIfNecessary(Shape shape)
		{
			ActiveRecordShape arshape = shape as ActiveRecordShape;

			if (arshape != null)
			{
				if (arshape.ActiveRecordDescriptor is ActiveRecordDescriptorSubClass)
				{
					ActiveRecordDescriptorSubClass subclass = arshape.ActiveRecordDescriptor as ActiveRecordDescriptorSubClass;
					ConnectSubToSuperClass( ObtainShape(subclass.BaseClass), shape );
				}
				else
				{
					ConnectSubToSuperClass( ObtainBaseShape(arshape), shape );
				}
			}
		}

		private void addSubClassMenu_Click(object sender, System.EventArgs e)
		{
			if (SelectedShape is ActiveRecordShape)
			{
				_actionSet.CreateSubClass( (SelectedShape as ActiveRecordShape).ActiveRecordDescriptor );
			}
		}

		private void addJoinedSubclassMenu_Click(object sender, System.EventArgs e)
		{
			if (SelectedShape is ActiveRecordShape)
			{
				_actionSet.CreateJoinedSubClass( (SelectedShape as ActiveRecordShape).ActiveRecordDescriptor );
			}
		}

		private void showCodePreviewMenu_Click(object sender, System.EventArgs e)
		{
//			_actionSet.PreviewCode( SelectedShape );
		}
	}
}