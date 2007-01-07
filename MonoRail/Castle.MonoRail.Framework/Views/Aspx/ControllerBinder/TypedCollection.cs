// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

#if (DOTNET2 && NET)

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections;
	using System.ComponentModel;

	public class TypedCollection<T> : CollectionBase
		where T : AbstractBindingComponent
	{
		public T this[int index]
		{
			get { return (T) List[index]; }
			set { List[index] = value; }
		}

		public void Add(T member)
		{
			List.Add(member);
		}

		public void AddAt(int index, T member)
		{
			List.Insert(index, member);
		}

		public void Remove(T member)
		{
			List.Remove(member);
		}

		public bool Contains(T member)
		{
			return List.Contains(member);
		}

		protected override void OnValidate(object value)
		{
			base.OnValidate(value);

			if (!(value is T))
			{
				throw new ArgumentException(string.Format(
				                            	"Only {0} instances are supported by this collection.", typeof(T).Name));
			}

			AbstractBindingComponent properties = (AbstractBindingComponent) value;

			// We cannot throw the exception in Design Mode since the 
			// Set<Property> is never called to commit the update and so
			// the collection may contain invalid values.

			if (!properties.IsValid() && !Design.DesignUtil.IsInDesignMode)
			{
				throw new ArgumentException(properties.Error);
			}
		}

		#region Hide Properties for the Designer

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int Capacity
		{
			get { return base.Capacity; }
			set { base.Capacity = value; }
		}

		#endregion
	}
}

#endif
