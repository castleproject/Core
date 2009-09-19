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

namespace Castle.Components.Scheduler.Tests.UnitTests
{
	using System;
	using NUnit.Framework;
	using Utilities;

	[TestFixture]
	public class JobSpecTest : BaseUnitTest
	{
		private readonly Trigger trigger = PeriodicTrigger.CreateDailyTrigger(DateTime.UtcNow);

		[Test]
		public void ConstructorSetsProperties()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);
			Assert.AreEqual("abc", spec.Name);
			Assert.AreEqual("some job", spec.Description);
			Assert.AreEqual("with this key", spec.JobKey);
			Assert.AreSame(trigger, spec.Trigger);
			Assert.IsNull(spec.JobData);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorThrowsWhenJobNameIsNull()
		{
			new JobSpec(null, "some job", "with this key", trigger);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void ConstructorThrowsWhenJobNameIsEmpty()
		{
			new JobSpec("", "some job", "with this key", trigger);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorThrowsWhenJobDescriptionIsNull()
		{
			new JobSpec("abc", null, "with this key", trigger);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorThrowsWhenJobKeyIsNull()
		{
			new JobSpec("abc", "some job", null, trigger);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ConstructorThrowsWhenTriggerIsNull()
		{
			new JobSpec("abc", "some job", "with this key", null);
		}

		[Test]
		public void Name_GetterAndSetter()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			spec.Name = "new name";
			Assert.AreEqual("new name", spec.Name);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Name_ThrowsIfValueIsNull()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			spec.Name = null;
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void Name_ThrowsIfValueIsEmpty()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			spec.Name = "";
		}

		[Test]
		public void Description_GetterAndSetter()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			spec.Description = "new description";
			Assert.AreEqual("new description", spec.Description);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Description_ThrowsIfValueIsNull()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			spec.Description = null;
		}

		[Test]
		public void JobKey_GetterAndSetter()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			spec.JobKey = "new key";
			Assert.AreEqual("new key", spec.JobKey);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void JobKey_ThrowsIfValueIsNull()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			spec.JobKey = null;
		}

		[Test]
		public void Trigger_GetterAndSetter()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			Trigger newTrigger = PeriodicTrigger.CreateDailyTrigger(DateTime.UtcNow);
			spec.Trigger = newTrigger;
			Assert.AreSame(newTrigger, spec.Trigger);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Trigger_ThrowsIfValueIsNull()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			spec.Trigger = null;
		}

		[Test]
		public void JobData_GetterAndSetter()
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);

			JobData jobData = new JobData();
			spec.JobData = jobData;
			Assert.AreSame(jobData, spec.JobData);
		}

		[TestCase(false, false)]
		[TestCase(true, false)]
		[TestCase(true, true)]
		public void ClonePerformsADeepCopy(bool useGenericClonable, bool jobDataIsNull)
		{
			JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);
			spec.JobData = jobDataIsNull ? null : new JobData();

			JobSpec clone = useGenericClonable
			                	? spec.Clone()
			                	: (JobSpec) ((ICloneable) spec).Clone();

			Assert.AreNotSame(spec, clone);
			Assert.AreNotSame(trigger, clone.Trigger);

			if (! jobDataIsNull)
				Assert.AreNotSame(spec.JobData, clone.JobData);

			JobAssert.AreEqual(spec, clone);
		}
	}
}