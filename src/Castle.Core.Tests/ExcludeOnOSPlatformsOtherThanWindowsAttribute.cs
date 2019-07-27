// Copyright 2004-2019 Castle Project - http://www.castleproject.org/
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
	using System.Runtime.InteropServices;
	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal;

	/// <summary>
	///   Excludes a test method from test runs on OS platforms other than Windows.
	///   This attribute may be placed on the same method more than once.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class ExcludeOnOSPlatformsOtherThanWindowsAttribute : NUnitAttribute, IApplyToTest
	{
		private string reason;

		/// <summary>
		///   Initializes a new instance of the <see cref="ExcludeOnOSPlatformsOtherThanWindowsAttribute"/> class.
		/// </summary>
		/// <param name="reason">
		///   The reason why the test should excluded.
		/// </param>
		public ExcludeOnOSPlatformsOtherThanWindowsAttribute(string reason)
		{
			this.reason = reason;
		}

		public void ApplyToTest(Test test)
		{
			if (test.RunState == RunState.NotRunnable || test.RunState == RunState.Skipped)
			{
				return;
			}

			var osDescription = RuntimeInformation.OSDescription;
			if (RuntimeInformation.OSDescription.Contains("Windows") == false)
			{
				test.RunState = RunState.Skipped;
				test.Properties.Add(PropertyNames.SkipReason, $"Only supported on Windows, but currently running on {osDescription}. ({this.reason})");
			}
		}
	}
}
