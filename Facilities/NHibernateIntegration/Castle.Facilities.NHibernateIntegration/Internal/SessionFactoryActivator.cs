using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using NHibernate;
using NHibernate.Cfg;

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	/// <summary>
	/// Postpones the initiation of SessionFactory until Resolve
	/// </summary>
	public class SessionFactoryActivator : DefaultComponentActivator
	{
		/// <summary>
		/// Constructor for SessionFactoryActivator
		/// </summary>
		/// <param name="model"></param>
		/// <param name="kernel"></param>
		/// <param name="onCreation"></param>
		/// <param name="onDestruction"></param>
		public SessionFactoryActivator(ComponentModel model, IKernel kernel,
			ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{

		}

		/// <summary>
		/// Creates the <see cref="ISessionFactory"/> from the configuration
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override object Create(CreationContext context)
		{
			var configuration = Model.ExtendedProperties[Constants.SessionFactoryConfiguration]
			                    as Configuration;
			return configuration.BuildSessionFactory();
		}
	}
}
