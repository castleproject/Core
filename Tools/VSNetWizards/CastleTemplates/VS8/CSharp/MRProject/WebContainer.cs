namespace !NAMESPACE!
{
	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	public class WebContainer : WindsorContainer
	{
		public WebContainer() : base(new XmlInterpreter())
		{
			AddFacilities();
			AddComponents();
		}

		private void AddFacilities()
		{
		}

		private void AddComponents()
		{
			
		}
	}
}
