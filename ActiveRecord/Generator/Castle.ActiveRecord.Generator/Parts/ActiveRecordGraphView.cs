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


	/// <summary>
	/// Summary description for ActiveRecordGraphView
	/// </summary>
	public class ActiveRecordGraphView : Content
	{
		private Netron.GraphLib.UI.GraphControl graphControl1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Model _model;

		public ActiveRecordGraphView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

//			graphControl1.AddLibrary("BasicShapes.dll");
			graphControl1.AddLibrary("Castle.ActiveRecord.Generator.exe");
			graphControl1.EnableContextMenu = true;

//			ActiveRecordShape shape = (ActiveRecordShape) graphControl1.AddShape(
//				"castle.ar.shape", new PointF(80,20)); 
//				
//			shape.Name = "Company";
//			shape.Table = "tb_Companies";
////			shape.Tag = node;
//			shape.FitSize(false);
//			Random rnd = new Random(1);
//			shape.X = rnd.Next(50,this.graphControl1.Width-100);
//			shape.Y = rnd.Next(50,this.graphControl1.Height-20);
//
//			shape = (ActiveRecordShape) graphControl1.AddShape(
//				"castle.ar.shape", new PointF(80,20)); 
//			shape.Name = "ActiveRecordBase";
//			shape.Table = "using 'MyAlias' db";
//			shape.FitSize(false);
//			shape.X = rnd.Next(50,this.graphControl1.Width-100);
//			shape.Y = rnd.Next(50,this.graphControl1.Height-20);
//
//
//
//			shape = (ActiveRecordShape) graphControl1.AddShape(
//				"castle.ar.shape", new PointF(80,20)); 
//				
//			shape.Name = "Person";
//			shape.Table = "tb_People";
//			//			shape.Tag = node;
//			shape.FitSize(false);
//			shape.X = rnd.Next(50,this.graphControl1.Width-100);
//			shape.Y = rnd.Next(50,this.graphControl1.Height-20);
//
//			shape = (ActiveRecordShape) graphControl1.AddShape(
//				"castle.ar.shape", new PointF(80,20)); 
//				
//			shape.Name = "Firm";
//			shape.Table = "extends Company";
//			//			shape.Tag = node;
//			shape.FitSize(false);
//			shape.X = rnd.Next(50,this.graphControl1.Width-100);
//			shape.Y = rnd.Next(50,this.graphControl1.Height-20);
//
//
//			shape = (ActiveRecordShape) graphControl1.AddShape(
//				"castle.ar.shape", new PointF(80,20)); 
//				
//			shape.Name = "Client";
//			shape.Table = "extends Company";
//			//			shape.Tag = node;
//			shape.FitSize(false);
//			shape.X = rnd.Next(50,this.graphControl1.Width-100);
//			shape.Y = rnd.Next(50,this.graphControl1.Height-20);
		}

		public ActiveRecordGraphView(Model model) : this()
		{
			_model = model;
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
			this.graphControl1 = new Netron.GraphLib.UI.GraphControl();
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
			this.graphControl1.OnShapeAdded += new Netron.GraphLib.NewShape(this.graphControl1_OnShapeAdded);
			this.graphControl1.OnClear += new System.EventHandler(this.graphControl1_OnClear);
			this.graphControl1.OnShapeRemoved += new Netron.GraphLib.NewShape(this.graphControl1_OnShapeRemoved);
			this.graphControl1.OnContextMenu += new System.Windows.Forms.MouseEventHandler(this.graphControl1_OnContextMenu);
			this.graphControl1.ShowNodeProperties += new Netron.GraphLib.ShowPropsDelegate(this.graphControl1_ShowNodeProperties);
			this.graphControl1.OnInfo += new Netron.GraphLib.InfoDelegate(this.graphControl1_OnInfo);
			this.graphControl1.OnNewConnection += new Netron.GraphLib.NewConnection(this.graphControl1_OnNewConnection);
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
			this.ResumeLayout(false);

		}
		#endregion

		private void graphControl1_OnContextMenu(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		private void graphControl1_OnInfo(object obj)
		{
			
		}

		private bool graphControl1_OnNewConnection(object sender, Netron.GraphLib.ConnectionEventArgs e)
		{
			return true;
		}

		private void graphControl1_OnShapeAdded(object sender, Netron.GraphLib.Shape shape)
		{
			if (shape is ActiveRecordShape)
			{
				using(NewARClassWizard wizard = new NewARClassWizard(_model, shape))
				{
					if (wizard.ShowDialog(this) != DialogResult.OK)
					{
						graphControl1.Nodes.Remove(shape);
					}
				}
			}
		}

		private void graphControl1_OnShapeRemoved(object sender, Netron.GraphLib.Shape shape)
		{
			// TODO: Do not allow the removal of ARBase
		}

		private void graphControl1_ShowNodeProperties(object sender, Netron.GraphLib.PropertyBag props)
		{
			if (props.Owner is ActiveRecordBaseClassShape)
			{
				ActiveRecordBasePropertiesDialog d = new ActiveRecordBasePropertiesDialog();
				d.ShowDialog(this);
			}
			else if (props.Owner is ActiveRecordShape)
			{
				ActiveRecordPropertiesDialog d = new ActiveRecordPropertiesDialog();
				d.ShowDialog(this);
			}
		}

		private void graphControl1_OnClear(object sender, System.EventArgs e)
		{
		}
	}
}
