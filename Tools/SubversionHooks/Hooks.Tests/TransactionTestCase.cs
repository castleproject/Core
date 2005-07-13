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

namespace Castle.SvnHooks.Tests
{
	using System;
	
	using NUnit.Framework;

	/// <summary>
	/// Summary description for TransactionTestCase.
	/// </summary>
	[TestFixture] public class TransactionTestCase
	{
		[Test] public void Parse()
		{
			Transaction t = Transaction.Parse("0-1");
			
			Assert.AreEqual(0, t.FromRevision, "Transaction.FromRevision does not match the parsed string");
			Assert.AreEqual(1, t.ToRevision, "Transaction.ToRevision does not match the parsed string");
		}
		[Test] public void ParseHighNumbers()
		{
			Transaction t = Transaction.Parse(String.Concat(Int32.MaxValue - 1, '-', Int32.MaxValue));
			
			Assert.AreEqual(Int32.MaxValue-1, t.FromRevision, "Transaction.FromRevision does not match the parsed string");
			Assert.AreEqual(Int32.MaxValue, t.ToRevision, "Transaction.ToRevision does not match the parsed string");
		}
		
		[ExpectedException(typeof(OverflowException))]
		[Test] public void ParseTooHighNumbers()
		{
			Transaction t = Transaction.Parse(String.Concat(Int64.MaxValue - 1, '-', Int64.MaxValue));
		}
		

		[Ignore("The transaction numbers dont seem to be from revision - to reveision, but rather something else")]
		[ExpectedException(typeof(FormatException))]
		[Test] public void ParseNegativeFromRevision()
		{
			Transaction t = Transaction.Parse(String.Concat(-1, '-', 10));
		}
		[Ignore("The transaction numbers dont seem to be from revision - to reveision, but rather something else")]
		[ExpectedException(typeof(FormatException))]
		[Test] public void ParseToRevisionLowerThanFromRevision()
		{
			Transaction t = Transaction.Parse(String.Concat(10, '-', 0));
		}
		[Ignore("The transaction numbers dont seem to be from revision - to reveision, but rather something else")]
		[ExpectedException(typeof(FormatException), "From revision number must be lower than To revision: 10-10")]
		[Test] public void ParseToRevisionEqualFromRevision()
		{
			Transaction t = Transaction.Parse(String.Concat(10, '-', 10));
		}
	}
}
