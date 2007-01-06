// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

#if DOTNET2

[assembly :
	System.Web.UI.WebResource("Castle.MonoRail.Framework.Views.Aspx.ControllerBinder.Design.Castle.gif", "image/gif")]
[assembly:
	System.Web.UI.WebResource("Castle.MonoRail.Framework.Views.Aspx.ControllerBinder.Design.MonoRail.gif", "image/gif")]

namespace Castle.MonoRail.Framework.Views.Aspx.Design
{
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Web.UI;
	using System.Web.UI.Design;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	internal class ControllerActionBinderDesigner : ControlDesigner
	{
		private ControllerBinder binder;

		public override void Initialize(IComponent component)
		{
			binder = (ControllerBinder) component;

			// The Asp.Net Web Forms Designer does not seem to
			// recognize the ISupportInitialize interface.

			((ISupportInitialize)binder).BeginInit();
			base.Initialize(component);
			((ISupportInitialize)binder).EndInit();
		}

		public override string GetDesignTimeHtml()
		{
			StringWriter sw = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter(sw);

			Panel panel = new Panel();
			panel.BackColor = Color.WhiteSmoke;
			panel.Width = new Unit("100%");

			HtmlTable table = new HtmlTable();
			table.Attributes["align"] = "center";
			HtmlTableRow row = new HtmlTableRow();

			HtmlTableCell cell1 = new HtmlTableCell();
			cell1.Align = "left";
			cell1.VAlign = "middle";

			HtmlImage castleImg = new HtmlImage();
			castleImg.Style["margin"] = "4px";
			castleImg.Src = binder.Page.ClientScript.GetWebResourceUrl(
				GetType(), "Castle.MonoRail.Framework.Views.Aspx.ControllerBinder.Design.Castle.gif");
			cell1.Controls.Add(castleImg);
			row.Cells.Add(cell1);

			HtmlTableCell cell2 = new HtmlTableCell();
			cell1.Align = "left";
			cell1.VAlign = "middle";

			HtmlImage monoRailImg = new HtmlImage();
			monoRailImg.Src = binder.Page.ClientScript.GetWebResourceUrl(
				GetType(), "Castle.MonoRail.Framework.Views.Aspx.ControllerBinder.Design.MonoRail.gif");
			cell2.Controls.Add(monoRailImg);
			row.Cells.Add(cell2);

			HtmlTableCell cell3 = new HtmlTableCell();
			cell3.Align = "center";
			cell3.VAlign = "middle";
			cell3.Attributes["style"] = "font-family: verdana, tahoma, arial, sans-serif; font-size: 0.9em; color:#5266A6";
			LiteralControl caption = new LiteralControl();
			int bindingCount = binder.ControllerBindings.Count;
			caption.Text = string.Format("<b>Controller Binder</b> - {0} binding{1}",
				bindingCount, bindingCount != 1 ? "s" : "");
			cell3.Controls.Add(caption);
			row.Cells.Add(cell3);

			table.Rows.Add(row);

			panel.Controls.Add(table);

			// Get the HTML produced by the control.
			panel.RenderControl(writer);
			return sw.ToString();
		}
	}
}

#endif