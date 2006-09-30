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

#if DOTNET2

namespace Castle.ActiveRecord.Tests
{
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model.GenericModel;


	[TestFixture]
	public class NullablesGenericsTestCase : AbstractActiveRecordTest
	{
		[SetUp]
		public new void Init()
		{
			ActiveRecordStarter.ResetInitializationFlag();

			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(GenericNullableModel), typeof(SurveyAssociation));
	
			Recreate();
		}

		[Test]
		public void Usage()
		{
            GenericNullableModel model = new GenericNullableModel();
			model.Save();

			GenericNullableModel[] models = GenericNullableModel.FindAll();

			Assert.AreEqual(1, models.Length);

			model = models[0];

			Assert.AreEqual(null, model.Age);
            Assert.AreEqual(null, model.Completion);
            Assert.AreEqual(null, model.Accepted);
		}

		[Test]
		public void ProblemReportedOnForum()
		{
			SurveyAssociation model = new SurveyAssociation();
			model.SurveyId = 1;
			model.Save();

			SurveyAssociation[] models = SurveyAssociation.FindAll();

			Assert.AreEqual(1, models.Length);

			model = models[0];

			Assert.AreEqual(null, model.DepartmentId);
		}
	}
}

#endif