namespace Castle.Components.DictionaryAdapter
{
	public interface IValueInitializer
	{
		void Initialize(IDictionaryAdapter dictionaryAdapter, object value);
	}
}
