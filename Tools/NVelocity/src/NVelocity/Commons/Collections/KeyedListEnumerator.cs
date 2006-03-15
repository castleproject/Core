namespace Commons.Collections
{
	using System;
	using System.Collections;

	public class KeyedListEnumerator : IDictionaryEnumerator
	{
		private int index = -1;
		private ArrayList objs;

		internal KeyedListEnumerator(ArrayList list)
		{
			objs = list;
		}

		public bool MoveNext()
		{
			index++;

			if (index >= objs.Count)
				return false;

			return true;
		}

		public void Reset()
		{
			index = -1;
		}

		public object Current
		{
			get
			{
				if (index < 0 || index >= objs.Count)
					throw new InvalidOperationException();

				return objs[index];
			}
		}

		public DictionaryEntry Entry
		{
			get { return (DictionaryEntry) Current; }
		}

		public object Key
		{
			get { return Entry.Key; }
		}

		public object Value
		{
			get { return Entry.Value; }
		}
	}
}