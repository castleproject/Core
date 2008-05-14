namespace Castle.Facilities.NHibernateIntegration.Tests.Common
{
	using Iesi.Collections;
	using NHibernate.Event;

	public class CustomDeleteListener : NHibernate.Event.IDeleteEventListener
	{
		public void OnDelete(DeleteEvent @event)
		{
		}

		public void OnDelete(DeleteEvent @event, ISet transientEntities)
		{
		}
	}
}
