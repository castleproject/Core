namespace Castle.MonoRail.Framework.Services
{
	using System.Collections.Generic;
	using System.Reflection;
	using Descriptors;
	using Providers;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultActionSelector : IActionSelector
	{
		private readonly List<ISubActionSelector> subSelectors = new List<ISubActionSelector>();

		#region IActionSelector Members

		/// <summary>
		/// Selects the an action.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <param name="actionType">Type of the action.</param>
		/// <returns></returns>
		public IExecutableAction Select(IEngineContext engineContext, IController controller, IControllerContext context,
		                                ActionType actionType)
		{
			string actionName = context.Action;

			// Look for the target method
			MethodInfo actionMethod = SelectActionMethod(controller, context, context.Action, actionType);

			if (actionMethod == null)
			{
				// If we couldn't find a method for this action, look for a dynamic action
				IDynamicAction dynAction = null;

				if (context.DynamicActions.ContainsKey(actionName))
				{
					dynAction = context.DynamicActions[actionName];
				}

				if (dynAction != null)
				{
					return new DynamicActionExecutor(dynAction);
				}
			}
			else
			{
				ActionMetaDescriptor actionDesc = context.ControllerDescriptor.GetAction(actionMethod);

				return new ActionMethodExecutor(actionMethod, actionDesc ?? new ActionMetaDescriptor());
			}

			IExecutableAction executableAction = RunSubSelectors(engineContext, controller, context, actionType);

			if (executableAction != null)
			{
				return executableAction;
			}

			// Last try:
			return ResolveDefaultMethod(context.ControllerDescriptor, controller, context, actionType);
		}

		/// <summary>
		/// Registers the specified sub selector.
		/// </summary>
		/// <param name="subSelector">The sub selector.</param>
		public void Register(ISubActionSelector subSelector)
		{
			subSelectors.Add(subSelector);
		}

		/// <summary>
		/// Unregisters the specified sub selector.
		/// </summary>
		/// <param name="subSelector">The sub selector.</param>
		public void Unregister(ISubActionSelector subSelector)
		{
			subSelectors.Remove(subSelector);
		}

		#endregion

		/// <summary>
		/// Selects the action method.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <param name="name">The name.</param>
		/// <param name="actionType">The action type</param>
		/// <returns></returns>
		protected virtual MethodInfo SelectActionMethod(IController controller, IControllerContext context, string name,
		                                                ActionType actionType)
		{
			object action = context.ControllerDescriptor.Actions[name];

			if (actionType == ActionType.Sync)
			{
				MethodInfo methodInfo = action as MethodInfo;

				if (methodInfo != null)
				{
					return methodInfo;
				}

				return controller.GetType().GetMethod(name,
				                                      BindingFlags.Public | BindingFlags.Instance |
				                                      BindingFlags.IgnoreCase);
			}

			AsyncActionPair actionPair = (AsyncActionPair) action;
			
			if (actionPair == null)
			{
				return null;
			}
			
			return actionType == ActionType.AsyncBegin ? actionPair.BeginActionInfo : actionPair.EndActionInfo;
		}

		/// <summary>
		/// Runs the sub selectors.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <param name="actionType">Type of the action.</param>
		/// <returns></returns>
		protected virtual IExecutableAction RunSubSelectors(IEngineContext engineContext, IController controller,
		                                                    IControllerContext context, ActionType actionType)
		{
			foreach(ISubActionSelector selector in subSelectors)
			{
				IExecutableAction action = selector.Select(engineContext, controller, context, actionType);

				if (action != null)
				{
					return action;
				}
			}

			return null;
		}

		/// <summary>
		/// The following lines were added to handle _default processing
		/// if present look for and load _default action method
		/// <seealso cref="DefaultActionAttribute"/>
		/// </summary>
		private IExecutableAction ResolveDefaultMethod(ControllerMetaDescriptor controllerDesc, IController controller,
		                                               IControllerContext context, ActionType actionType)
		{
			if (controllerDesc.DefaultAction != null)
			{
				MethodInfo method = SelectActionMethod(
					controller, context,
					controllerDesc.DefaultAction.DefaultAction, actionType);

				if (method != null)
				{
					ActionMetaDescriptor actionDesc = controllerDesc.GetAction(method);

					return new ActionMethodExecutor(method, actionDesc ?? new ActionMetaDescriptor());
				}
			}

			return null;
		}
	}
}