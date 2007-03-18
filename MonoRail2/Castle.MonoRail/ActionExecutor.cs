namespace Castle.MonoRail
{
	public enum ActionType
	{
		Method
	}

	public abstract class ActionExecutor
	{
		private readonly string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionExecutor"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		protected ActionExecutor(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public abstract ActionType ActionType { get; }

		public abstract void Execute();
	}
}
