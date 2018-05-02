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

namespace Castle.DynamicProxy.Tests.GenClasses
{
	using System;

	public class GenClassWithGenMethods<T> where T : new()
	{
		private object savedParam;
		private bool invoked;

		public object SavedParam
		{
			get { return savedParam; }
		}

		public bool Invoked
		{
			get { return invoked; }
		}

		public virtual T DoSomething<Z>(Z z)
		{
			invoked = true;

			savedParam = z;

			return new T();
		}

		public virtual void DoSomethingElse<T2>(TestConverter<int, T2> converter, int value)
		{
			invoked = true;

			savedParam = converter(value);
		}
	}

	public delegate TOutput TestConverter<TInput, TOutput>(TInput input);
}