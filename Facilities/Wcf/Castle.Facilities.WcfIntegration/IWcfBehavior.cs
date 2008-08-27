// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

using System.Collections.Generic;

namespace Castle.Facilities.WcfIntegration
{
	using System.ServiceModel.Description;
	using Castle.MicroKernel;

	public interface IWcfBehavior
	{
		void Accept(IWcfBehaviorVisitor visitor);
		ICollection<IHandler> GetHandlers(IKernel kernel);
	}

	public interface IWcfServiceBehavior : IWcfBehavior
	{
		void Install(ServiceDescription description, IKernel kernel);
	}

	public interface IWcfEndpointBehavior : IWcfBehavior
	{
		void Install(ServiceEndpoint endpoint, IKernel kernel);
	}
}
