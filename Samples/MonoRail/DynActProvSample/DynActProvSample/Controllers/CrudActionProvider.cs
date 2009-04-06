// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace DynActProvSample.Controllers
{
	using System;
	using Castle.ActiveRecord;
	using Castle.Components.Binder;
	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;

	[AttributeUsage(AttributeTargets.Class)]
	public class CrudAttribute : Attribute
	{
		private readonly Type activeRecordType;

		public CrudAttribute(Type activeRecordType)
		{
			this.activeRecordType = activeRecordType;
		}

		public Type ActiveRecordType
		{
			get { return activeRecordType; }
		}
	}

	/// <summary>
	/// Includes dynamic actions on the controller
	/// </summary>
	public class CrudActionProvider : IDynamicActionProvider
	{
		public void IncludeActions(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			Type controllerType = controller.GetType();

			object[] atts = controllerType.GetCustomAttributes(typeof(CrudAttribute), false);

			if (atts.Length == 0)
			{
				throw new Exception("CrudAttribute not used on " + controllerType.Name);
			}

			CrudAttribute crudAtt = (CrudAttribute) atts[0];

			Type arType = crudAtt.ActiveRecordType;
			String prefix = arType.Name.ToLower();

			controllerContext.DynamicActions["list"] = new ListAction(arType);
			controllerContext.DynamicActions["new"] = new NewAction();
			controllerContext.DynamicActions["create"] = new CreateAction(arType, prefix);
			controllerContext.DynamicActions["edit"] = new EditAction(arType, prefix);
			controllerContext.DynamicActions["update"] = new UpdateAction(arType, prefix);
			controllerContext.DynamicActions["confirmdelete"] = new ConfirmDeleteAction(arType, prefix);
			controllerContext.DynamicActions["delete"] = new DeleteAction(arType, prefix);
		}
	}

	public class ListAction : IDynamicAction
	{
		private readonly Type arType;

		public ListAction(Type arType)
		{
			this.arType = arType;
		}

		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			controllerContext.PropertyBag["items"] = ActiveRecordMediator.FindAll(arType);

			new RenderingSupport(controllerContext, engineContext).RenderView("list");

			return null;
		}
	}

	public class NewAction : IDynamicAction
	{
		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			new RenderingSupport(controllerContext, engineContext).RenderView("new");
			
			return null;
		}
	}

	public class CreateAction : IDynamicAction
	{
		private readonly Type arType;
		private readonly string prefix;

		public CreateAction(Type arType, String prefix)
		{
			this.arType = arType;
			this.prefix = prefix;
		}

		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			object instance = null;

			try
			{
				ARSmartDispatcherController arController =
					(ARSmartDispatcherController) controller;

				ARDataBinder binder = (ARDataBinder) arController.Binder;
				binder.AutoLoad = AutoLoadBehavior.OnlyNested;

				TreeBuilder builder = new TreeBuilder();

				instance = binder.BindObject(
					arType, prefix,
					builder.BuildSourceNode(engineContext.Request.Form));

				ActiveRecordMediator.Create(instance);

				engineContext.Response.Redirect(controllerContext.Name, "list");
			}
			catch(Exception ex)
			{
				engineContext.Flash["errormessage"] = ex.Message;
				engineContext.Flash[prefix] = instance;

				engineContext.Response.Redirect(controllerContext.Name, "new");
			}

			return null;
		}
	}

	public class EditAction : IDynamicAction
	{
		private readonly Type arType;
		private readonly string prefix;

		public EditAction(Type arType, String prefix)
		{
			this.arType = arType;
			this.prefix = prefix;
		}

		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			int id = Convert.ToInt32(engineContext.Request.QueryString["id"]);

			controllerContext.PropertyBag[prefix] =
				ActiveRecordMediator.FindByPrimaryKey(arType, id);

			new RenderingSupport(controllerContext, engineContext).RenderView("edit");

			return null;
		}
	}

	public class UpdateAction : IDynamicAction
	{
		private readonly Type arType;
		private readonly string prefix;

		public UpdateAction(Type arType, String prefix)
		{
			this.arType = arType;
			this.prefix = prefix;
		}

		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			object instance = null;

			try
			{
				ARSmartDispatcherController arController =
					(ARSmartDispatcherController) controller;

				ARDataBinder binder = (ARDataBinder) arController.Binder;
				binder.AutoLoad = AutoLoadBehavior.Always;

				TreeBuilder builder = new TreeBuilder();

				instance = binder.BindObject(
					arType, prefix,
					builder.BuildSourceNode(engineContext.Request.Form));

				ActiveRecordMediator.Update(instance);

				engineContext.Response.Redirect(controllerContext.Name, "list");
			}
			catch(Exception ex)
			{
				engineContext.Flash["errormessage"] = ex.Message;
				engineContext.Flash[prefix] = instance;

				engineContext.Response.Redirect(controllerContext.Name, "edit", engineContext.Request.QueryString);
			}

			return null;
		}
	}

	public class ConfirmDeleteAction : IDynamicAction
	{
		private readonly Type arType;
		private readonly string prefix;

		public ConfirmDeleteAction(Type arType, String prefix)
		{
			this.arType = arType;
			this.prefix = prefix;
		}

		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			int id = Convert.ToInt32(engineContext.Request.QueryString["id"]);

			controllerContext.PropertyBag[prefix] =
				ActiveRecordMediator.FindByPrimaryKey(arType, id);

			new RenderingSupport(controllerContext, engineContext).RenderView("confirmdelete");

			return null;
		}
	}

	public class DeleteAction : IDynamicAction
	{
		private readonly Type arType;
		private readonly string prefix;

		public DeleteAction(Type arType, String prefix)
		{
			this.arType = arType;
			this.prefix = prefix;
		}

		public object Execute(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			object instance = null;

			try
			{
				int id = Convert.ToInt32(engineContext.Request.Form[prefix + ".id"]);

				instance = ActiveRecordMediator.FindByPrimaryKey(arType, id);

				ActiveRecordMediator.Delete(instance);

				engineContext.Response.Redirect(controllerContext.Name, "list");
			}
			catch(Exception ex)
			{
				engineContext.Flash["errormessage"] = ex.Message;
				engineContext.Flash[prefix] = instance;

				engineContext.Response.Redirect(controllerContext.Name, "confirmdelete", engineContext.Request.QueryString);
			}

			return null;
		}
	}
}