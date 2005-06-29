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

	using Castle.MonoRail.Framework;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Removes the ActiveRecord instance
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>{name}removed</c>
	/// </remarks>
	public class RemoveAction : EditAction
	{
		private bool removalSuccessful;

		public RemoveAction(Type modelType) : base(modelType)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\{1}removed", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			ReadPkFromParams(controller);

			try
			{
				instance = SupportingUtils.FindByPK( Model.Type, idVal );
			}
			catch(Exception ex)
			{
				throw new ScaffoldException("Could not obtain instance by using this id", ex);
			}

			try
			{
				(instance as ActiveRecordBase).Delete();

				removalSuccessful = true;
			}
			catch(Exception)
			{
				removalSuccessful = false;
			}

			controller.PropertyBag["armodel"] = Model;
			controller.PropertyBag["item"] = instance;
			controller.PropertyBag["removalSuccessful"] = removalSuccessful;
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<p>");

			if (removalSuccessful)
			{
				sb.AppendFormat("{0} was removed successfully.", Model.Type.Name);
			}
			else
			{
				sb.Append("<font color=\"red\">");
				sb.AppendFormat("{0} could not be removed...", Model.Type.Name);
				sb.Append("</font>");
			}

			sb.Append("</p>");

			sb.Append( helper.LinkTo( "Back to list", "list" + Model.Type.Name ) );

			controller.DirectRender(sb.ToString());
		}
	}
}
