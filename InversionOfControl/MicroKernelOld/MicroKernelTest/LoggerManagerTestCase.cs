// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Test
{
	using System;

	using NUnit.Framework;

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Subsystems.Logger;
	using Castle.MicroKernel.Subsystems.Logger.Default;

	/// <summary>
	/// Summary description for LoggerManagerTestCase.
	/// </summary>
	[TestFixture]
	public class LoggerManagerTestCase : Assertion
	{
		[Test]
		public void TestUsage()
		{
			IKernel kernel = new BaseKernel();
			LoggerManager manager = new LoggerManager();

			kernel.AddSubsystem( KernelConstants.LOGGER, manager );
			
			IKernelSubsystem subsystem = kernel.GetSubsystem( KernelConstants.LOGGER );
			AssertNotNull( subsystem );
			AssertEquals( subsystem, manager );
		}

		[Test]
		public void TestCreateLogger()
		{
			IKernel kernel = new BaseKernel();
			LoggerManager manager = new LoggerManager();

			kernel.AddSubsystem( KernelConstants.LOGGER, manager );
			
			ILogger logger = manager.CreateLogger( "name", "impl", null );
			AssertNotNull( logger );
		}
	}
}
