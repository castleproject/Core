namespace Castle.MonoRail
{
	public interface IController
	{
		void SetInitialState(string area, string controller, string action);
	}
}
