// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model.StrictModel;


	[TestFixture]
	public class StrictModelTestCase : AbstractActiveRecordTest
	{
		[SetUp]
		public void Init()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Estrato), typeof(ReferenceEstrato), typeof(SurveyEstrato),
				typeof(QuestionContainer), typeof(Repository), typeof(Survey) );
	
			Recreate();
		}

		[Test]
		public void Creation()
		{
			Repository repos = new Repository();
			repos.Save();

			ReferenceEstrato refEst = new ReferenceEstrato();
			refEst.Repository = repos;

			ReferenceEstrato subRefEst1 = new ReferenceEstrato();
			subRefEst1.ParentEstrato = refEst;
			subRefEst1.Repository = repos;
			ReferenceEstrato subRefEst2 = new ReferenceEstrato();
			subRefEst2.ParentEstrato = refEst;
			subRefEst2.Repository = repos;

			refEst.Save();
			subRefEst1.Save();
			subRefEst2.Save();

			ReferenceEstrato[] refEsts = ReferenceEstrato.FindAll();
			Assert.AreEqual(3, refEsts.Length);

			SurveyEstrato[] surEsts = SurveyEstrato.FindAll();
			Assert.AreEqual(0, surEsts.Length);
		}

		[Test]
		public void DistinctTypes()
		{
			Repository repos = new Repository();

			ReferenceEstrato loc = new ReferenceEstrato(repos);
			ReferenceEstrato sul = new ReferenceEstrato(loc);
			ReferenceEstrato norte = new ReferenceEstrato(loc);
			ReferenceEstrato sudeste = new ReferenceEstrato(loc);
			ReferenceEstrato nordeste = new ReferenceEstrato(loc);

			using(new SessionScope())
			{
				repos.Save();
				loc.Save();
				sul.Save();
				sudeste.Save();
				norte.Save();
				nordeste.Save();
			}

			Survey pesquisa = new Survey();

			SurveyEstrato extCidade = new SurveyEstrato(pesquisa);
			// extCidade.ReferencedEstratos.Add(loc);

			SurveyEstrato extSub1 = new SurveyEstrato(extCidade, pesquisa);
			// extSub1.ReferencedEstratos.Add(sudeste);

			SurveyEstrato extSub2 = new SurveyEstrato(extCidade, pesquisa);
			// extSub2.ReferencedEstratos.Add(sudeste);

			SurveyEstrato extSub3 = new SurveyEstrato(extCidade, pesquisa);
			// extSub3.ReferencedEstratos.Add(sudeste);

			using(new SessionScope())
			{
				pesquisa.Save();
				extCidade.Save();
				extSub1.Save();
				extSub2.Save();
				extSub3.Save();
			}

			extCidade = SurveyEstrato.Find(extCidade.Id);
			Assert.IsNotNull(extCidade);

			Estrato[] estratos = SurveyEstrato.FindAll();
			Assert.AreEqual( 4, estratos.Length );

			estratos = ReferenceEstrato.FindAll();
			Assert.AreEqual( 5, estratos.Length );
		}

		[Test]
		public void UsageWithReferences()
		{
			Repository repos = new Repository();

			ReferenceEstrato loc = new ReferenceEstrato(repos);
			ReferenceEstrato sul = new ReferenceEstrato(loc);
			ReferenceEstrato norte = new ReferenceEstrato(loc);
			ReferenceEstrato sudeste = new ReferenceEstrato(loc);
			ReferenceEstrato nordeste = new ReferenceEstrato(loc);

			using(new SessionScope())
			{
				repos.Save();
				loc.Save();
				sul.Save();
				sudeste.Save();
				norte.Save();
				nordeste.Save();
			}

			Survey pesquisa = new Survey();

			SurveyEstrato extCidade = new SurveyEstrato(pesquisa);
			extCidade.ReferencedEstratos.Add(loc);

			SurveyEstrato extSub1 = new SurveyEstrato(extCidade, pesquisa);
			extSub1.ReferencedEstratos.Add(sudeste);

			SurveyEstrato extSub2 = new SurveyEstrato(extCidade, pesquisa);
			extSub2.ReferencedEstratos.Add(sudeste);

			SurveyEstrato extSub3 = new SurveyEstrato(extCidade, pesquisa);
			extSub3.ReferencedEstratos.Add(sudeste);

			using(new SessionScope())
			{
				pesquisa.Save();
				extCidade.Save();
				extSub1.Save();
				extSub2.Save();
				extSub3.Save();
			}

			extCidade = SurveyEstrato.Find(extCidade.Id);
			Assert.IsNotNull(extCidade);

			Estrato[] estratos = SurveyEstrato.FindAll();
			Assert.AreEqual( 4, estratos.Length );

			estratos = ReferenceEstrato.FindAll();
			Assert.AreEqual( 5, estratos.Length );
		}
	}
}
