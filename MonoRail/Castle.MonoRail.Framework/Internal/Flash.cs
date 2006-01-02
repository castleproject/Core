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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Runtime.Serialization;
	using System.Collections;

	[Serializable]
	public class Flash : Hashtable
	{
		public static readonly String FlashKey = "flash";

		[NonSerialized] 
		private ArrayList keep = new ArrayList();

		[NonSerialized] 
		private bool hasItemsToKeep;

		[NonSerialized] 
		private bool hasSwept;

		public Flash()
		{
		}

		public Flash(Flash copy)
		{
			if (copy == null) return;

			foreach(DictionaryEntry entry in copy)
			{
				base.Add(entry.Key, entry.Value);
			}
		}

		internal Flash(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Remove any element thats not marked to be kept.
		/// This method is automatically called by the framework after the controller is processed.
		/// </summary>
		public void Sweep()
		{
			if (hasSwept)
			{
				object[] keys = new object[Keys.Count];

				Keys.CopyTo(keys, 0);

				if (keys.Length != 0)
				{
					keep.AddRange(keys);
				}
			}

			if (keep.Count == 0)
			{
				this.Clear();
			}
			else
			{
				object[] keys = new object[Keys.Count];

				Keys.CopyTo(keys, 0);

				for(int i = 0; i < keys.Length; i++)
				{
					if (!keep.Contains(keys[i]))
					{
						Remove(keys[i]);
					}
					else if (!hasItemsToKeep)
					{
						hasItemsToKeep = true;
					}
				}

				keep.Clear();
			}

			hasSwept = true;
		}

		/// <summary>
		/// Keeps the entire flash contents available for the next action
		/// </summary>
		public void Keep()
		{
			keep.Clear();

			foreach(object key in base.Keys)
			{
				keep.Add(key);
			}
		}

		/// <summary>
		/// Keeps the Flash['key'] contents available for the next action
		/// </summary>
		public void Keep(object key)
		{
			if (!keep.Contains(key)) keep.Add(key);
		}

		/// <summary>
		/// Marks the entire flash to be discarded by the end of the current action
		/// </summary>
		public void Discard()
		{
			keep.Clear();
		}

		/// <summary>
		/// Marks Flash[key] to be discarded by the end of the current action
		/// </summary>
		public void Discard(object key)
		{
			keep.Remove(key);
		}

		/// <summary>
		/// Sets a flash that will not be available to the next action, only to the current.
		/// <code>
		///     Flash.Now( key, "Hello current action" )
		/// </code>
		/// <para>
		/// This method enables you to use the flash as a central messaging system in your app.
		/// When you need to pass an object to the next action, you use the standard flash assign (<c>[]=</c>).
		/// When you need to pass an object to the current action, you use <c>Now</c>, and your object will
		/// vanish when the current action is done.
		/// </para>
		/// <para>
		/// Entries set via <c>Now</c> are accessed the same way as standard entries: <c>Flash['my-key']</c>.
		/// </para>
		/// </summary>
		public void Now(object key, object value)
		{
			base[key] = value;
		}

		public override void Add(object key, object value)
		{
			InternalAdd(key, value);
		}

		public override object this[object key]
		{
			get { return base[key]; }
			set { InternalAdd(key, value); }
		}

		internal bool HasItemsToKeep
		{
			get { return hasItemsToKeep; }
		}

		/// <summary>
		/// Making sure we keep any item added 
		/// to the flash directly for at least one more action.
		/// </summary>
		private void InternalAdd(object key, object value)
		{
			keep.Add(key);
			base[key] = value;
		}
	}
}