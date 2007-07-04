namespace !NAMESPACE!.Core.Infraestructure
{
	using Castle.ActiveRecord;

	public class BaseARRepository<T> : IRepository<T> where T : class, IIdentifiable
	{
		public T FetchById(int id)
		{
			return ActiveRecordMediator<T>.FindByPrimaryKey(id, true);
		}

		public T[] FetchAll()
		{
			return ActiveRecordMediator<T>.FindAll();
		}

		public void Update(T item)
		{
			ActiveRecordMediator<T>.Update(item);
		}

		public void Create(T item)
		{
			ActiveRecordMediator<T>.Create(item);
		}

		public void Delete(T item)
		{
			ActiveRecordMediator<T>.Delete(item);
		}
	}
}
