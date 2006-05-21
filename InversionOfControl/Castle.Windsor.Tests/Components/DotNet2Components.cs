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

#if DOTNET2

namespace Castle.Windsor.Tests
{
	using System;
	using System.Collections;

	using Castle.Windsor.Tests.Components;

	public interface IRepository<T>
    {
        T Get(int id);
    }

    public class DemoRepository<T> : IRepository<T>
    {
        private string name;
        private ICache<T> cache;

        public ICache<T> Cache
        {
            get { return cache; }
            set { cache = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public T Get(int id)
        {
            return Activator.CreateInstance<T>();
        }
    }

    public class ReviewerRepository : DemoRepository<IReviewer>
    {
        string name;
        ICache<IReviewer> cache;

        public ICache<IReviewer> Cache
        {
            get { return cache; }
            set { cache = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public IReviewer Get(int id)
        {
            return null;
        }
    }

    public interface ICache<T> 
    {
        void Put(string key, T item);
        T Get(string key);
    }

    public class HashTableCache<T> : ICache<T>
    {
        Hashtable hash = new Hashtable();

        public void Put(string key, T item)
        {
            hash[key] = item;
        }
        public T Get(string key)
        {
            return (T)hash[key];
        }
    }

    public class NullCache<T> : ICache<T>
    {
        public void Put(string key, T item)
        {
        }

        public T Get(string key)
        {
            return default(T);
        }
    }

    
    public class LoggingRepositoryDecorator<T> : IRepository<T>
    {
        public IRepository<T> inner;

        public LoggingRepositoryDecorator()
        { }

        public LoggingRepositoryDecorator(IRepository<T> inner)
        {
            this.inner = inner;
        }

        public T Get(int id)
        {
            Console.WriteLine("Getting {0}", id);
            return inner.Get(id);
        }
    }

	[Castle.Model.Transient]
	public class TransientRepository<T> : IRepository<T> where T: new()
	{
		public T Get(int id)
		{
			return new T();
		}
	}
	
	public class NeedsGenericType
	{
		ICache<string> cache;

		public NeedsGenericType(ICache<string> cache)
		{
			this.cache = cache;
		}
	}
}
#endif