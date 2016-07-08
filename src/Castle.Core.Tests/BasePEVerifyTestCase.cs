// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
#if FEATURE_ASSEMBLYBUILDER_SAVE
	using System.Diagnostics;
#endif
	using System.IO;

	using NUnit.Framework;

#if !__MonoCS__ // mono doesn't have PEVerify
	public class FindPeVerify
	{
		private static readonly string[] PeVerifyProbingPaths =
		{
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
			throw new FileNotFoundException(
				"Please check the PeVerifyProbingPaths configuration setting and set it to the folder where peverify.exe is located");
		}

		private static string peVerifyPath;

		public static string PeVerifyPath
		{
			get { return peVerifyPath ?? (peVerifyPath = FindPeVerifyPath()); }
		}
	}
#endif

	public abstract class BasePEVerifyTestCase
#if FEATURE_XUNITNET
		: IDisposable
#endif
	{
		protected ProxyGenerator generator;
		protected IProxyBuilder builder;

		private bool verificationDisabled;

#if FEATURE_XUNITNET
		protected BasePEVerifyTestCase()
		{
			Init();
		}
#else
		[SetUp]
#endif
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

#if FEATURE_XUNITNET
		public void Dispose()
		{
			TearDown();
		}
#endif

#if FEATURE_ASSEMBLYBUILDER_SAVE && !__MonoCS__ // mono doesn't have PEVerify
#if !FEATURE_XUNITNET
		[TearDown]
#endif
		public virtual void TearDown()
		{
			if (!IsVerificationDisabled)
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

			Console.WriteLine(GetType().FullName + ": " + result);

			if (process.ExitCode != 0)
			{
				Console.WriteLine(processOutput);
				Assert.Fail("PeVerify reported error(s): " + Environment.NewLine + processOutput, result);
			}
		}
#else
#if !FEATURE_XUNITNET
		[TearDown]
#endif
		public virtual void TearDown()
		{
		}
#endif
	}
}