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
	using System.Collections;

	using Castle.MonoRail.Framework;

	using Castle.ActiveRecord;

	/// <summary>
	/// Performs the inclusion
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>create{name}</c>
	/// </remarks>
	public class CreateAction : NewAction
	{
		private ArrayList errors = new ArrayList();
		private DataBinder binder;
		private IRailsEngineContext context;

		public CreateAction(Type modelType) : base(modelType)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			if (controller.Context.Flash["errors"] != null)
			{
				return base.ComputeTemplateName(controller);
			}
			else
			{
				return String.Format(@"{0}\create{1}", controller.Name, Model.Type.Name);
			}
		}

		protected override void PerformActionProcess(Controller controller)
		{
			context = controller.Context;

			binder = new DataBinder(controller.Context);
			
			instance = binder.BindObject( Model.Type );

			try
			{
				SaveInstance(instance, controller);
			}
			catch(Exception ex)
			{
				errors.Add( "Could not save " + Model.Type.Name + ". " + ex.Message );
			}

			if (errors.Count != 0)
			{
				controller.Context.Flash["errors"] = errors;
			}

			controller.PropertyBag["armodel"] = Model;
			controller.PropertyBag["instance"] = instance;
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			if (controller.Context.Flash["errors"] != null)
			{
				base.RenderStandardHtml(controller);
			}
			else
			{
				controller.Redirect( controller.Name, "list" + Model.Type.Name );
			}
		}

		protected void SaveInstance(object instance, Controller controller)
		{
			if (instance is ActiveRecordValidationBase)
			{
				ActiveRecordValidationBase instanceBase = instance as ActiveRecordValidationBase;

				if (!instanceBase.IsValid())
				{
					errors.AddRange(instanceBase.ValidationErrorMessages);
					prop2Validation = instanceBase.PropertiesValidationErrorMessage;
				}
				else
				{
					instanceBase.Save();
				}
			}
			else
			{
				ActiveRecordBase instanceBase = instance as ActiveRecordBase;

				instanceBase.Save();
			}
		}
	}
}
