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

	public enum LifecycleStepType
	{
		Commission,
		Decommission
	}

	/// <summary>
	/// Represents a collection of ordered lifecycle steps.
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class LifecycleStepCollection : ICollection
	{
		private static object _syncRoot = new object();
		private List<object> commissionSteps;
		private List<object> decommissionSteps;

		/// <summary>
		/// Initializes a new instance of the <see cref="LifecycleStepCollection"/> class.
		/// </summary>
		public LifecycleStepCollection()
		{
			commissionSteps = new List<object>();
			decommissionSteps = new List<object>();
		}

		/// <summary>
		/// Returns all steps for the commission phase
		/// </summary>
		/// <returns></returns>
		public object[] GetCommissionSteps()
		{
			object[] steps = new object[commissionSteps.Count];
			commissionSteps.CopyTo(steps, 0);
			return steps;
		}

		/// <summary>
		/// Returns all steps for the decommission phase
		/// </summary>
		/// <returns></returns>
		public object[] GetDecommissionSteps()
		{
			object[] steps = new object[decommissionSteps.Count];
			decommissionSteps.CopyTo(steps, 0);
			return steps;
		}

		/// <summary>
		/// Gets a value indicating whether this instance has commission steps.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has commission steps; otherwise, <c>false</c>.
		/// </value>
		public bool HasCommissionSteps
		{
			get { return commissionSteps.Count != 0; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance has decommission steps.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has decommission steps; otherwise, <c>false</c>.
		/// </value>
		public bool HasDecommissionSteps
		{
			get { return decommissionSteps.Count != 0; }
		}

		/// <summary>
		/// Adds a step to the commission or decomission phases.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="stepImplementation"></param>
		public void Add(LifecycleStepType type, object stepImplementation)
		{
			if (stepImplementation == null) throw new ArgumentNullException("stepImplementation");

			if (type == LifecycleStepType.Commission)
			{
				commissionSteps.Add(stepImplementation);
			}
			else
			{
				decommissionSteps.Add(stepImplementation);
			}
		}

		/// <summary>
		/// Copies the elements of
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
			get { return commissionSteps.Count + decommissionSteps.Count; }
		}

		/// <summary>
		/// Gets an object that
		/// can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <value></value>
		public object SyncRoot
		{
			get { return _syncRoot; }
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
			List<object> newList = new List<object>(commissionSteps);
			newList.AddRange(decommissionSteps);
			return newList.GetEnumerator();
		}
	}
}