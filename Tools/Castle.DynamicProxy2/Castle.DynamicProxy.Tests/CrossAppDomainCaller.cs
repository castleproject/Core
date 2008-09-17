using System;

namespace Castle.DynamicProxy.Tests
{
	[Serializable]
	public class CrossAppDomainCaller
	{
		public static void RunInOtherAppDomain(Action<object[]> callback, params object[] args)
		{
			CrossAppDomainCaller callbackObject = new CrossAppDomainCaller(callback, args);
			AppDomain newDomain = AppDomain.CreateDomain("otherDomain", AppDomain.CurrentDomain.Evidence,
			                                             AppDomain.CurrentDomain.SetupInformation);
			try
			{
				newDomain.DoCallBack(callbackObject.Run);
			}
			finally
			{
				AppDomain.Unload(newDomain);
			}
		}

		private readonly Action<object[]> callback;
		private readonly object[] args;

		public CrossAppDomainCaller(Action<object[]> callback, object[] args)
		{
			this.callback = callback;
			this.args = args;
		}

		private void Run()
		{
			callback(args);
		}
	}
}