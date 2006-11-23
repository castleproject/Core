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

namespace Castle.DynamicProxy.Tests.GenInterfaces
{
	using System;

	public class GenInterfaceWithGenArray<T> : IGenInterfaceWithGenArray<T>
		where T : struct
	{
		private T[] innerItems;
		
		public GenInterfaceWithGenArray()
		{
			innerItems = new T[] { new T(), new T(), new T() };
		}

		public void CopyTo(T[] items)
		{
			Array.Copy(innerItems, items, Math.Min(items.Length, innerItems.Length));
		}

		public T[] CreateItems()
		{
			return innerItems;
		}
	}
	
	public interface IGenInterfaceWithGenArray<T>
		where T : struct
	{
		void CopyTo(T[] items);
		
		T[] CreateItems();
	}
}
