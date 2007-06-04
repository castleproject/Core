namespace Castle.MonoRail.ActiveRecordSupport.Tests
{
	using Castle.MonoRail.TestSupport;
	using WatiN.Core;

	public abstract class BaseAcceptanceTestCase : BaseARTestCase
	{
		protected IE ie;

		public override void InitFixture()
		{
			base.InitFixture();

			WebServer.StartWebServer();

			IE.Settings.WaitUntilExistsTimeOut = 1;
			ie = new IE();
		}

		public override void TerminateFixture()
		{
			base.TerminateFixture();

			WebServer.StopWebServer();
			ie.Close();
		}
	}
}
