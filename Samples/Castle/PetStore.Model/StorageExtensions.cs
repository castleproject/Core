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

namespace PetStore.Model
{
	public static class StorageExtensions
	{
		public static void Save<T>(this T t) where T:class, IAggregateRoot
		{
			Storage<T>.Save(t);
		}

		public static void Update<T>(this T t) where T : class, IAggregateRoot
		{
			Storage<T>.Update(t);
		}

		public static void Create<T>(this T t) where T : class, IAggregateRoot
		{
			Storage<T>.Create(t);
		}

		public static void Delete<T>(this T t) where T : class, IAggregateRoot
		{
			Storage<T>.Delete(t);
		}
	}
}