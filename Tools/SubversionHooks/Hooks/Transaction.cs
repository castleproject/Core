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

namespace Castle.SvnHooks
{
	using System;
	
	/// <summary>
	/// A structure that holds the transaction 
	/// number for a given transaction.
	/// </summary>
	public struct Transaction
	{
		public Transaction(int fromRevision, int toRevision)
		{
			this.fromRevision = fromRevision;
			this.toRevision = toRevision;
		}

		public static Transaction FromRevisions(int fromRevision, int toRevision)
		{
			return new Transaction(fromRevision, toRevision);
		}

		public static Transaction Parse(String s)
		{
			String[] split = s.Split('-');

			Transaction t = new Transaction(
				Int32.Parse(split[0]),
				Int32.Parse(split[1]));

			// Please refer to the ignore message on the tests

//			if (t.FromRevision < 0)
//				throw new FormatException("From revision number cannot be negative");
//			if (t.ToRevision <= t.FromRevision)
//				throw new FormatException("From revision number must be lower than To revision: " + s);

			return t;
		}

		public override string ToString()
		{
			return String.Concat(fromRevision, "-", toRevision);
		}


		public int FromRevision
		{
			get { return this.fromRevision; }
		}

		public int ToRevision
		{
			get { return this.toRevision; }
		}


		private int fromRevision;
		private int toRevision;
	}
}
