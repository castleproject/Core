// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Tests
{
	using System.Threading;

	using Castle.Core.Internal;

	using Xunit;

#if !SILVERLIGHT && !NETCORE
	public class SlimReadWriteLockTestCase
	{
		private SlimReadWriteLock @lock;

		public SlimReadWriteLockTestCase()
		{
			@lock = new SlimReadWriteLock();
		}

		[Fact]
		public void Can_be_used_ForReading_multiple_nested_time()
		{
			using (@lock.ForReading())
			{
				using (@lock.ForReading())
				{
					Assert.True(@lock.IsReadLockHeld);
				}
			}
		}

		[Fact]
		public void Can_be_used_ForWriting_multiple_nested_time()
		{
			using (@lock.ForWriting())
			{
				using (@lock.ForWriting())
				{
					Assert.True(@lock.IsWriteLockHeld);
				}
			}
		}

		[Fact]
		public void Can_be_used_ForReadingUpgradeable_multiple_nested_time()
		{
			using (@lock.ForReadingUpgradeable())
			{
				using (@lock.ForReadingUpgradeable())
				{
					Assert.True(@lock.IsUpgradeableReadLockHeld);
				}
			}
		}

		[Fact]
		public void Can_be_upgraded_from_nested_ForReadingUpgradeable()
		{
			using (@lock.ForReadingUpgradeable())
			{
				using (var holder = @lock.ForReadingUpgradeable())
				{
					holder.Upgrade();
					Assert.True(@lock.IsWriteLockHeld);
				}
			}
		}

		[Fact]
		public void Can_be_used_ForReading_when_used_ForWriting()
		{
			using (@lock.ForWriting())
			{
				using (var holder = @lock.ForReading())
				{
					Assert.True(@lock.IsWriteLockHeld);
					Assert.True(holder.LockAcquired);
				}
			}
		}

		[Fact]
		public void Can_be_used_ForReading_when_used_ForReadingUpgradeable()
		{
			using (@lock.ForReadingUpgradeable())
			{
				using (var holder = @lock.ForReading())
				{
					Assert.True(@lock.IsUpgradeableReadLockHeld);
					Assert.True(holder.LockAcquired);
				}
			}
		}

#if !MONO26
		[Fact]
		public void Can_NOT_be_used_ForReadingUpgradeable_when_used_ForReading()
		{
			using (@lock.ForReading())
			{
				Assert.Throws(typeof(LockRecursionException), () => @lock.ForReadingUpgradeable());
			}
		}
#endif

		[Fact]
		public void Can_be_used_ForReadingUpgradeable_when_used_ForWriting()
		{
			using (@lock.ForWriting())
			{
				using (var holder = @lock.ForReadingUpgradeable())
				{
					Assert.True(@lock.IsWriteLockHeld);
					Assert.True(holder.LockAcquired);
				}
			}
		}

		[Fact]
		public void Can_NOT_be_used_ForWriting_when_used_ForReading()
		{
			using (@lock.ForReading())
			{
				Assert.Throws(typeof(LockRecursionException), () => @lock.ForWriting());
			}
		}

		[Fact]
		public void Can_be_used_ForWriting_when_used_ForReadingUpgradeable()
		{
			using (@lock.ForReadingUpgradeable())
			{
				using (var holder = @lock.ForWriting())
				{
					Assert.True(@lock.IsUpgradeableReadLockHeld);
					Assert.True(holder.LockAcquired);
				}
			}
		}

		[Fact]
		public void Can_be_used_ForWriting_when_used_ForReadingUpgradeable_and_upgraded_after()
		{
			using (var upg = @lock.ForReadingUpgradeable())
			{
				using (var holder = @lock.ForWriting())
				{
					Assert.True(@lock.IsUpgradeableReadLockHeld);
					Assert.True(holder.LockAcquired);
					upg.Upgrade();
				}
			}
		}

		[Fact]
		public void Can_be_used_ForWriting_when_used_ForReadingUpgradeable_and_upgraded_before()
		{
			using (var upg = @lock.ForReadingUpgradeable())
			{
				upg.Upgrade();
				using (var holder = @lock.ForWriting())
				{
					Assert.True(@lock.IsUpgradeableReadLockHeld);
					Assert.True(holder.LockAcquired);
				}
			}
		}
	}
#endif
}