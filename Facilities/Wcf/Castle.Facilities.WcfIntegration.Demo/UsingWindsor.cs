using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Castle.Facilities.WcfIntegration.Demo
{
	[ServiceContract()]
	public interface IAmUsingWindsor
	{
		[OperationContract]
		int GetValueFromWindsorConfig();
	}

	public class UsingWindsor : IAmUsingWindsor
	{
		private readonly int number;

		public UsingWindsor(int number)
		{
			this.number = number;
		}

		public int GetValueFromWindsorConfig()
		{
			return number;
		}
	}
}

