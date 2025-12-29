// Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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

#nullable enable

namespace Castle.DynamicProxy
{
	using System;
	using System.Diagnostics.Tracing;
#if FEATURE_EVENTCOUNTER
	using System.Threading;
#endif

	[EventSource(Name = "Castle.DynamicProxy")]
	internal sealed class DynamicProxyEventSource : EventSource
	{
		public static DynamicProxyEventSource Log { get; } = new DynamicProxyEventSource();

#if FEATURE_EVENTCOUNTER
		private int typeCacheHitCount;
		private int typeCacheMissCount;
		private int typeGeneratedCount;
#endif

		private DynamicProxyEventSource()
		{
		}

		private enum EventId
		{
			None,
			TypeCacheHit,
			TypeCacheMiss,
			TypeGenerated,
		}

		[NonEvent]
		public void TypeCacheHit(Type requested, Type cached)
		{
#if FEATURE_EVENTCOUNTER
			Interlocked.Increment(ref typeCacheHitCount);
#endif

			if (IsEnabled())
			{
				TypeCacheHit(requested.FullName!, cached.FullName!);
			}
		}

		[Event((int)EventId.TypeCacheHit)]
		private void TypeCacheHit(string requested, string cached)
		{
			WriteEvent((int)EventId.TypeCacheHit, requested, cached);
		}

		[NonEvent]
		public void TypeCacheMiss(Type requested)
		{
#if FEATURE_EVENTCOUNTER
			Interlocked.Increment(ref typeCacheMissCount);
#endif

			if (IsEnabled())
			{
				TypeCacheMiss(requested.FullName!);
			}
		}

		[Event((int)EventId.TypeCacheMiss)]
		private void TypeCacheMiss(string requested)
		{
			WriteEvent((int)EventId.TypeCacheMiss, requested);
		}

		[NonEvent]
		public void TypeGenerated(Type type)
		{
#if FEATURE_EVENTCOUNTER
			Interlocked.Increment(ref typeGeneratedCount);
#endif

			if (IsEnabled())
			{
				TypeGenerated(type.FullName!);
			}
		}

		[Event((int)EventId.TypeGenerated)]
		private void TypeGenerated(string type)
		{
			WriteEvent((int)EventId.TypeGenerated, type);
		}

#if FEATURE_EVENTCOUNTER
		protected override void OnEventCommand(EventCommandEventArgs command)
		{
			if (command.Command == EventCommand.Enable)
			{
				_ = new PollingCounter("castle.dynamic_proxy.type_cache_hit.count", this, () => typeCacheHitCount)
				{
					DisplayName = "Type cache hits",
					DisplayUnits = "count",
				};

				_ = new PollingCounter("castle.dynamic_proxy.type_cache_miss.count", this, () => typeCacheMissCount)
				{
					DisplayName = "Type cache misses",
					DisplayUnits = "count",
				};

				_ = new PollingCounter("castle.dynamic_proxy.type_generated.count", this, () => typeGeneratedCount)
				{
					DisplayName = "Types generated",
					DisplayUnits = "count",
				};
			}
		}
#endif
	}
}
