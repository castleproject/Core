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

namespace Castle.MonoRail.Framework.Tests.Helpers
{
	using System.Globalization;
	using System.IO;
	using System.Threading;
	using Castle.MonoRail.Framework.Helpers;
	using Core;
	using NUnit.Framework;

	[TestFixture]
	public class FormHelperSetOperation
	{
		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;
		}

		[Test]
		public void EmptyInitialSet()
		{
			OperationState state = SetOperation.IterateOnDataSource(
				null, new int[] {1, 2, 3, 4}, null);

			Assert.IsNotNull(state);
			Assert.IsTrue(state is ListDataSourceState);
			Assert.IsNull(state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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
		public void ApplyingFormat()
		{
			OperationState state = SetOperation.IterateOnDataSource(
				null, new int[] {1, 2, 3, 4}, DictHelper.Create("textformat=C"));

			Assert.IsNotNull(state);
			Assert.IsTrue(state is ListDataSourceState);
			Assert.IsNull(state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
			{
				iterated = true;

				Assert.IsFalse(item.IsSelected);
				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);

				Assert.AreEqual(index.ToString("C"), item.Text);
				Assert.AreEqual(index.ToString(), item.Value);

				index++;
			}

			Assert.IsTrue(iterated);
		}


		[Test]
		public void NullDataSource()
		{
			OperationState state = SetOperation.IterateOnDataSource(
				new int[] {1, 2, 3, 4}, null, null);

			Assert.IsNotNull(state);
			Assert.IsTrue(state is NoIterationState);
			Assert.IsNull(state.TargetSuffix);

			Assert.IsFalse(state.MoveNext());
		}

		[Test]
		public void EmptyDataSource()
		{
			OperationState state = SetOperation.IterateOnDataSource(
				new int[] {1, 2, 3, 4}, new int[0], null);

			Assert.IsNotNull(state);
			Assert.IsTrue(state is NoIterationState);
			Assert.IsNull(state.TargetSuffix);

			Assert.IsFalse(state.MoveNext());
		}

		[Test]
		public void SingleValueInitialSet_EnumPairs()
		{
			OperationState state = SetOperation.IterateOnDataSource(
				FileMode.Append, new Pair<FileMode, string>[]
				                 	{
				                 		new Pair<FileMode, string>(FileMode.Append, "append"),
				                 		new Pair<FileMode, string>(FileMode.Create, "create"),
				                 		new Pair<FileMode, string>(FileMode.CreateNew, "new"),
				                 		new Pair<FileMode, string>(FileMode.Open, "open")
				                 	}, DictHelper.Create("value=First", "text=Second"));

			Assert.IsNotNull(state);
			Assert.IsTrue(state is DifferentTypeOperationState);
			Assert.AreEqual("", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach(SetItem item in state)
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

				index++;
			}

			Assert.IsTrue(iterated);
		}

		[Test]
		public void SingleValueInitialSet_SameTypes()
		{
			OperationState state = SetOperation.IterateOnDataSource(
				1, new int[] {1, 2, 3, 4}, null);

			Assert.IsNotNull(state);
			Assert.IsTrue(state is SameTypeOperationState);
			Assert.AreEqual("", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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
			OperationState state = SetOperation.IterateOnDataSource(
				new int[] {1, 2}, new int[] {1, 2, 3, 4}, null);

			Assert.IsNotNull(state);
			Assert.IsTrue(state is SameTypeOperationState);
			Assert.AreEqual("", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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
			OperationState state = SetOperation.IterateOnDataSource(
				new Role(1, "Admin"), new Role[] {new Role(0, "User"), new Role(1, "Admin")},
				DictHelper.Create("text=Name", "value=Id"));

			Assert.IsNotNull(state);
			Assert.IsTrue(state is SameTypeOperationState);
			Assert.AreEqual("Id", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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
			OperationState state = SetOperation.IterateOnDataSource(
				new Role[] {new Role(0, "User"), new Role(1, "Admin")},
				new Role[] {new Role(0, "User"), new Role(1, "Admin")},
				DictHelper.Create("text=Name", "value=Id"));

			Assert.IsNotNull(state);
			Assert.IsTrue(state is SameTypeOperationState);
			Assert.AreEqual("Id", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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
			OperationState state = SetOperation.IterateOnDataSource(
				new ProductCategory[] {new ProductCategory(0, "User"), new ProductCategory(1, "Admin")},
				new Role[] {new Role(0, "User"), new Role(1, "Admin")},
				DictHelper.Create("text=Name", "value=Id"));

			Assert.IsNotNull(state);
			Assert.IsTrue(state is DifferentTypeOperationState);
			Assert.AreEqual("Id", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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
			OperationState state = SetOperation.IterateOnDataSource(
				new Role2(1, "Admin"),
				new Role[] {new Role(0, "User"), new Role(1, "Admin")},
				DictHelper.Create("text=Name", "value=Id", "sourceproperty=identification"));

			Assert.IsNotNull(state);
			Assert.IsTrue(state is DifferentTypeOperationState);
			Assert.AreEqual("Identification", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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
			OperationState state = SetOperation.IterateOnDataSource(
				new Role2[] {new Role2(0, "User"), new Role2(1, "Admin")},
				new Role[] {new Role(0, "User"), new Role(1, "Admin")},
				DictHelper.Create("text=Name", "value=Id", "sourceproperty=identification"));

			Assert.IsNotNull(state);
			Assert.IsTrue(state is DifferentTypeOperationState);
			Assert.AreEqual("Identification", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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

		/// <summary>
		/// The target is a decimal while the data source is an array of strings
		/// </summary>
		[Test]
		public void SingleSelectionDifferentTypes()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("pt-br");

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;

			OperationState state = SetOperation.IterateOnDataSource(
				(decimal?) 1.5,
				new string[] {"1", "1,5", "2"}, null);
			Assert.IsNotNull(state);
			Assert.IsTrue(state is DifferentTypeOperationState);

			bool iterated = false;
			int index = 0;

			foreach (SetItem item in state)
			{
				iterated = true;

				Assert.IsNotNull(item.Text);
				Assert.IsNotNull(item.Value);

				if (index == 1)
				{
					Assert.IsTrue(item.IsSelected);
					Assert.AreEqual("1,5", item.Text);
					Assert.AreEqual("1,5", item.Value);
				}
				else
				{
					Assert.IsFalse(item.IsSelected);
				}

				index++;
			}

			Assert.IsTrue(iterated);
		}

		[Test]
		public void MultipleValuesInitialSet_DifferentTypes_Primitive_NoNameMatching()
		{
			OperationState state = SetOperation.IterateOnDataSource(
				new int[] {0, 1},
				new Role[] {new Role(0, "User"), new Role(1, "Admin")},
				DictHelper.Create("text=Name", "value=Id"));

			Assert.IsNotNull(state);
			Assert.IsTrue(state is DifferentTypeOperationState);
			Assert.AreEqual("", state.TargetSuffix);

			bool iterated = false;
			int index = 1;

			foreach (SetItem item in state)
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