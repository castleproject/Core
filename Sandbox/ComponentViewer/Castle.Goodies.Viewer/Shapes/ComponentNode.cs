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

namespace Castle.Goodies.Viewer.Shapes
{
	using System;
	using System.Drawing;
	using System.ComponentModel;

	using Netron.GraphLib;
	using Netron.GraphLib.Attributes;
	using Netron.GraphLib.Interfaces;

	using Castle.Model;


	[Serializable]
	[Description("Component node")]
	[NetronGraphShape("Component node", "castle.comp.node", "Component node", "Castle.Goodies.Viewer.Shapes.ComponentNode")]
	public class ComponentNode : Shape
	{
		private String _key;
		private String _implementation;
		private float _lineHeight;

		private Connector TopNode;
		private Connector BottomNode;

		public ComponentNode() : base()
		{
			Init(false);
		}

		public ComponentNode(IGraphSite site) : base(site)
		{
			Init(true);
		}

		public String Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public String Implementation
		{
			get { return _implementation; }
			set { _implementation = value; }
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
		/// Paints the shape of the person object in the plex. Here you can let your imagination go.
		/// MAKE IT PERFORMANT, this is a killer method called 200.000 times a minute!
		/// </summary>
		/// <param name="g">The graphics canvas onto which to paint</param>
		protected override void Paint(Graphics g)
		{
			base.Paint(g);

			if (recalculateSize)
			{
				SizeF size1 = g.MeasureString(Key + "\"\"", mFont);
				SizeF size2 = g.MeasureString(mText, mFont);
				SizeF size3 = g.MeasureString(Implementation, mFont);

				_lineHeight = size1.Height;

				SizeF max = new SizeF( Math.Max(size1.Width, Math.Max(size2.Width, size3.Width) ) + 3, 
					(_lineHeight * 3) + 10 );

				Rectangle = new RectangleF(new PointF(Rectangle.X, Rectangle.Y), max);
				
				recalculateSize = false;
			}

			g.FillRectangle(BackgroundBrush, Rectangle.X, Rectangle.Y, Rectangle.Width + 1, Rectangle.Height + 1);
			g.DrawRectangle(pen, Rectangle.X, Rectangle.Y, Rectangle.Width + 1, Rectangle.Height + 1);
			g.DrawLine(pen, Rectangle.X, Rectangle.Y + _lineHeight + 3, Rectangle.X + Rectangle.Width, Rectangle.Y + _lineHeight + 3);
			
			ComponentModel model = Tag as ComponentModel;

			if (model != null)
			{
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				g.DrawString("\"" + Key + "\"", mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + 3, sf);
				g.DrawString(mText, mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + _lineHeight + 6, sf);
				g.DrawString(Implementation, mFont, TextBrush, Rectangle.X + (Rectangle.Width/2), Rectangle.Y + (_lineHeight * 2) + 6, sf);
			}
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