namespace !NAMESPACE!.Core.Infraestructure
{
	public interface IRepository<T> where T : IIdentifiable
	{
		T FetchById(int id);

		T[] FetchAll();

		void Update(T item);

		void Create(T item);

		void Delete(T item);
	}
}
