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

	public class ParametersDescriptor<S> : ComponentDescriptor<S>
	{
		private readonly Parameter[] parameters;

		public ParametersDescriptor(params Parameter[] parameters)
		{
			this.parameters = parameters;
		}

		protected internal override void ApplyToModel(IKernel kernel, ComponentModel model)
		{
			foreach(Parameter parameter in parameters)
			{
				ApplyParameter(kernel, model, parameter);
			}
		}

		private void ApplyParameter(IKernel kernel, ComponentModel model, Parameter parameter)
		{
			if (parameter.Value != null)
			{
				Registration.AddParameter(kernel, model, parameter.Key, parameter.Value);
			}
			else if (parameter.ConfigNode != null)
			{
				Registration.AddParameter(kernel, model, parameter.Key, parameter.ConfigNode);				
			}
		}
	}
}