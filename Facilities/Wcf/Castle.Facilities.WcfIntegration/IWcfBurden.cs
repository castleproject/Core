namespace Castle.Facilities.WcfIntegration
{
	using Castle.MicroKernel;

	public interface IWcfBurden
	{
		void Add(object instance);

		void Release(IKernel kernel);
	}
}
