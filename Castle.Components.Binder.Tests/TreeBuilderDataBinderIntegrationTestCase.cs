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
// 
namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections.Specialized;
	using NUnit.Framework;

	[TestFixture]
	public class TreeBuilderDataBinderIntegrationTestCase
	{
		#region Setup/Teardown

		[SetUp]
		public void Init()
		{
			builder = new TreeBuilder();
			binder = new DataBinder();
		}

		#endregion

		private DataBinder binder;
		private TreeBuilder builder;

		[Test]
		public void SimpleEntries()
		{
			var nameValueColl = new NameValueCollection();

			nameValueColl.Add("name", "hammett");
			nameValueColl.Add("age", "27");
			nameValueColl.Add("age", "28");

			CompositeNode root = builder.BuildSourceNode(nameValueColl);

			Assert.AreEqual("hammett", binder.BindParameter(typeof (String), "name", root));
			Assert.AreEqual(new[] {27, 28}, binder.BindParameter(typeof (int[]), "age", root));
		}
	}
}