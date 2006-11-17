
namespace Castle.MicroKernel.Tests.ClassComponents
{
	public class ClassWithTwoParametersWithSameType
	{
		private readonly ICommon one;
		private readonly ICommon two;

		public ICommon One
		{
			get { return this.one; }
		}

		public ICommon Two
		{
			get { return this.two; }
		}

		public ClassWithTwoParametersWithSameType(ICommon one, ICommon two)
		{
			this.one = one;
			this.two = two;
		}
	}
}
