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
	using Castle.Model;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.SubSystems;
	using Castle.MicroKernel.SubSystems.Conversion;

	public class RemoteInspector : IContributeComponentModelConstruction
	{
		private IConversionManager converter;

		public RemoteInspector()
		{
		}
		#region IContributeComponentModelConstruction Members

		public void ProcessModel(Castle.MicroKernel.IKernel kernel, ComponentModel model)
		{
			if (model.Configuration == null) return;

			if(converter==null)
				converter = kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey ) as IConversionManager;

			String remoteAtt = model.Configuration.Attributes["remote"];
			if(remoteAtt==null) return;
			bool remotable = (bool) converter.PerformConversion( 
				remoteAtt, typeof(bool) );

			model.ExtendedProperties["remotable"] = remotable;
			if(remotable)
			{
				//TODO: Intercept by proxy
				//model.Interceptors.Add(new InterceptorReference(typeof(RemoteInterceptor)));
				//model.CustomComponentActivator = typeof(RemoteActivator);
			}
		}

		#endregion
	}
}
