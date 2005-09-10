// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.LoggingExample
{
	using System;
	using AspectSharp.Builder;
	using AspectSharp.LoggingExample.Model;

	/// <summary>
	/// Sample application demonstrating a very basic use of Aspect#.
	/// </summary>
	class Application
	{
		private static AspectEngine _engine;

		[STAThread]
		static void Main(string[] args)
		{
			//Reading the config from an application configuration file.
			AppDomainConfigurationBuilder builder = new AppDomainConfigurationBuilder();

			//Creating the AspectEngine, based on the configuration.
			_engine = builder.Build();

			ClassInvocation();

			Console.WriteLine(" ");
			Console.WriteLine(" ");

			InterfaceInvocation();
		}

		/// <summary>
		/// Shows interception of an interface method.
		/// </summary>
		private static void InterfaceInvocation()
		{
			Console.WriteLine("InterfaceInvocation");

			//Wraping the class which is compatible with the inteface.
			IAgatKiller agatKiller = _engine.WrapInterface(typeof(IAgatKiller), new Ronin()) as IAgatKiller;

			agatKiller.KillAgat("Demon's Castle", "Bleed Sword");
		}

		/// <summary>
		/// Shows interception of an class method.
		/// </summary>
		private static void ClassInvocation()
		{
			Console.WriteLine("ClassInvocation");

			//Wraping the class
			MachineGun machineGun = _engine.WrapClass(typeof(MachineGun)) as MachineGun;

			//Not intercepted. It isn't a virtual method.
			machineGun.Fire(5);

			//Intercepted.
			machineGun.FireTenTimes();
		}
	}
}
