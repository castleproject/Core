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

namespace Castle.DynamicProxy.Tests.BugsReported
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class DynProxy88 : BasePEVerifyTestCase
	{
		[Test]
		public void ShouldGenerateTypeWithDuplicatedBaseInterfacesInterfaceProxy()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IBase), new[] { typeof(ISub1), typeof(ISub2) });
		}

		[Test]
		public void ShouldGenerateTypeWithDuplicatedBaseInterfacesClassProxy()
		{
			generator.CreateClassProxy(typeof(Inherited), new[] {typeof(ISub1), typeof(ISub2)}, new IInterceptor[0]);
		}
	}

	public interface IBase
	{
		void Foo();
	}

	public class Inherited : IBase
	{
		public void Foo()
		{
			
		}
	}

	public interface ISub1 : IBase
	{
		void Bar();
	}
	public interface ISub2 : IBase
	{
		void Baz();
	}

	public class MyFoo:Inherited,ISub1,ISub2
	{
		void ISub1.Bar()
		{
			throw new NotImplementedException();
		}

		public virtual void Baz()
		{
			throw new NotImplementedException();
		}

		void IBase.Foo()
		{
			throw new NotImplementedException();
		}
	}
}