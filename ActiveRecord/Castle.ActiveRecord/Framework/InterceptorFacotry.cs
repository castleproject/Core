namespace Castle.ActiveRecord.Framework
{
	using NHibernate;

	/// <summary>
	/// Create an interceptor for the session.
	/// Allow to override the default for creating the intercetor
	/// </summary>
	public class InterceptorFacotry
	{
		/// <summary>
		/// Creates an instance of the interceptor
		/// </summary>
		public delegate IInterceptor CreateInterceptor();

		/// <summary>
		/// Create the 
		/// </summary>
		public static CreateInterceptor Create = new CreateInterceptor(DefaultCreateInterceptor);

		private static IInterceptor DefaultCreateInterceptor()
		{
			return HookDispatcher.Instance;
		}
	}
}
