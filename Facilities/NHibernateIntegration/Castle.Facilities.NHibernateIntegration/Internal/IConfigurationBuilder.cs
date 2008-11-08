namespace Castle.Facilities.NHibernateIntegration.Internal
{
	using Castle.Core.Configuration;
	using NHibernate.Cfg;

	/// <summary>
	/// Builds up the Configuration object
	/// </summary>
	public interface IConfigurationBuilder
	{
		/// <summary>
		/// Builds the Configuration object from the specifed configuration
		/// </summary>
		Configuration GetConfiguration(IConfiguration config);
	}
}
