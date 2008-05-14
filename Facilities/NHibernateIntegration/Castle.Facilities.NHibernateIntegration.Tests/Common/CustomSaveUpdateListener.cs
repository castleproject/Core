namespace Castle.Facilities.NHibernateIntegration.Tests.Common
{
	using NHibernate.Event;

	public class CustomSaveUpdateListener : NHibernate.Event.ISaveOrUpdateEventListener
	{
		public void OnSaveOrUpdate(SaveOrUpdateEvent @event)
		{
		}
	}
}
