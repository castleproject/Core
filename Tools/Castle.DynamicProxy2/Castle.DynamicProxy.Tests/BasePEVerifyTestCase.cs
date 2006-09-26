// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using NUnit.Framework;

	public abstract class BasePEVerifyTestCase
	{
		[TearDown]
		public void RunPEVerifyOnGeneratedAssembly()
		{
			Process process = new Process();

			process.StartInfo.FileName = @"C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\peverify.exe";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
			process.StartInfo.Arguments = "CastleDynProxy2.dll";
			process.Start();
			process.WaitForExit();

			string result = process.ExitCode + " code " + process.StandardOutput.ReadToEnd();

			Console.WriteLine(result);

			if (process.ExitCode != 0)
			{
				Assert.Fail(result);
			}
		}
	}
}
