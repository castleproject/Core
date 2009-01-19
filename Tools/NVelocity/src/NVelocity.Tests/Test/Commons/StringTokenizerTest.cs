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

namespace NVelocity.Test.Commons
{
	using NUnit.Framework;
	using StringTokenizer = global::Commons.Collections.StringTokenizer;

	[TestFixture]
	public class StringTokenizerTest
	{
		[Test]
		public void StringIsTokenizedWithDefaultDelimiters()
		{
			const string toTokenize = "First\tSecond\tThird";
			StringTokenizer tokenizer = new StringTokenizer(toTokenize);

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("First", tokenizer.NextToken());

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("Second", tokenizer.NextToken());

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("Third", tokenizer.NextToken());

			Assert.IsFalse(tokenizer.HasMoreTokens());
		}

		[Test]
		public void StringIsTokenizedWithSpecifiedDelimiters()
		{
			const string toTokenize = "First,Second,Third";
			StringTokenizer tokenizer = new StringTokenizer(toTokenize, ",");

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("First", tokenizer.NextToken());

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("Second", tokenizer.NextToken());

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("Third", tokenizer.NextToken());

			Assert.IsFalse(tokenizer.HasMoreTokens());
		}

		[Test]
		public void RepeatedStringIsTokenizedCorrectly()
		{
			const string toTokenize = "First\tFirstly\tThird";
			StringTokenizer tokenizer = new StringTokenizer(toTokenize);

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("First", tokenizer.NextToken());

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("Firstly", tokenizer.NextToken());

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("Third", tokenizer.NextToken());

			Assert.IsFalse(tokenizer.HasMoreTokens());
		}

		[Test]
		public void ChangingDelimitersIsHandledCorrectly()
		{
			const string toTokenize = "First,more\tSecond,Third";
			StringTokenizer tokenizer = new StringTokenizer(toTokenize);

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("First,more", tokenizer.NextToken());

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("Second", tokenizer.NextToken(","));

			Assert.IsTrue(tokenizer.HasMoreTokens());
			Assert.AreEqual("Third", tokenizer.NextToken());

			Assert.IsFalse(tokenizer.HasMoreTokens());
		}

		[Test]
		public void CountIsCorrect()
		{
			const string toTokenize = "First\tSecond\tThird";
			StringTokenizer tokenizer = new StringTokenizer(toTokenize);

			Assert.AreEqual(3, tokenizer.Count);

			tokenizer.NextToken();
			Assert.AreEqual(2, tokenizer.Count);

			tokenizer.NextToken();
			Assert.AreEqual(1, tokenizer.Count);

			string token = tokenizer.NextToken();
			// This assert assures that asking for the count does not
			// affect the tokens themselves.
			Assert.AreEqual("Third", token);
			Assert.AreEqual(0, tokenizer.Count);
		}
	}
}
