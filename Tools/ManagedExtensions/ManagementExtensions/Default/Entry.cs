// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Default
{
	using System;

	/// <summary>
	/// Summary description for Entry.
	/// </summary>
	public class Entry
	{
		protected Object instance;
		protected MDynamicSupport invoker;

		public Entry(Object instance, MDynamicSupport invoker)
		{
			this.instance = instance;
			this.invoker  = invoker;
		}

		public Object Instance
		{
			get
			{
				return instance;
			}
		}

		public MDynamicSupport Invoker 
		{
			get
			{
				return invoker;
			}
		}
	}
}
