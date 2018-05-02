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

namespace Castle.DynamicProxy.Tests.Interfaces
{
	using System;

	public interface IGenericWithRefOut
	{
		void Do<T>(out T i);
		void Did<T>(ref T i);
	}

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class GenericWithRefOut : IGenericWithRefOut
	{
		public void Do<T>(out T i)
		{
			i = default(T);
		}

		public void Did<T>(ref T i)
		{
			i = default(T);
		}
	}
}