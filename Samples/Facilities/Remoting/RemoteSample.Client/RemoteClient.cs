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

namespace RemoteSample.Client
{

	using System;
	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	using RemoteSample.Components;

	class RemoteClientMain
	{
		[STAThread]
		static void Main(String[] args)
		{
			IWindsorContainer container = new WindsorContainer( new XmlInterpreter("../../ClientConfig.xml") );
			
			// The component doesn't exists here, so it's a remoteComponent
			IRemoteConsole remoteConsole = (IRemoteConsole)container["remote.console"];
			remoteConsole.WriteLine("Writing this text from Client into Server.");
			
			Console.WriteLine("....press a key to stop");
			Console.ReadLine();
		}
	}
}
