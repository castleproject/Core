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

namespace Castle.DynamicProxy.Tests.InterClasses
{
	using System;

	public interface IService2
	{
		void DoOperation2();
	}

#if SILVERLIGHT || NETCORE
	public class Service2 : IService2
#else
	public class Service2 : MarshalByRefObject, IService2
#endif
	{
		public void DoOperation2()
		{
		}
	}
}