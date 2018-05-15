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
	using System.Diagnostics;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Flags enumeration used to specify a framework, or a combination of frameworks.
	/// </summary>
	[Flags]
	public enum Framework
	{
		/// <summary>
		///   Mono.
		/// </summary>
		Mono = 1,

		/// <summary>
		///   .NET Core.
		/// </summary>
		NetCore = 2,

		/// <summary>
		///   The .NET Framework.
		/// </summary>
		NetFramework = 4,
	}

	internal static class FrameworkUtil
	{
		public static string GetName(this Framework framework)
		{
			switch (framework)
			{
				case Framework.Mono:
					return "Mono";

				case Framework.NetCore:
					return ".NET Core";

				case Framework.NetFramework:
					return "the .NET Framework";

				default:
					throw new ArgumentOutOfRangeException(nameof(framework));
			}
		}

		public static bool TryGetRunningFramework(out Framework runningFramework)
		{
			runningFramework = 0;

			if (Framework.Mono.IsRunning())
			{
				runningFramework = Framework.Mono;
			}

			if (Framework.NetCore.IsRunning())
			{
				Debug.Assert(runningFramework == 0);
				runningFramework = Framework.NetCore;
			}

			if (Framework.NetFramework.IsRunning())
			{
				Debug.Assert(runningFramework == 0);
				runningFramework = Framework.NetFramework;
			}

			return runningFramework != 0;
		}

		public static bool IsRunning(this Framework framework)
		{
			var frameworkDescription = RuntimeInformation.FrameworkDescription;

			switch (framework)
			{
				case Framework.Mono:
					return frameworkDescription.StartsWith("Mono");

				case Framework.NetCore:
					return frameworkDescription.StartsWith(".NET Core");

				case Framework.NetFramework:
					return frameworkDescription.StartsWith(".NET Framework");

				default:
					throw new ArgumentOutOfRangeException(nameof(framework));
			}
		}
	}
}
