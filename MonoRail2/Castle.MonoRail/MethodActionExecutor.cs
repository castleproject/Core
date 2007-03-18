namespace Castle.MonoRail
{
	using System.Reflection;

	public class MethodActionExecutor : ActionExecutor
	{
		private readonly MethodInfo actionMethod;

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodActionExecutor"/> class.
		/// </summary>
		/// <param name="actionMethod">The action method.</param>
		public MethodActionExecutor(MethodInfo actionMethod) : base(actionMethod.Name)
		{
			this.actionMethod = actionMethod;
		}

		public override void Execute()
		{
			
		}

		public override ActionType ActionType
		{
			get { return ActionType.Method; }
		}
	}
}
