namespace Castle.MicroKernel.Tests.ClassComponents
{
	public interface ITask
	{
		
	}
	public interface ITask<T>:ITask
	{
		
	}
	public class Task<T> : ITask<T>{}

	

}
