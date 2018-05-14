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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal;

	/// <summary>
	///   Excludes a test method from test runs on the specified framework(s).
	///   This attribute may be placed on the same method more than once.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class ExcludeOnFrameworkAttribute : NUnitAttribute, IApplyToTest
	{
		private Framework oneOrMoreFrameworks;
		private string reason;

		/// <summary>
		///   Initializes a new instance of the <see cref="ExcludeOnFrameworkAttribute"/> class.
		/// </summary>
		/// <param name="oneOrMoreFrameworks">
		///   The framework(s) on which the test should be excluded.
		/// </param>
		/// <param name="reason">
		///   The reason why the test should excluded.
		/// </param>
		public ExcludeOnFrameworkAttribute(Framework oneOrMoreFrameworks, string reason)
		{
			this.oneOrMoreFrameworks = oneOrMoreFrameworks;
			this.reason = reason;
		}

		public void ApplyToTest(Test test)
		{
			if (test.RunState == RunState.NotRunnable || test.RunState == RunState.Skipped)
			{
				return;
			}

			foreach (Framework framework in Enum.GetValues(typeof(Framework)))
			{
				if ((this.oneOrMoreFrameworks & framework) != 0 && framework.IsRunning())
				{
					test.RunState = RunState.Skipped;
					test.Properties.Add(PropertyNames.SkipReason, $"Not supported on {framework.GetName()}. ({this.reason})");
					break;
				}
			}
		}
	}
}
