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

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Renders an edit form
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>edit{name}</c>
	/// </remarks>
	public class EditAction : NewAction
	{
		protected object idVal;

		public EditAction(Type modelType) : base(modelType)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\edit{1}", controller.Name, Model.Type.Name);
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
			base.GenerateHtmlForm( Model.Type.Name, Model, instance, controller, "Edit" );
		}

		protected override void AddHiddenFields(StringBuilder sb)
		{
			sb.Append( helper.InputHidden( keyProperty.Name, idVal.ToString() ) );
		}

		protected override void CreateForm(StringBuilder sb, String name, Controller controller)
		{
			sb.Append( helper.Form( String.Format("update{0}.{1}", 
				name, controller.Context.UrlInfo.Extension) ) );
		}

		protected void ReadPkFromParams(Controller controller)
		{
			ObtainPKProperty();
	
			String id = controller.Context.Params["id"];
	
			if (id == null)
			{
				throw new ScaffoldException("Can't edit without the proper id");
			}
	
			idVal = DataBinder.Convert( keyProperty.PropertyType, id, "id", null, controller.Context );
		}
	}
}
