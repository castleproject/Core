namespace Castle.ActiveRecord.Generator.Parts
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using Netron.GraphLib;
	using WeifenLuo.WinFormsUI;

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

		public ActiveRecordGraphView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			graphControl1.AddLibrary("BasicShapes.dll");
			graphControl1.AddLibrary("spikegen.exe");

//			ActiveRecordNode shape = (ActiveRecordNode) graphControl1.AddShape(
//				"castle.comp.node2", new PointF(80,20)); 
//				
//			shape.Key = "some key";
//			shape.Text = "value";
//			shape.Implementation = "";
//
//			shape.Tag = node;
//			shape.FitSize(false);
//			Random rnd = new Random(1);
//			shape.X = rnd.Next(50,this.graphControl1.Width-100);
//			shape.Y = rnd.Next(50,this.graphControl1.Height-20);
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
			this.graphControl1.AllowAddConnection = false;
			this.graphControl1.AllowAddShape = false;
			this.graphControl1.AllowDeleteShape = false;
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
			this.graphControl1.GradientBottom = System.Drawing.Color.White;
			this.graphControl1.GradientTop = System.Drawing.Color.LightSteelBlue;
			this.graphControl1.GraphLayoutAlgorithm = Netron.GraphLib.GraphLayoutAlgorithms.SpringEmbedder;
			this.graphControl1.GridSize = 15;
			this.graphControl1.Location = new System.Drawing.Point(2, 2);
			this.graphControl1.Name = "graphControl1";
			this.graphControl1.RestrictToCanvas = true;
			this.graphControl1.ShowGrid = true;
			this.graphControl1.Size = new System.Drawing.Size(844, 390);
			this.graphControl1.Snap = true;
			this.graphControl1.TabIndex = 0;
			this.graphControl1.Text = "graphControl1";
			this.graphControl1.Zoom = 1F;
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
	}
}
