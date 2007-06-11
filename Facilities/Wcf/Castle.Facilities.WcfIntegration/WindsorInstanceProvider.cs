using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Castle.Windsor;

namespace Castle.Facilities.WcfIntegration
{
	/// <summary>
	/// Initialize a service using Windsor
	/// </summary>
	public class WindsorInstanceProvider : IInstanceProvider
	{
		private readonly IWindsorContainer container;
		private readonly Type type;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorInstanceProvider"/> class.
		/// </summary>
		public WindsorInstanceProvider(IWindsorContainer container, Type type)
		{
			this.container = container;
			this.type = type;
		}

		///<summary>
		///Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"></see> object.
		///</summary>
		///
		///<returns>
		///A user-defined service object.
		///</returns>
		///
		///<param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext"></see> object.</param>
		public object GetInstance(InstanceContext instanceContext)
		{
			return GetInstance(instanceContext, null);	
		}

		///<summary>
		///Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"></see> object.
		///</summary>
		///
		///<returns>
		///The service object.
		///</returns>
		///
		///<param name="message">The message that triggered the creation of a service object.</param>
		///<param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext"></see> object.</param>
		public object GetInstance(InstanceContext instanceContext, Message message)
		{
			return container.Resolve(type);
		}

		///<summary>
		///Called when an <see cref="T:System.ServiceModel.InstanceContext"></see> object recycles a service object.
		///</summary>
		///
		///<param name="instanceContext">The service's instance context.</param>
		///<param name="instance">The service object to be recycled.</param>
		public void ReleaseInstance(InstanceContext instanceContext, object instance)
		{
			container.Release(instance);
		}
	}
}