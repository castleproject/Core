// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Generator.Parts.Shapes
{
	using System;
	using System.IO;
	using System.Drawing;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Reflection;

	using Netron.GraphLib;
	using Netron.GraphLib.UI;
	using Netron.GraphLib.Interfaces;
	using Netron.GraphLib.Attributes;

	using Castle.ActiveRecord.Generator.Components.Database;


	[Serializable]
	[NetronGraphShape("ActiveRecordBase", 
		"castle.ar.base.shape", 
		"ActiveRecord", "Castle.ActiveRecord.Generator.Parts.Shapes.ActiveRecordBaseClassShape", 
		"Represents an ActiveRecord base class that maps a database connection")]
	public class ActiveRecordBaseClassShape : AbstractARShape
	{
		private float _lineHeight;

		private Connector TopNode;
		private Connector BottomNode;

		public ActiveRecordBaseClassShape() : base()
		{
			Init(false);
		}

		public ActiveRecordBaseClassShape(IGraphSite site) : base(site)
		{
			Init(true);
		}

		public ActiveRecordBaseDescriptor RelatedDescriptor
		{
			get { return ARDescriptor as ActiveRecordBaseDescriptor; }
			set { ARDescriptor = value; }
		}

		private void Init(bool resizable)
		{
			Rectangle = new RectangleF(0, 0, 70, 50);
			this.NodeColor = System.Drawing.SystemColors.Window;

			TopNode = new Connector(this, "Top", true);
			TopNode.ConnectorLocation = ConnectorLocations.North;
			Connectors.Add(TopNode);

			BottomNode = new Connector(this, "Bottom", true);
			BottomNode.ConnectorLocation = ConnectorLocations.South;
			Connectors.Add(BottomNode);

			Resizable = resizable;
			recalculateSize = true;
		}

		/// <summary>
		/// Overrides the default thumbnail used in the shape viewer
		/// </summary>
		/// <returns></returns>
		public override Bitmap GetThumbnail()
		{
			Bitmap bmp = null;
			try
			{
				Stream stream =
					Assembly.GetExecutingAssembly().GetManifestResourceStream(
						"Castle.ActiveRecord.Generator.Parts.Shapes.Resources.ARBaseShape.gif");

				bmp = Bitmap.FromStream(stream) as Bitmap;
				stream.Close();
				stream = null;
			}
			catch (Exception exc)
			{
				Trace.WriteLine(exc.Message);
			}
			return bmp;
		}

		/// <summary>
		/// Paints the shape of the person object in the plex. Here you can let your imagination go.
		/// MAKE IT PERFORMANT, this is a killer method called 200.000 times a minute!
		/// </summary>
		/// <param name="g">The graphics canvas onto which to paint</param>
		protected override void Paint(Graphics g)
		{
			base.Paint(g);

			if (RelatedDescriptor == null) return;

			// Lets update the position
			RelatedDescriptor.PositionInView = new PointF(X, Y);

			if (recalculateSize)
			{
				SizeF max = g.MeasureString(RelatedDescriptor.ClassName + "\"\"", mFont);

				_lineHeight = max.Height;

//				SizeF max = new SizeF(Math.Max(size1.Width, size3.Width) + 3,
//				                      (_lineHeight*2) + 10);

				Rectangle = new RectangleF(new PointF(Rectangle.X, Rectangle.Y), max);

				recalculateSize = false;
			}

			g.FillRectangle(BackgroundBrush, Rectangle.X, Rectangle.Y, Rectangle.Width + 1, Rectangle.Height + 1);
			g.DrawRectangle(pen, Rectangle.X, Rectangle.Y, Rectangle.Width + 1, Rectangle.Height + 1);
//			g.DrawLine(pen, Rectangle.X, Rectangle.Y + _lineHeight + 3, Rectangle.X + Rectangle.Width, Rectangle.Y + _lineHeight + 3);

			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			g.DrawString(RelatedDescriptor.ClassName, mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + 3, sf);
			//			g.DrawString(mText, mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + _lineHeight + 6, sf);
			//			g.DrawString(Table, mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + (_lineHeight*2) + 6, sf);
			//			g.DrawString("[not mapped]", mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + (_lineHeight) + 6, sf);
		}

		/// <summary>
		/// Returns a floating-point point coordinates for a given connector
		/// </summary>
		/// <param name="c">A connector object</param>
		/// <returns>A floating-point pointF</returns>
		public override PointF ConnectionPoint(Connector c)
		{
			if (c == TopNode) return new PointF(Rectangle.Left + (Rectangle.Width*1/2), Rectangle.Top);
			if (c == BottomNode) return new PointF(Rectangle.Left + (Rectangle.Width*1/2), Rectangle.Bottom);
			return new PointF(0, 0);
		}
	}
}