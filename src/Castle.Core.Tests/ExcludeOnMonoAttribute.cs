// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ExcludeOnMonoAttribute : NUnitAttribute, IApplyToTest
	{
		private static bool IsRunningOnMono() => Type.GetType("Mono.Runtime", throwOnError: false) != null;

		private string reason;

		public ExcludeOnMonoAttribute(string reason)
		{
			this.reason = reason;
		}

		public void ApplyToTest(Test test)
		{
			if (test.RunState != RunState.NotRunnable && test.RunState != RunState.Skipped && IsRunningOnMono())
			{
				test.RunState = RunState.Skipped;
				test.Properties.Add(PropertyNames.SkipReason, $"Not supported on Mono. ({reason})");
			}
		}
	}
}
