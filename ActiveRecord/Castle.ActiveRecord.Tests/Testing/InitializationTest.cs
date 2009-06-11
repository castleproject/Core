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

namespace Castle.ActiveRecord.Tests.Testing
{
	using System;
	using System.Linq;

	using NUnit.Framework;

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Testing;
	using Castle.ActiveRecord.Tests.Model;
	using System.Reflection;
	using System.Collections.Generic;


	[TestFixture]
	public class TypeInitializationTest : NUnitInMemoryTest
	{
		public override Type[] GetTypes()
		{
			return new[] { typeof(Blog), typeof(Post) };
		}

		[Test]
		public void TypesAreInitialized()
		{
			Assert.IsTrue(Array.Exists(ActiveRecordStarter.GetRegisteredTypes(),t=>t.Equals(typeof(Blog))));
			Assert.IsTrue(Array.Exists(ActiveRecordStarter.GetRegisteredTypes(),t=>t.Equals(typeof(Post))));
		}

		[Test]
		public void BasicUsageIsPossible()
		{
			new Blog() { Author = "Me", Name = "Titles" }.Save();
			Assert.AreEqual(1, Blog.FindAll().Length);
		}
	}

	[TestFixture]
	public class AssemblyInitializationTest : NUnitInMemoryTest
	{
		public override System.Reflection.Assembly[] GetAssemblies()
		{
			return new []{ Assembly.GetAssembly(typeof(ActiveRecordClass))};
		}

		[Test]
		public void TypeIsInitialized()
		{
			Assert.IsTrue(Array.Exists(ActiveRecordStarter.GetRegisteredTypes(), t => t.Equals(typeof(ActiveRecordClass))));
		}
	}

	[TestFixture]
	public class MultipleBaseInitializationTest : NUnitInMemoryTest
	{
		public override Type[] GetTypes()
		{
			return new[] { typeof(Blog), typeof(Post), typeof(Test2ARBase), typeof(OtherDbBlog), typeof(OtherDbPost) };
		}

		public override Type[] GetAdditionalBaseClasses()
		{
			return new[] { typeof(Test2ARBase) };
		}

		[Test]
		public void TypesAreInitialized()
		{
			Assert.IsTrue(Array.Exists(ActiveRecordStarter.GetRegisteredTypes(), t => t.Equals(typeof(Blog))));
			Assert.IsTrue(Array.Exists(ActiveRecordStarter.GetRegisteredTypes(), t => t.Equals(typeof(OtherDbBlog))));
		}
	}

	[TestFixture]
	public class AdditionalPropertiesInitializationTest : NUnitInMemoryTest
	{
		public override Type[] GetTypes()
		{
			return new[] { typeof(Blog), typeof(Post)};
		}

		public override IDictionary<string, string> GetProperties()
		{
			return new Dictionary<string, string> {
				{"show_sql","true"}
			};
		}

		[Test]
		public void PropertiesAreCarriesOver()
		{
			Blog.FindAll();
			Assert.AreEqual("true", Blog.Holder.GetConfiguration(typeof(ActiveRecordBase)).Properties["show_sql"]);
		}

	}
}
