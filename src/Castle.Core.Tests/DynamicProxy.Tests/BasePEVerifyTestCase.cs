// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Diagnostics;
	using System.IO;

	using NUnit.Framework;

	public class FindPeVerify
	{
		private static readonly string[] PeVerifyProbingPaths =
		{
			@"C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.1 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.1 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\NETFX 4.0 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1\Bin\NETFX 4.0 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools",
			@"C:\Program Files\Microsoft SDKs\Windows\v7.0A\Bin",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin",
			@"C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin",
			@"C:\Program Files (x86)\Microsoft SDKs\Windows\v6.0A\Bin",
			@"C:\Program Files (x86)\Microsoft Visual Studio 8\SDK\v2.0\bin"
		};

		private static string FindPeVerifyPath()
		{
			var peVerifyProbingPaths = PeVerifyProbingPaths;
			foreach (var path in peVerifyProbingPaths)
			{
				var file = Path.Combine(path, "peverify.exe");
				if (File.Exists(file))
				{
					return file;
				}
			}
			return null;
		}

		private static Lazy<string> peVerifyPath => new Lazy<string>(FindPeVerifyPath);

		public static string PeVerifyPath => peVerifyPath.Value;
	}

	public abstract class BasePEVerifyTestCase
	{
		protected ProxyGenerator generator;
		protected IProxyBuilder builder;

		private bool verificationDisabled;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[SetUp]
		public virtual void Init()
		{
			ResetGeneratorAndBuilder();
			verificationDisabled = false;
		}

		public void ResetGeneratorAndBuilder()
		{
#if FEATURE_ASSEMBLYBUILDER_SAVE
			builder = new PersistentProxyBuilder();
#else
			builder = new DefaultProxyBuilder();
#endif
			generator = new ProxyGenerator(builder);
		}

		public void DisableVerification()
		{
			verificationDisabled = true;
		}

		public bool IsVerificationDisabled
		{
			get { return verificationDisabled; }
		}

#if FEATURE_ASSEMBLYBUILDER_SAVE

		public bool IsVerificationPossible => FindPeVerify.PeVerifyPath != null;

		[TearDown]
		public virtual void TearDown()
		{
			if (IsVerificationPossible && !IsVerificationDisabled)
			{
				// Note: only supports one generated assembly at the moment
				var path = ((PersistentProxyBuilder)builder).SaveAssembly();
				if (path != null)
				{
					RunPEVerifyOnGeneratedAssembly(path);
				}
			}
		}

		public void RunPEVerifyOnGeneratedAssembly(string assemblyPath)
		{
			if (!IsVerificationPossible)
			{
				throw new InvalidOperationException();
			}

			var process = new Process
			{
				StartInfo =
					{
						FileName = FindPeVerify.PeVerifyPath,
						RedirectStandardOutput = true,
						UseShellExecute = false,
						WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
						Arguments = "\"" + assemblyPath + "\" /VERBOSE",
						CreateNoWindow = true
					}
			};
			process.Start();
			var processOutput = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			var result = process.ExitCode + " code ";

			if (process.ExitCode != 0)
			{
				Console.WriteLine(processOutput);
				Assert.Fail("PeVerify reported error(s): " + Environment.NewLine + processOutput, result);
			}
		}

#else
		public bool IsVerificationPossible => false;

		[TearDown]
		public virtual void TearDown()
		{
		}
#endif // FEATURE_ASSEMBLYBUILDER_SAVE
	}
}