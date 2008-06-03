namespace Castle.MonoRail.Views.Brail.TestSite.Components
{
	using Framework;

	public class ComponentUsingCaptureFor : ViewComponent
	{
		public override void Render()
		{
			RenderView("ComponentUsingCaptureFor");
		}
	}
}