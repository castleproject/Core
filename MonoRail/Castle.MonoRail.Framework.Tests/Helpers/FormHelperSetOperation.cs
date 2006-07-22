// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System.Globalization;
	using System.Threading;

	using Castle.MonoRail.Framework.Helpers;
	
	using NUnit.Framework;

	[TestFixture]
	public class FormHelperSetOperation
	{
		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;
		}
		
		[Test]
		public void EmptyInitialSet()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				null, new int[] {1, 2, 3, 4}, null);
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.ListDataSourceState);
			Assert.IsNull(state.TargetSuffix);
			
			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;
				
				Assert.IsFalse(item.IsSelected);
				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);

				Assert.AreEqual(index.ToString(), item.Text);
				Assert.AreEqual(index.ToString(), item.Value);
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}

		[Test]
		public void NullDataSource()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new int[] {1, 2, 3, 4}, null, null);
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.NoIterationState);
			Assert.IsNull(state.TargetSuffix);
			
			bool iterated = false;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;
			}
			
			Assert.IsFalse(iterated);
		}
		
		[Test]
		public void EmptyDataSource()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new int[] {1, 2, 3, 4}, new int[0], null);
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.NoIterationState);
			Assert.IsNull(state.TargetSuffix);
			
			bool iterated = false;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;
			}
			
			Assert.IsFalse(iterated);
		}

		[Test]
		public void SingleValueInitialSet_SameTypes()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				1, new int[] {1, 2, 3, 4}, null);
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.SameTypeOperationState);
			Assert.AreEqual("", state.TargetSuffix);
			
			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;
				
				if (index == 1)
				{
					Assert.IsTrue(item.IsSelected);
				}
				else
				{
					Assert.IsFalse(item.IsSelected);
				}
				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);

				Assert.AreEqual(index.ToString(), item.Text);
				Assert.AreEqual(index.ToString(), item.Value);
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}

		[Test]
		public void MultipleValuesInitialSet_SameTypes()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new int[] {1,2}, new int[] {1, 2, 3, 4}, null);
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.SameTypeOperationState);
			Assert.AreEqual("", state.TargetSuffix);
			
			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;
				
				if (index == 1 || index == 2)
				{
					Assert.IsTrue(item.IsSelected);
				}
				else
				{
					Assert.IsFalse(item.IsSelected);
				}
				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);

				Assert.AreEqual(index.ToString(), item.Text);
				Assert.AreEqual(index.ToString(), item.Value);
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}

		[Test]
		public void SingleValueInitialSet_SameTypes_NonPrimitive()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new Role(1, "Admin"), new Role[] {new Role(0,"User"), new Role(1,"Admin")}, 
				DictHelper.Create("text=Name", "value=Id"));
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.SameTypeOperationState);
			Assert.AreEqual("Id", state.TargetSuffix);
			
			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;

				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);

				if (index == 1)
				{
					Assert.IsFalse(item.IsSelected);
					Assert.AreEqual("User", item.Text);
					Assert.AreEqual("0", item.Value);
				}
				else
				{
					Assert.IsTrue(item.IsSelected);
					Assert.AreEqual("Admin", item.Text);
					Assert.AreEqual("1", item.Value);
				}
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}
		
		[Test]
		public void MultipleValuesInitialSet_SameTypes_NonPrimitive()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new Role[] {new Role(0,"User"), new Role(1,"Admin")}, 
				new Role[] {new Role(0,"User"), new Role(1,"Admin")}, 
				DictHelper.Create("text=Name", "value=Id"));
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.SameTypeOperationState);
			Assert.AreEqual("Id", state.TargetSuffix);
			
			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;

				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);
				Assert.IsTrue(item.IsSelected);

				if (index == 1)
				{
					Assert.AreEqual("User", item.Text);
					Assert.AreEqual("0", item.Value);
				}
				else
				{
					Assert.AreEqual("Admin", item.Text);
					Assert.AreEqual("1", item.Value);
				}
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}

		[Test]
		public void MultipleValuesInitialSet_DifferentTypes_NonPrimitive()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new ProductCategory[] {new ProductCategory(0,"User"), new ProductCategory(1,"Admin")}, 
				new Role[] {new Role(0,"User"), new Role(1,"Admin")}, 
				DictHelper.Create("text=Name", "value=Id"));
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.DifferentTypeOperationState);
			Assert.AreEqual("Id", state.TargetSuffix);
			
			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;

				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);
				Assert.IsTrue(item.IsSelected);

				if (index == 1)
				{
					Assert.AreEqual("User", item.Text);
					Assert.AreEqual("0", item.Value);
				}
				else
				{
					Assert.AreEqual("Admin", item.Text);
					Assert.AreEqual("1", item.Value);
				}
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}

		[Test]
		public void SingleValueInitialSet_DifferentTypes_NonPrimitive_NoNameMatching()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new Role2(1,"Admin"), 
				new Role[] {new Role(0,"User"), new Role(1,"Admin")}, 
				DictHelper.Create("text=Name", "value=Id", "sourceproperty=identification"));
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.DifferentTypeOperationState);
			Assert.AreEqual("Identification", state.TargetSuffix);
			
			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;

				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);

				if (index == 1)
				{
					Assert.IsFalse(item.IsSelected);
					Assert.AreEqual("User", item.Text);
					Assert.AreEqual("0", item.Value);
				}
				else
				{
					Assert.IsTrue(item.IsSelected);
					Assert.AreEqual("Admin", item.Text);
					Assert.AreEqual("1", item.Value);
				}
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}

		[Test]
		public void MultipleValuesInitialSet_DifferentTypes_NonPrimitive_NoNameMatching()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new Role2[] {new Role2(0,"User"), new Role2(1,"Admin")}, 
				new Role[] {new Role(0,"User"), new Role(1,"Admin")}, 
				DictHelper.Create("text=Name", "value=Id", "sourceproperty=identification"));
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.DifferentTypeOperationState);
			Assert.AreEqual("Identification", state.TargetSuffix);

			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;

				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);
				Assert.IsTrue(item.IsSelected);

				if (index == 1)
				{
					Assert.AreEqual("User", item.Text);
					Assert.AreEqual("0", item.Value);
				}
				else
				{
					Assert.AreEqual("Admin", item.Text);
					Assert.AreEqual("1", item.Value);
				}
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}

		[Test]
		public void MultipleValuesInitialSet_DifferentTypes_Primitive_NoNameMatching()
		{
			FormHelper.OperationState state = FormHelper.SetOperation.IterateOnDataSource(
				new int[] {0,1}, 
				new Role[] {new Role(0,"User"), new Role(1,"Admin")}, 
				DictHelper.Create("text=Name", "value=Id"));
			
			Assert.IsNotNull(state);
			Assert.IsTrue(state is FormHelper.DifferentTypeOperationState);
			Assert.AreEqual("", state.TargetSuffix);
			
			bool iterated = false;
			int index = 1;
			
			foreach(FormHelper.SetItem item in state)
			{
				iterated = true;

				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);
				Assert.IsTrue(item.IsSelected);

				if (index == 1)
				{
					Assert.AreEqual("User", item.Text);
					Assert.AreEqual("0", item.Value);
				}
				else
				{
					Assert.AreEqual("Admin", item.Text);
					Assert.AreEqual("1", item.Value);
				}
				
				index++;
			}
			
			Assert.IsTrue(iterated);
		}
	}
}
