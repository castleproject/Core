// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.Core.Internal;

	/// <summary>
	/// Collection of <see cref="InterceptorReference"/>
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class InterceptorReferenceCollection : ICollection<InterceptorReference>
	{
		private readonly LinkedList list = new LinkedList();

		/// <summary>
		/// Adds the specified interceptor.
		/// </summary>
		/// <param name="interceptor">The interceptor.</param>
		public void Add(InterceptorReference interceptor)
		{
			list.Add(interceptor);
		}

		public void Clear()
		{
			list.Clear();
		}

		public bool Contains(InterceptorReference item)
		{
			return list.Contains(item);
		}

		public void CopyTo(InterceptorReference[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(InterceptorReference item)
		{
			list.Remove(item);
			return true;
		}

		/// <summary>
		/// Adds the the specified interceptor as the first.
		/// </summary>
		/// <param name="interceptor">The interceptor.</param>
		public void AddFirst(InterceptorReference interceptor)
		{
			list.AddFirst(interceptor);
		}

		/// <summary>
		/// Adds the the specified interceptor as the last.
		/// </summary>
		/// <param name="interceptor">The interceptor.</param>
		public void AddLast(InterceptorReference interceptor)
		{
			list.AddLast(interceptor);
		}

		/// <summary>
		/// Inserts the specified interceptor at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="interceptor">The interceptor.</param>
		public void Insert(int index, InterceptorReference interceptor)
		{
			list.Insert(index, interceptor);
		}

		/// <summary>
		/// Gets a value indicating whether this instance has interceptors.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has interceptors; otherwise, <c>false</c>.
		/// </value>
		public bool HasInterceptors
		{
			get { return Count != 0; }
		}

		/// <summary>
		/// When implemented by a class, copies the elements of
		/// the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="array"/> is <see langword="null"/>.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// 	<para>
		/// 		<paramref name="array"/> is multidimensional.</para>
		/// 	<para>-or-</para>
		/// 	<para>
		/// 		<paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.</para>
		/// 	<para>-or-</para>
		/// 	<para>The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</para>
		/// </exception>
		/// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the number of
		/// elements contained in the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <value></value>
		public int Count
		{
			get { return list.Count; }
		}

		public bool IsReadOnly
		{
			get { return list.IsReadOnly; }
		}

		/// <summary>
		/// Gets an object that
		/// can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <value></value>
		public object SyncRoot
		{
			get { return list; }
		}

		/// <summary>
		/// Gets a value
		/// indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized
		/// (thread-safe).
		/// </summary>
		/// <value></value>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// Returns an enumerator that can iterate through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/>
		/// that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		/// <summary>
		/// Adds the interceptor to the end of the interceptors list if it does not exist already.
		/// </summary>
		/// <param name="interceptorReference">The interceptor reference.</param>
		public void AddIfNotInCollection(InterceptorReference interceptorReference)
		{
			if (list.Contains(interceptorReference) == false)
				list.AddLast(interceptorReference);
		}

		IEnumerator<InterceptorReference> IEnumerable<InterceptorReference>.GetEnumerator()
		{
			return (IEnumerator<InterceptorReference>) list.GetEnumerator();
		}
	}
}