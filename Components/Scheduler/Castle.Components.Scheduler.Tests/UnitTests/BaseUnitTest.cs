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
	using NUnit.Framework;
	using Rhino.Mocks;

	/// <summary>
	/// Base unit test.
	/// All unit tests that require certain common facilities like Mock Objects
	/// inherit from this class.
	/// </summary>
	[TestFixture]
	public abstract class BaseUnitTest
	{
		private MockRepository mocks;

		/// <summary>
		/// Gets the mock object repository.
		/// </summary>
		public MockRepository Mocks
		{
			get
			{
				if (mocks == null)
					mocks = new MockRepository();

				return mocks;
			}
		}

		[SetUp]
		public virtual void SetUp()
		{
		}

		[TearDown]
		public virtual void TearDown()
		{
			if (mocks != null)
			{
				try
				{
					mocks.ReplayAll();
					mocks.VerifyAll();
				}
				finally
				{
					mocks = null;
				}
			}
		}
	}
}