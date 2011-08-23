// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;

	public abstract class Iterator<T> : IEnumerable<T>, IEnumerator<T>, ILazy<T>
	{
		[DebuggerStepThrough]
		protected Iterator() { }

		public abstract bool MoveNext();
		public abstract bool HasCurrent { get; }
		public abstract T Current { get; }

		public virtual bool IsMutable
		{
			get { return false; }
		}

		public virtual T Create()
		{
			throw Error.IteratorNotMutable();
		}

		public virtual T Remove()
		{
			throw Error.IteratorNotMutable();
		}

		public virtual T Require()
		{
			if (!HasCurrent)
				Create();

			return Current;
		}

		bool ILazy<T>.HasValue
		{
			get { return HasCurrent; }
		}

		T ILazy<T>.Value
		{
			get { return Require(); }
		}

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
			throw Error.NotSupported();
		}

		[DebuggerStepThrough]
		void IDisposable.Dispose()
		{
			// Do nothing
		}

		[DebuggerStepThrough]
		protected static T OnNoCurrent()
		{
			throw Error.NoCurrentItem();
		}
	}
}
#endif
