using System.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.Model;
using Castle.Model.Configuration;

namespace Castle.Facilities.ActiveRecord
{
	public class ActiveRecordFacility : AbstractFacility
	{
		protected override void Init()
		{
			if( FacilityConfig == null )
			{
				throw new ConfigurationException( "The ActiveRecordFacility requires and external configuration." );
			}
			IConfiguration factoriesConfig = FacilityConfig.Children["factory"];
			if( factoriesConfig == null )
			{
				throw new ConfigurationException( "You need at least one factory to use the ActiveRecordFacility." );
			}
			ComponentModel model = new ComponentModel( "activerecord", typeof( NHibernateMappingEngine ), typeof( NHibernateMappingEngine ) );
			model.Configuration = factoriesConfig;
			Kernel.AddCustomComponent( model );
		}
	}
}