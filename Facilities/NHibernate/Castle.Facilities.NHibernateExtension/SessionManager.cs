// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.NHibernateExtension
{
	using System;
	using System.Diagnostics;
	using System.Collections;
	using System.Threading;
	using System.Runtime.CompilerServices;

	using NHibernate;

	/// <summary>
	/// PerThread class to hold ISession instances.
	/// </summary>
	public abstract class SessionManager
	{
		private static LocalDataStoreSlot _slot = Thread.AllocateDataSlot();

		/// <summary>
		/// Returns the current NHibernate's Session
		/// </summary>
		public static ISession CurrentSession
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				return CurrentPair != null ? CurrentPair.Session : null;
			}
		}

		internal static Stack CurStack
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				Stack stack = (Stack) Thread.GetData(_slot);

				if (stack == null)
				{
					stack = new Stack();
					Thread.SetData(_slot, stack);
				}

				return stack;
			}
		}

		internal static Pair CurrentPair
		{
			get
			{
				if (CurStack.Count == 0) return null;
				return (Pair) CurStack.Peek();
			}
		}

		/// <summary>
		/// Used by the internal framwork. Do not explicit call this method.
		/// </summary>
		public static bool IsCurrentSessionCompatible(String id)
		{
			Pair pair = CurrentPair;
			if (pair == null) return false;
			return pair.id.Equals(id);
		}

		/// <summary>
		/// Used by the internal framwork. Do not explicit call this method.
		/// </summary>
		public static void Push(ISession session, String id)
		{
			CurStack.Push( new Pair(session, id) );
		}

		/// <summary>
		/// Used by the internal framwork. Do not explicit call this method.
		/// </summary>
		public static ISession Pop(String id)
		{
			Pair pair = CurStack.Pop() as Pair;
			if (pair == null) return null;
			Debug.Assert( pair.id.Equals(id) );
			return pair.Session;
		}
	}

	internal class Pair
	{
		public readonly ISession Session;
		public readonly String id;

		public Pair(ISession session, string id)
		{
			Session = session;
			this.id = id;
		}
	}
}