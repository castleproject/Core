// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

	public class GenInterfaceHierarchy<T> : IGenInterfaceHierarchySpecialization<T>
	{
		private List<T> items = new List<T>();

		public int Count()
		{
			return items.Count;
		}

		public T[] FetchAll()
		{
			T[] copy = new T[Count()];
			items.CopyTo(copy);
			return copy;
		}

		public void Add()
		{
		}

		public void Add(T item)
		{
			items.Add(item);
		}

		public T Get()
		{
			return items[0];
		}

		public T[] GetAll()
		{
			return FetchAll();
		}
	}

	public interface IGenInterfaceHierarchySpecialization<T> : IGenInterfaceHierarchyBase<T>
	{
		int Count();

		T[] FetchAll();
	}

	public interface IGenInterfaceHierarchyBase<T>
	{
		void Add();

		void Add(T item);

		T Get();

		T[] GetAll();
	}
}