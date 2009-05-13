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

namespace Castle.Windsor.Tests
{
	using System.Collections.Generic;
	using Castle.MicroKernel;
	using NUnit.Framework;

	[TestFixture]
	public class ContainerProblem
	{
		[Test]
		public void CausesStackOverflow()
		{
			IWindsorContainer container = new WindsorContainer();

			container.AddComponent("child", typeof(IChild), typeof(Child));
			container.AddComponent("parent", typeof(IParent), typeof(Parent));

			// child or parent will cause a stack overflow...?

			// IChild child = (IChild)container["child"];
			// IParent parent = (IParent) container["parent"];
			object component = container["parent"];
		}
	}

	public interface IParent : IList<IChild>
	{
	}

	public interface IChild
	{
	}

	public class Child : IChild
	{
		public Child(IParent parent)
		{
		}
	}

	public class Parent : List<IChild>
	{
		public Parent(IKernel kernel)
		{
		}
	}
}