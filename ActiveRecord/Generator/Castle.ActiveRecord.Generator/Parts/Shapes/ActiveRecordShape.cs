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

namespace Castle.ActiveRecord.Generator.Parts.Shapes
{
	using System;
	using System.IO;
	using System.Drawing;
	using System.Diagnostics;
	using System.Reflection;

	using Netron.GraphLib;
	using Netron.GraphLib.Interfaces;
	using Netron.GraphLib.Attributes;

	using Castle.ActiveRecord.Generator.Components.Database;


	[Serializable]
	[NetronGraphShape("ActiveRecord", 
		"castle.ar.shape", "ActiveRecord", 
		"Castle.ActiveRecord.Generator.Parts.Shapes.ActiveRecordShape", 
		"Represents an ActiveRecord class")]
	public class ActiveRecordShape : AbstractARShape
	{
		private float _lineHeight;

		private Connector TopNode;
		private Connector BottomNode;

		public ActiveRecordShape() : base()
		{
			Init(false);
		}

		public ActiveRecordShape(IGraphSite site) : base(site)
		{
			Init(true);
		}

		private void Init(bool resizable)
		{
			Rectangle = new RectangleF(0, 0, 70, 50);

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
			Bitmap bmp=null;
			try
			{
				Stream stream = 
					Assembly.GetExecutingAssembly().GetManifestResourceStream(
						"Castle.ActiveRecord.Generator.Parts.Shapes.Resources.ARShape.gif");
					
				bmp= Bitmap.FromStream(stream) as Bitmap;
				stream.Close();
				stream=null;
			}
			catch(Exception exc)
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

			String className = "ClassName";
			String alias = "[Alias]";
			String tableName = "TableName";

			if (ActiveRecordDescriptor != null)
			{
				className = ActiveRecordDescriptor.ClassName;
				alias = ActiveRecordDescriptor.Table.DatabaseDefinition.Alias;
				tableName = ActiveRecordDescriptor.Table.Name;

				// Lets update the position
				ActiveRecordDescriptor.PositionInView = new PointF(X, Y);
			}
			else
			{
				className = "Class name";
				alias = "[alias]";
				tableName = "table";
			}

			if (recalculateSize)
			{
				SizeF size1 = g.MeasureString(className + "\"\"", mFont);
				SizeF size2 = g.MeasureString(alias + "[]", mFont);
				SizeF size3 = g.MeasureString(tableName, mFont);

				_lineHeight = size1.Height;

				SizeF max = new SizeF(Math.Max(size1.Width, Math.Max(size2.Width, size3.Width)) + 3,
										(_lineHeight*3) + 10);

				Rectangle = new RectangleF(new PointF(Rectangle.X, Rectangle.Y), max);

				recalculateSize = false;
			}

			g.FillRectangle(BackgroundBrush, Rectangle.X, Rectangle.Y, Rectangle.Width + 1, Rectangle.Height + 1);
			g.DrawRectangle(pen, Rectangle.X, Rectangle.Y, Rectangle.Width + 1, Rectangle.Height + 1);
			g.DrawLine(pen, Rectangle.X, Rectangle.Y + _lineHeight + 3, Rectangle.X + Rectangle.Width, Rectangle.Y + _lineHeight + 3);

			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			g.DrawString(className, mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + 3, sf);
			g.DrawString(tableName, mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + _lineHeight + 6, sf);
			g.DrawString(alias, mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + (_lineHeight * 2) + 6, sf);
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

		public ActiveRecordDescriptor ActiveRecordDescriptor
		{
			get { return ARDescriptor as ActiveRecordDescriptor; }
			set { ARDescriptor = value; }
		}
	}
}