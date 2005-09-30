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

namespace Castle.MonoRail.ActiveRecordScaffold
{
	using System;
	using System.Text;

	using Castle.ActiveRecord.Framework;
	using Castle.Components.Common.TemplateEngine;
	using Castle.MonoRail.Framework;


	/// <summary>
	/// Displays a confirmation message before performing 
	/// the removal of the instance
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>confirm{name}remove</c>
	/// </remarks>
	public class ConfirmRemoveAction : EditAction
	{
		public ConfirmRemoveAction(Type modelType, ITemplateEngine templateEngine) : base(modelType, templateEngine)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\confirm{1}remove", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			ReadPkFromParams(controller);

			try
			{
				instance = SupportingUtils.FindByPK( Model.Type, idVal );
				
				controller.PropertyBag["armodel"] = Model;
				controller.PropertyBag["item"] = instance;
			}
			catch(Exception ex)
			{
				throw new ScaffoldException("Could not obtain instance by using this id", ex);
			}
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<p>");

			sb.AppendFormat("Confirm removal of {0} ?", instance.ToString());

			sb.Append("</p>");

			sb.Append("<p>");
			sb.Append( helper.LinkTo( "Yes", controller.Name, "remove" + Model.Type.Name, idVal ) );
			sb.Append("  |  ");
			sb.Append( helper.LinkTo( "No", "list" + Model.Type.Name ) );
			sb.Append("</p>");

			controller.DirectRender(sb.ToString());
		}
	}
}
