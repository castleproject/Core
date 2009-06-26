// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.NHibernateIntegration.SessionStores
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Runtime.CompilerServices;
	using System.Runtime.Remoting.Messaging;

	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractDictStackSessionStore : AbstractSessionStore
	{
		/// <summary>
		/// Name used for storage in <see cref="CallContext"/>
		/// </summary>
		protected String SlotKey
		{
			get
			{
				if (string.IsNullOrEmpty(this.slotKey))
					this.slotKey = string.Format("nh.facility.stacks.{0}", Guid.NewGuid());
				return this.slotKey;
			}
		}
		private string slotKey;

		/// <summary>
		/// Gets the stack of <see cref="SessionDelegate"/> objects for the specified <paramref name="alias"/>.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected override Stack GetStackFor(String alias)
		{
			if (alias == null) throw new ArgumentNullException("alias");

			IDictionary alias2Stack = this.GetDictionary();

			if (alias2Stack == null)
			{
				alias2Stack = new HybridDictionary(true);

				this.StoreDictionary(alias2Stack);
			}

			Stack stack = alias2Stack[alias] as Stack;

			if (stack == null)
			{
				stack = Stack.Synchronized( new Stack() );

				alias2Stack[alias] = stack;
			}

			return stack;
		}

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <returns></returns>
		protected abstract IDictionary GetDictionary();

		/// <summary>
		/// Stores the dictionary.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		protected abstract void StoreDictionary(IDictionary dictionary);
	}
}