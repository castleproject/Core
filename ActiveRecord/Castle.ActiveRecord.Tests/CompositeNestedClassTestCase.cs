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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using Castle.ActiveRecord.Tests.Model.CompositeNested;
	using NUnit.Framework;
	
	
	[TestFixture]
	public class CompositeNestedClassTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CompositeNestedComponents()
		{ 
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(CompositeNestedParent), typeof(NestedDependent),
				typeof(Dependent));
			Recreate();

			CompositeNestedParent.DeleteAll();
			
			CompositeNestedParent compositeNestedParent = new CompositeNestedParent();
			compositeNestedParent.Save();
			
			compositeNestedParent =  CompositeNestedParent.Find(compositeNestedParent.Id);
			
			Assert.AreEqual(1, compositeNestedParent.Id);
			Assert.IsNotNull(compositeNestedParent.Dependents);

			Dependent dependent = new Dependent();
			dependent.NestedDependentProp = new NestedDependent();
			dependent.NestedDependentProp.IntProp = 1;
			dependent.DateProp = new DateTime(2000, 1, 1);

			dependent.NestedDependentProp.InnerNestedDependantProp = new InnerNestedDependant();
			dependent.NestedDependentProp.InnerNestedDependantProp.StringProp = "String property value";

			compositeNestedParent.Dependents.Add(dependent);
			
			compositeNestedParent.Save();

			compositeNestedParent = CompositeNestedParent.Find(compositeNestedParent.Id);

			Assert.AreEqual(1, compositeNestedParent.Dependents.Count);
			dependent = (Dependent) compositeNestedParent.Dependents[0];
			
			Assert.AreEqual(1, dependent.NestedDependentProp.IntProp);
			Assert.AreEqual("String property value", dependent.NestedDependentProp.InnerNestedDependantProp.StringProp);
			

		}
	}
}
