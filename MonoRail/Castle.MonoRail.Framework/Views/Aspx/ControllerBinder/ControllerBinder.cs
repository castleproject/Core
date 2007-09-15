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

#if NET

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Web.UI;
	using System.Web.UI.Design;


	/// <summary>
	/// Pendent
	/// </summary>
	[ProvideProperty("ControllerBinding", typeof(Control))]
	[NonVisualControl, Designer(typeof(Design.ControllerActionBinderDesigner))]
	[ParseChildren(true, "ControllerBindings"), PersistChildren(false)]
	public class ControllerBinder : Control, IExtenderProvider, IControllerBinder, ISupportInitialize
	{
		#region Fields

		private bool initialized;
		private bool actionDispatched;
		private PropertyDescriptor bindingsDescriptor;
		private readonly ControllerBindingCollection bindings;
		private ActionArgumentCollection actionArguments;
		private readonly EventHandlerFactory eventHandlerFactory;

		private static readonly object BeforeActionEvent = new object();
		private static readonly object AfterActionEvent = new object();
		private static readonly object ActionErrorEvent = new object();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerBinder"/> class.
		/// </summary>
		public ControllerBinder()
		{
			eventHandlerFactory = new EventHandlerFactory();
			bindings = new ControllerBindingCollection(this);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether a control is being used on a design surface.
		/// </summary>
		/// <value></value>
		/// <returns>true if the control is being used in a designer; otherwise, false.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool DesignMode
		{
			get { return Design.DesignUtil.IsInDesignMode; }
		}

		/// <summary>
		/// Gets the controller bindings.
		/// </summary>
		/// <value>The controller bindings.</value>
		[Category("Behavior")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public ControllerBindingCollection ControllerBindings
		{
			get { return bindings; }
		}

		/// <summary>
		/// Gets the action arguments.
		/// </summary>
		/// <value>The action arguments.</value>
		[Category("Behavior")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Description("Global arguments passed to all controller actions.")]
		public ActionArgumentCollection ActionArguments
		{
			get
			{
				if (actionArguments == null)
				{
					actionArguments = new ActionArgumentCollection();
				}

				return actionArguments;
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when [before action].
		/// </summary>
		[Description("Fired before the Controller action is executed.")]
		public event ActionBindingDelegate BeforeAction
		{
			add { Events.AddHandler(BeforeActionEvent, value); }
			remove { Events.RemoveHandler(BeforeActionEvent, value); }
		}

		/// <summary>
		/// Occurs when [after action].
		/// </summary>
		[Description("Fired after the Controller action is executed.")]
		public event ActionBindingDelegate AfterAction
		{
			add { Events.AddHandler(AfterActionEvent, value); }
			remove { Events.RemoveHandler(AfterActionEvent, value); }
		}

		/// <summary>
		/// Occurs when [action error].
		/// </summary>
		[Description("Fired if the Controller action raised an exception.")]
		public event ActionBindingDelegate ActionError
		{
			add { Events.AddHandler(ActionErrorEvent, value); }
			remove { Events.RemoveHandler(ActionErrorEvent, value); }
		}

		/// <summary>
		/// Called when [before action].
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="actionArgs">The action args.</param>
		/// <returns></returns>
		protected virtual bool OnBeforeAction(ActionBinding action, IDictionary actionArgs)
		{
			ActionBindingDelegate eventDelegate =
				(ActionBindingDelegate) Events[BeforeActionEvent];
			return (eventDelegate != null) ? eventDelegate(action, actionArgs) : true;
		}

		/// <summary>
		/// Called when [after action].
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="actionArgs">The action args.</param>
		protected virtual void OnAfterAction(ActionBinding action, IDictionary actionArgs)
		{
			ActionBindingDelegate eventDelegate =
				(ActionBindingDelegate) Events[AfterActionEvent];
			if (eventDelegate != null) eventDelegate(action, actionArgs);
		}

		/// <summary>
		/// Called when [action error].
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="ex">The ex.</param>
		/// <returns></returns>
		protected virtual bool OnActionError(ActionBinding action, Exception ex)
		{
			ActionBindingErrorDelegate eventDelegate =
				(ActionBindingErrorDelegate) Events[ActionErrorEvent];
			return (eventDelegate != null) ? eventDelegate(action, ex) : false;
		}

		#endregion

		#region IExtenderProvider

		/// <summary>
		/// Specifies whether this object can provide its extender properties to the specified object.
		/// </summary>
		/// <param name="extendee">The <see cref="T:System.Object"></see> to receive the extender properties.</param>
		/// <returns>
		/// true if this object can provide extender properties to the specified object; otherwise, false.
		/// </returns>
		bool IExtenderProvider.CanExtend(object extendee)
		{
			return IsBindableControl(extendee as Control);
		}

		/// <summary>
		/// Gets the controller binding.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <returns></returns>
		public ControllerBindingProperty GetControllerBinding(Control control)
		{
			ControllerBinding binding = null;

			foreach(ControllerBinding candidate in ControllerBindings)
			{
				if (candidate.ControlID == control.ID)
				{
					candidate.Binder = this;
					candidate.ControlInstance = control;
					binding = candidate;
				}
			}

			if (binding == null)
			{
				binding = AddBinding(control);
			}

			return new ControllerBindingProperty(binding);
		}

		/// <summary>
		/// This is never fired in ASP.NET runtime code
		/// </summary>
		/// <param name="extendee"></param>
		/// <param name="value"></param>
		public void SetControllerBinding(object extendee, object value)
		{
			//ControllerBindingProperty binding = (ControllerBinding) value;
			//Control control = (Control) extendee;
		}

		#endregion

		#region IControllerBinder

		/// <summary>
		/// Determines whether [is bindable control] [the specified control].
		/// </summary>
		/// <param name="control">The control.</param>
		/// <returns>
		/// 	<c>true</c> if [is bindable control] [the specified control]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsBindableControl(Control control)
		{
			if (control is ControllerBinder) return false;

			return (control != null && IsVisualControl(control) &&
			        (!DesignMode || EventUtil.HasCompatibleEvents(control)));
		}

		/// <summary>
		/// Adds the binding.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <returns></returns>
		public ControllerBinding AddBinding(Control control)
		{
			ControllerBinding newBinding = new ControllerBinding(this);
			newBinding.ControlInstance = control;
			ControllerBindings.Add(newBinding);

			return newBinding;
		}

		/// <summary>
		/// Finds the control with ID.
		/// </summary>
		/// <param name="controlID">The control ID.</param>
		/// <returns></returns>
		public Control FindControlWithID(string controlID)
		{
			if (DesignMode)
			{
				return FindControlAtDesignTime(controlID);	
			}
			else
			{
				return FindControlAtRunTime(controlID);
			}
		}

		/// <summary>
		/// Gets the controller actions.
		/// </summary>
		/// <returns></returns>
		public string[] GetControllerActions()
		{
			return new string[0];
		}

		/// <summary>
		/// Determines whether [is visual control] [the specified control].
		/// </summary>
		/// <param name="control">The control.</param>
		/// <returns>
		/// 	<c>true</c> if [is visual control] [the specified control]; otherwise, <c>false</c>.
		/// </returns>
		private bool IsVisualControl(Control control)
		{
			if (!(control is IDataSource))
			{
				return !TypeDescriptor.GetAttributes(control).Contains(
					NonVisualControlAttribute.NonVisual);
			}
			return false;
		}

		/// <summary>
		/// Finds the control at design time.
		/// </summary>
		/// <param name="controlID">The control ID.</param>
		/// <returns></returns>
		private Control FindControlAtDesignTime(string controlID)
		{
			IContainer container = GetContainer();

			if (container != null)
			{
				foreach(IComponent component in container.Components)
				{
					Control control = component as Control;

					if ((control != null) && (control.ID == controlID))
					{
						return control;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Finds the control at run time.
		/// </summary>
		/// <param name="controlID">The control ID.</param>
		/// <returns></returns>
		private Control FindControlAtRunTime(string controlID)
		{
			return WebFormUtils.FindControlRecursive(Page, controlID);
		}

		#endregion

		#region Designer Support

		void ISupportInitialize.BeginInit()
		{
		}

		void ISupportInitialize.EndInit()
		{
			if (!initialized && DesignMode)
			{
				RegisterBinderServices();
				FillMissingControlInstances();

				IComponentChangeService changes = GetService<IComponentChangeService>(this);

				if (changes != null)
				{
					changes.ComponentChanged += OnComponentChanged;
					changes.ComponentRemoved += OnComponentRemoved;
				}

				initialized = true;
			}
		}

		private void RegisterBinderServices()
		{
			IServiceContainer services = GetService<IServiceContainer>(this);

			if (services != null)
			{
				services.AddService(typeof(IControllerBinder), this);
			}
		}

		private void UnregisterBinderServices()
		{
			IServiceContainer services = GetService<IServiceContainer>(this);

			if (services != null)
			{
				services.RemoveService(typeof(IControllerBinder));
			}
		}

		private void FillMissingControlInstances()
		{
			foreach (ControllerBinding binding in ControllerBindings)
			{
				if (binding.ControlInstance == null && !string.IsNullOrEmpty(binding.ControlID))
				{
					binding.ControlInstance = FindControlWithID(binding.ControlID);
				}
			}
		}

		private void OnComponentChanged(object source, ComponentChangedEventArgs e)
		{
			if (IsBinderComopnent(e.Component))
			{
				NotifyDesigner();
			}
		}

		private void OnComponentRemoved(object source, ComponentEventArgs e)
		{
			// This is the only way I was able to consistently unsubscribe 
			// from the component change events.

			if (e.Component == this)
			{
				UnregisterBinderServices();

				IComponentChangeService changes = GetService<IComponentChangeService>(this);

				changes.ComponentChanged -= OnComponentChanged;
				changes.ComponentRemoved -= OnComponentRemoved;
			}
			else
			{
				Control control = e.Component as Control;

				if (control != null)
				{
					IDesignerHost designerHost = GetService<IDesignerHost>(control);

					// The Loading flag is checked on the designer since all 
					// cotnrols are removed and then added back when switching
					// between source and design mode.

					if (designerHost != null && !designerHost.Loading)
					{
						if (ControllerBindings.Remove(control))
						{
							NotifyDesigner();
						}
					}
				}
			}
		}

		private T GetService<T>(IComponent context)
		{
			if (context.Site != null)
			{
				return (T)Site.GetService(typeof(T));
			}

			return default(T);
		}

		private IContainer GetContainer()
		{
			IContainer container = null;

			IDesignerHost host = GetService<IDesignerHost>(this);

			if (host != null)
			{
				container = host.Container;

				if (container == null && Site != null)
				{
					container = Site.Container;
				}
			}

			return container;
		}

		/// <summary>
		/// Notifies the designer every time there is a change to
		/// the child properties.  This is a workaround for the Web
		/// Designer not issuing SetControllerBinding invocations.
		/// </summary>
		private void NotifyDesigner()
		{
			if (DesignMode && Site != null)
			{
				if (bindingsDescriptor == null)
				{
					try
					{
						bindingsDescriptor = TypeDescriptor.GetProperties(this)["ControllerBindings"];
					}
					catch
					{
						return;
					}
				}

				IDesignerHost host = GetService<IDesignerHost>(this);

				if (host != null)
				{
					ControlDesigner designer = host.GetDesigner(this) as ControlDesigner;

					if (designer != null)
					{
						ComponentChangedEventArgs changeEvent =
							new ComponentChangedEventArgs(this, bindingsDescriptor,
							                              null, ControllerBindings);

						designer.OnComponentChanged(this, changeEvent);
					}
				}
			}
		}

		private bool IsBinderComopnent(object component)
		{
			return component.GetType().Namespace == GetType().Namespace;
		}

		#endregion

		#region Control Runtime

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (Page.IsPostBack)
			{
				MapEventToControllerAction();
			}
		}

		private void MapEventToControllerAction()
		{
			foreach(ControllerBinding binding in ControllerBindings)
			{
				if (binding.IsValid() && binding.ActionBindings.Count > 0)
				{
					Control control = FindControlAtRunTime(binding.ControlID);

					if (IsBindableControl(control))
					{
						ApplyControlActionBindings(control, binding);
					}
				}
			}
		}

		private void ApplyControlActionBindings(Control control, ControllerBinding binding)
		{
			EventDescriptorCollection events = EventUtil.GetCompatibleEvents(control);

			foreach(ActionBinding action in binding.ActionBindings)
			{
				EventDescriptor eventToBind = events.Find(action.EventName, false);

				if (eventToBind != null)
				{
					BindingContext context = new BindingContext(this, action);
					SubscribeToEvent(control, eventToBind, context);
				}
			}
		}

		private void SubscribeToEvent(Control control, EventDescriptor eventToWire,
		                              BindingContext context)
		{
			Type eventHandlerType;

			if (EventUtil.IsCommandEvent(eventToWire))
			{
				eventHandlerType = typeof(CommandEventHandlerDelegate<>);
			}
			else
			{
				eventHandlerType = typeof(EventHandlerDelegate<>);
			}

			Delegate eventHandler = eventHandlerFactory.CreateActionDelegate(
				eventHandlerType, eventToWire, context);

			eventToWire.AddEventHandler(control, eventHandler);
		}

		internal void DispatchAction(string actionName, BindingContext context)
		{
			if (actionDispatched) return;

			IDictionary actionArgs = context.ResolveActionArguments();

			if (OnBeforeAction(context.Action, actionArgs))
			{
				try
				{
					ContinueAction(actionName, actionArgs);

					OnAfterAction(context.Action, actionArgs);
				}
				catch (Exception ex)
				{
					if (!OnActionError(context.Action, ex))
					{
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Continues the action.
		/// </summary>
		/// <param name="actionName">Name of the action.</param>
		/// <param name="actionArgs">The action args.</param>
		public void ContinueAction(string actionName, IDictionary actionArgs)
		{
			if (actionDispatched)
			{
				throw new InvalidOperationException("An action has already been dispatched");
			}

			if (string.IsNullOrEmpty(actionName))
			{
				throw new ArgumentException("actionName is null or empty");
			}

			Controller controller = GetCurrentController();

			if (controller != null)
			{
				actionDispatched = true;
				controller.Send(actionName, actionArgs);
			}
			else
			{
				throw new InvalidOperationException(
					"A Controller is not present for the current context");
			}
		}

		#region IBindingScope

		object IBindingScope.ResolveSymbol(string symbol)
		{
			return WebFormUtils.GetFieldOrProperty(Page, symbol);
		}

		void IBindingScope.AddActionArguments(BindingContext context,
		                                      IDictionary resolvedActionArgs)
		{
			context.ResolveActionArguments(ActionArguments, resolvedActionArgs);
		}

		#endregion

		private Controller GetCurrentController()
		{
			IControllerLifecycleExecutor executor = (IControllerLifecycleExecutor)
			                                        MonoRailHttpHandler.CurrentContext.UnderlyingContext.Items[
			                                        	ControllerLifecycleExecutor.ExecutorEntry];

			return (executor != null) ? executor.Controller : null;
		}

		#endregion
	}

	#region ControllerBindingProperty

	/// <summary>
	/// Pendent
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ControllerBindingProperty
	{
		private ControllerBinding binding;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerBindingProperty"/> class.
		/// </summary>
		/// <param name="binding">The binding.</param>
		internal ControllerBindingProperty(ControllerBinding binding)
		{
			this.binding = binding;
		}

		/// <summary>
		/// Gets the action bindings.
		/// </summary>
		/// <value>The action bindings.</value>
		[Category("Behavior")]
		public ActionBindingCollection ActionBindings
		{
			get { return binding.ActionBindings; }
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			return binding.ToString();
		}
	}

	#endregion

	#region Event Delegates

	/// <summary>
	/// Pendent
	/// </summary>
	public delegate bool ActionBindingDelegate(ActionBinding action, IDictionary actionArgs);

	/// <summary>
	/// Pendent
	/// </summary>
	public delegate bool ActionBindingErrorDelegate(ActionBinding action, Exception ex);

	#endregion
}

#endif
