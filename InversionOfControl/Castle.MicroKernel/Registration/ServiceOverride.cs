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

namespace Castle.MicroKernel.Registration
{
	using System;

	public class ServiceOverride : Property
	{
		internal ServiceOverride(String key, String value)
			: base(key, value)
		{
		}

		public new String Value
		{
			get { return (String) base.Value; }
		}


		public new static ServiceOverrideKey WithKey(String key)
		{
			return new ServiceOverrideKey(key);
		}
	}

	public class ServiceOverrideKey
	{
		private readonly String name;

		internal ServiceOverrideKey(String name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public ServiceOverride Eq(String value)
		{
			return new ServiceOverride(name, value);
		}
	}
}