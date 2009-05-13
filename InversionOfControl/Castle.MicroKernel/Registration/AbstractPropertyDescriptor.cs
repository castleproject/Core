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
	using System;
	using System.Collections;
	using Core;

	public abstract class AbstractPropertyDescriptor<S> : ComponentDescriptor<S>
	{
		private readonly Property[] properties;
		private readonly IDictionary dictionary;

		protected AbstractPropertyDescriptor(params Property[] properties)
		{
			this.properties = properties;
		}

		protected AbstractPropertyDescriptor(IDictionary dictionary)
		{
			this.dictionary = dictionary;
		}

		protected internal override void ApplyToModel(IKernel kernel, ComponentModel model)
		{
			if (dictionary != null)
			{
				foreach(DictionaryEntry property in dictionary)
				{
					ApplyProperty(kernel, model, property.Key.ToString(), property.Value, null);
				}
			}
			else if (properties != null)
			{
				foreach(Property property in properties)
				{
					ApplyProperty(kernel, model, property.Key, property.Value, property);
				}
			}
		}

		protected abstract void ApplyProperty(IKernel kernel, ComponentModel model,
		                                      String key, Object value, Property property);
	}
}