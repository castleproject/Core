using Castle.Facilities.WcfIntegration.Tests.Behaviors;

namespace Castle.Facilities.WcfIntegration.Tests
{
	public class Operations : IOperations
	{
		private readonly int number;

		public Operations(int number)
		{
			this.number = number;
		}

		public int GetValueFromConstructor()
		{
			return number;
		}

		public bool UnitOfWorkIsInitialized()
		{
			return UnitOfWork.initialized;
		}
	}
}