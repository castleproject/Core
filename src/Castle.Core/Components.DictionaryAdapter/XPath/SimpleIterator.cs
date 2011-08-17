using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Castle.Components.DictionaryAdapter
{
	//internal static class SimpleIterator
	//{
	//    public static SimpleIterator<T> ForObject<T>(T obj)
	//        where T : class
	//    {
	//        return (null == obj)
	//            ? (SimpleIterator<T>) EmptyIterator<T>.Instance
	//            : (SimpleIterator<T>) new SingleIterator<T>(obj);
	//    }
	//}

	internal abstract class SimpleIterator<T> : IEnumerable<T>, IEnumerator<T>
	{
		[DebuggerStepThrough]
		protected SimpleIterator() { }

		public abstract bool MoveNext();
		public abstract T Current { get; }

		[DebuggerStepThrough]
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this;
		}

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		object IEnumerator.Current
		{
			[DebuggerStepThrough]
			get { return Current; }
		}

		[DebuggerStepThrough]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerStepThrough]
		void IDisposable.Dispose() { }

		[DebuggerStepThrough]
		protected static T OnNoCurrent()
		{
			throw new InvalidOperationException();
		}
	}
}
