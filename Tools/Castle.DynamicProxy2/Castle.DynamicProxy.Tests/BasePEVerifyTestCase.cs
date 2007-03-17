// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using Castle.Core.Interceptor;
	using NUnit.Framework;

	public abstract class BasePEVerifyTestCase
	{
#if !MONO // mono doesn't have PEVerify
		
		[TearDown]
		public void RunPEVerifyOnGeneratedAssembly()
		{
			Process process = new Process();

#if DOTNET2
			string path = Path.Combine(ConfigurationManager.AppSettings["sdkDir"], "peverify.exe");
#else
			string path = Path.Combine(ConfigurationSettings.AppSettings["sdkDir"], "peverify.exe");
#endif

			if (!File.Exists(path))
			{
#if DOTNET2
				path = Path.Combine(ConfigurationManager.AppSettings["x86SdkDir"], "peverify.exe");
#else
				path = Path.Combine(ConfigurationSettings.AppSettings["x86SdkDir"], "peverify.exe");
#endif
			}
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("Please check the sdkDir configuration setting and set it to the location of peverify.exe");
			}
			process.StartInfo.FileName = path;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
			process.StartInfo.Arguments = typeof(IInterceptor).Assembly.GetName().Name + ".dll";
			process.Start();
			process.WaitForExit();

			string result = process.ExitCode + " code " + process.StandardOutput.ReadToEnd();

			Console.WriteLine(result);

			if (process.ExitCode != 0)
			{
				Assert.Fail(result);
			}
		}
#endif
	}
}
