using System.ServiceModel;
using NUnit.Framework;

namespace Castle.Facilities.WcfIntegration.Tests
{
	[ServiceContract]
	public interface IOperations
	{
		[OperationContract]
		int GetValueFromConstructor();
		[OperationContract]
		bool UnitOfWorkIsInitialized();
	}
}