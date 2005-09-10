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

namespace Castle.Facilities.RemoteIntegration
{
	using System;
	using System.Runtime.Remoting.Channels;
	using System.Runtime.Remoting.Channels.Tcp;
	using System.Runtime.Remoting.Channels.Http;

	using Castle.Model.Configuration;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;
    
	public class RemoteFacility : AbstractFacility
	{
		private IConversionManager _converter;

		public RemoteFacility()
		{
		}

		protected override void Init()
		{
			_converter = Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey ) as IConversionManager;
			ConfigureFacility();
		}

		private void ConfigureFacility()
		{
			Kernel.ProxyFactory = new RemoteProxyFactory(Kernel.ProxyFactory);
			Kernel.ComponentModelBuilder.AddContributor(new RemoteInspector());
			Kernel.AddComponent(KnownRemoteConsts.RemoteIntercepter, typeof(RemoteInterceptor));
			InitChannels();
			InitSubSystem();
		}

		private void InitChannels()
		{
			IConfiguration channels = FacilityConfig.Children["channels"];
			if(channels==null)
				return;
			foreach(IConfiguration channel in  channels.Children)
				ConfigureChannel( channel );
		}
        
		protected void InitSubSystem()
		{
			Kernel.AddSubSystem(KnownRemoteConsts.SubSystemRemoteConnection,new RemoteSubSystem(FacilityConfig));
			//override NamingsSuSystem
			INamingSubSystem currentSubSystem = (INamingSubSystem)Kernel.GetSubSystem(SubSystemConstants.NamingKey);
			Kernel.AddSubSystem(SubSystemConstants.NamingKey,new RemoteNamingSubSystem(currentSubSystem.GetKey2Handler(), currentSubSystem.GetService2Handler()));
		}

		public virtual void ConfigureChannel(IConfiguration channel)
		{
			if(channel==null)
				throw new ArgumentNullException("channel");

			String portAtt = channel.Attributes["port"];
			int port=0;

			if(portAtt!=null)
				port = (int) _converter.PerformConversion(portAtt, typeof(int) );

			//TODO: Custom Channel type from config and sink
			IChannel newChannel=null;

			if(channel.Attributes["ref"]=="tcp")
			{
				if(port>0)
					newChannel = new TcpChannel(port);
				else
					newChannel = new TcpChannel();
			}
			else
			if(channel.Attributes["ref"]=="http")
			{
				if(port>0)
					newChannel = new HttpChannel(port);
				else
					newChannel = new HttpChannel();
			}
			if(newChannel==null)
				throw new RemoteException("Invalid configuration channel "+channel.Name+". 'ref' attribute not found or invalid !");

			ChannelServices.RegisterChannel(newChannel);
		}
	}
}
