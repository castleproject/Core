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

namespace Castle.Facilities.WcfIntegration
{
    using System;

	public class WcfClientModel
	{
		private IWcfEndpoint endpoint;

		public WcfClientModel()
		{
		}

		public WcfClientModel(IWcfEndpoint endpoint)
		{
			Endpoint = endpoint;
		}

		public Type Contract
		{
			get { return endpoint.Contract; }
		}

		public IWcfEndpoint Endpoint
		{
			get { return endpoint; }
			set 
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				endpoint = value; 
			}
		}
	}
}

