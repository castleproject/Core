// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail.Tests
{
	using System.Collections;

	using NUnit.Framework;

	[TestFixture]
	public class ExpandDucktype_WorkaroundForDuplicateVirtualMethodsTestCase
	{
		public class TestObject
		{
			private object prop;

			public object Echo(object obj)
			{
				return obj;
			}

			public object Prop
			{
				get { return prop; }
				set { prop = value; }
			}
		}

		//Just did a new class here to make the test failure message nicer.
		internal class IgnoreNullForTest : IgnoreNull
		{
			public IgnoreNullForTest(object target) : base(target) {}

			public override string ToString()
			{
				return "Ignore Null";
			}
		}

		private TestObject _testObject;
		private object expected;
		private IgnoreNull ignored;

		[SetUp]
		public void Setup()
		{
			_testObject = new TestObject();
			expected = new object();
			ignored = new IgnoreNullForTest(expected);
		}


		[Test]
		public void InvokeMethod_ShouldPassIgnoreNullTarget_InsteadOfIgnoreNull_ToMethodBeingInvoked()
		{
			object actual =
				ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods.Invoke(_testObject, "Echo", new object[] {ignored});
			Assert.AreSame(expected, actual);
		}

		[Test]
		public void SetProperty_ShouldPassIgnoreNullTarget_InsteadOfIgnoreNull_ToPropertyBeingSet()
		{
			object actual =
				ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods.SetProperty(_testObject, "Prop", ignored);
			Assert.AreSame(expected, _testObject.Prop);
		}

		//TODO: SetSlice? I'm not sure what an object would look like that would require this, so I've intentionally not implemented functionality to replace the IgnoreNulls in this method.
		[Test]
		public void SetSlice_ShouldPassIgnoreNullTarget_InsteadOfIgnoreNull_ToSlice()
		{
			Hashtable hash = new Hashtable();
			ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods.SetSlice(hash, "", new object[] {"test", ignored});
			Assert.AreSame(expected, hash["test"]);
		}
	}
}