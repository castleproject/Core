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
	public void IncludeActions(Controller controller)
	{
		Type controllerType = controller.GetType();

		object[] atts = controllerType.GetCustomAttributes(typeof(CrudAttribute), false);

		if (atts.Length == 0)
		{
			throw new Exception("CrudAttribute not used on " + controllerType.Name);
		}

		CrudAttribute crudAtt = (CrudAttribute)atts[0];

		Type arType = crudAtt.ActiveRecordType;
		String prefix = arType.Name.ToLower();

		controller.DynamicActions["list"] = new ListAction(arType);
		controller.DynamicActions["new"] = new NewAction();
		controller.DynamicActions["create"] = new CreateAction(arType, prefix);
		controller.DynamicActions["edit"] = new EditAction(arType, prefix);
		controller.DynamicActions["update"] = new UpdateAction(arType, prefix);
		controller.DynamicActions["confirmdelete"] = new ConfirmDeleteAction(arType, prefix);
		controller.DynamicActions["delete"] = new DeleteAction(arType, prefix);
	}
}

	public class ListAction : IDynamicAction
	{
		private readonly Type arType;

		public ListAction(Type arType)
		{
			this.arType = arType;
		}

		public void Execute(Controller controller)
		{
			controller.PropertyBag["items"] = ActiveRecordMediator.FindAll(arType);

			controller.RenderView("list");
		}
	}

	public class NewAction : IDynamicAction
	{
		public void Execute(Controller controller)
		{
			controller.RenderView("new");
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

		public void Execute(Controller controller)
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
					builder.BuildSourceNode(controller.Form));

				ActiveRecordMediator.Create(instance);
				
				controller.Redirect(controller.Name, "list");
			}
			catch(Exception ex)
			{
				controller.Flash["errormessage"] = ex.Message;
				controller.Flash[prefix] = instance;

				controller.Redirect(controller.Name, "new");
			}
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

		public void Execute(Controller controller)
		{
			int id = Convert.ToInt32(controller.Query["id"]);

			controller.PropertyBag[prefix] = 
				ActiveRecordMediator.FindByPrimaryKey(arType, id);
			
			controller.RenderView("edit");
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

		public void Execute(Controller controller)
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
					builder.BuildSourceNode(controller.Form));

				ActiveRecordMediator.Update(instance);

				controller.Redirect(controller.Name, "list");
			}
			catch (Exception ex)
			{
				controller.Flash["errormessage"] = ex.Message;
				controller.Flash[prefix] = instance;

				controller.Redirect(controller.Name, "edit", controller.Query);
			}
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

		public void Execute(Controller controller)
		{
			int id = Convert.ToInt32(controller.Query["id"]);

			controller.PropertyBag[prefix] = 
				ActiveRecordMediator.FindByPrimaryKey(arType, id);

			controller.RenderView("confirmdelete");
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

		public void Execute(Controller controller)
		{
			object instance = null;

			try
			{
				int id = Convert.ToInt32(controller.Form[prefix + ".id"]);
				
				instance = ActiveRecordMediator.FindByPrimaryKey(arType, id);

				ActiveRecordMediator.Delete(instance);

				controller.Redirect(controller.Name, "list");
			}
			catch (Exception ex)
			{
				controller.Flash["errormessage"] = ex.Message;
				controller.Flash[prefix] = instance;

				controller.Redirect(controller.Name, "confirmdelete", controller.Query);
			}
		}
	}
}