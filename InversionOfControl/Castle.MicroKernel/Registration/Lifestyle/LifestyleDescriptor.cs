// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration
{
	using Castle.Core;
	using Castle.Core.Configuration;

	public class LifestyleDescriptor<S> : ComponentDescriptor<S>
	{
		private readonly LifestyleType lifestyle;

		public LifestyleDescriptor(LifestyleType lifestyle)
		{
			this.lifestyle = lifestyle;	
		}

		protected internal override void ApplyToConfiguration(IKernel kernel, IConfiguration configuration)
		{
			if (configuration.Attributes["lifestyle"] == null || IsOverWrite)
			{
				configuration.Attributes["lifestyle"] = lifestyle.ToString();
				ApplyLifestyleDetails(configuration);
			}
		}

		protected virtual void ApplyLifestyleDetails(IConfiguration configuration)
		{
		}
	}
}