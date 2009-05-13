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

namespace Castle.ActiveRecord.Framework.Internal.Tests
{
	using System.Collections;
	using NUnit.Framework;

	[TestFixture]
	public class InflectorTestCase
	{
		#region Fixture Data

		private static readonly Hashtable singularToPlural = new Hashtable();

		[TestFixtureSetUp]
		public void SetupTestData()
		{
			singularToPlural.Add("search", "searches");
			singularToPlural.Add("switch", "switches");
			singularToPlural.Add("fix", "fixes");
			singularToPlural.Add("box", "boxes");
			singularToPlural.Add("process", "processes");
			singularToPlural.Add("address", "addresses");
			singularToPlural.Add("case", "cases");
			singularToPlural.Add("stack", "stacks");
			singularToPlural.Add("wish", "wishes");
			singularToPlural.Add("fish", "fish");

			singularToPlural.Add("category", "categories");
			singularToPlural.Add("query", "queries");
			singularToPlural.Add("ability", "abilities");
			singularToPlural.Add("agency", "agencies");
			singularToPlural.Add("movie", "movies");

			singularToPlural.Add("archive", "archives");

			singularToPlural.Add("index", "indices");

			singularToPlural.Add("wife", "wives");
			singularToPlural.Add("safe", "saves");
			singularToPlural.Add("half", "halves");

			singularToPlural.Add("move", "moves");

			singularToPlural.Add("salesperson", "salespeople");
			singularToPlural.Add("person", "people");

			singularToPlural.Add("spokesman", "spokesmen");
			singularToPlural.Add("man", "men");
			singularToPlural.Add("woman", "women");

			singularToPlural.Add("basis", "bases");
			singularToPlural.Add("diagnosis", "diagnoses");

			singularToPlural.Add("datum", "data");
			singularToPlural.Add("medium", "media");
			singularToPlural.Add("analysis", "analyses");

			singularToPlural.Add("node_child", "node_children");
			singularToPlural.Add("child", "children");

			singularToPlural.Add("experience", "experiences");
			singularToPlural.Add("day", "days");

			singularToPlural.Add("comment", "comments");
			singularToPlural.Add("foobar", "foobars");
			singularToPlural.Add("newsletter", "newsletters");

			singularToPlural.Add("old_news", "old_news");
			singularToPlural.Add("news", "news");

			singularToPlural.Add("series", "series");
			singularToPlural.Add("species", "species");

			singularToPlural.Add("quiz", "quizzes");

			singularToPlural.Add("perspective", "perspectives");

			singularToPlural.Add("ox", "oxen");
			singularToPlural.Add("photo", "photos");
			singularToPlural.Add("buffalo", "buffaloes");
			singularToPlural.Add("tomato", "tomatoes");
			singularToPlural.Add("dwarf", "dwarves");
			singularToPlural.Add("elf", "elves");
			singularToPlural.Add("information", "information");
			singularToPlural.Add("equipment", "equipment");
			singularToPlural.Add("bus", "buses");
			singularToPlural.Add("status", "statuses");
			singularToPlural.Add("status_code", "status_codes");
			singularToPlural.Add("mouse", "mice");

			singularToPlural.Add("louse", "lice");
			singularToPlural.Add("house", "houses");
			singularToPlural.Add("octopus", "octopi");
			singularToPlural.Add("virus", "viri");
			singularToPlural.Add("alias", "aliases");
			singularToPlural.Add("portfolio", "portfolios");

			singularToPlural.Add("vertex", "vertices");
			singularToPlural.Add("matrix", "matrices");

			singularToPlural.Add("axis", "axes");
			singularToPlural.Add("testis", "testes");
			singularToPlural.Add("crisis", "crises");

			singularToPlural.Add("rice", "rice");
			singularToPlural.Add("shoe", "shoes");

			singularToPlural.Add("horse", "horses");
			singularToPlural.Add("prize", "prizes");
			singularToPlural.Add("edge", "edges");
		}

		#endregion

		[Test]
		public void PluralizePlurals()
		{
			Assert.AreEqual("plurals", Inflector.Pluralize("plurals"));
			Assert.AreEqual("Plurals", Inflector.Pluralize("Plurals"));
		}

		[Test]
		public void Pluralize()
		{
			foreach(DictionaryEntry dictionaryEntry in singularToPlural)
			{
				Assert.AreEqual(dictionaryEntry.Value, Inflector.Pluralize((string) dictionaryEntry.Key));
				Assert.AreEqual(Inflector.Capitalize((string) dictionaryEntry.Value),
				                Inflector.Pluralize(Inflector.Capitalize((string) dictionaryEntry.Key)));
			}
		}

		[Test]
		public void Singularize()
		{
			foreach(DictionaryEntry dictionaryEntry in singularToPlural)
			{
				Assert.AreEqual(dictionaryEntry.Key, Inflector.Singularize((string) dictionaryEntry.Value));
				Assert.AreEqual(Inflector.Capitalize((string) dictionaryEntry.Key),
				                Inflector.Singularize(Inflector.Capitalize((string) dictionaryEntry.Value)));
			}
		}
	}
}
