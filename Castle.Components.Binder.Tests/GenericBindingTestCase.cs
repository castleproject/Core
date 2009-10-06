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
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	public class GenericBindingTestCase
	{
		private static CompositeNode GetParamsNode(int expectedValue)
		{
			var paramsNode = new CompositeNode("root");
			var listNode = new IndexedNode("myList");
			paramsNode.AddChildNode(listNode);
			listNode.AddChildNode(new LeafNode(typeof (int), "", expectedValue));
			return paramsNode;
		}

		[Test]
		public void CanBindToGenericList()
		{
			int expectedValue = 32;
			var binder = new DataBinder();
			CompositeNode paramsNode = GetParamsNode(expectedValue);
			var myList = (List<int>) binder.BindObject(typeof (List<int>), "myList", paramsNode);

			Assert.AreEqual(expectedValue, myList[0]);
		}

		[Test]
		public void CanBindToGenericListInstance()
		{
			int expectedValue = 12;

			var myList = new List<int>();
			myList.Add(expectedValue);

			var binder = new DataBinder();
			CompositeNode paramsNode = GetParamsNode(expectedValue);

			binder.BindObjectInstance(myList, "myList", paramsNode);
			Assert.AreEqual(expectedValue, myList[0]);
		}
	}
}