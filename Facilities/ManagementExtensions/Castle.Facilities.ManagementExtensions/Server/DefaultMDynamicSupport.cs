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

namespace Castle.Facilities.ManagedExtensions.Server
{
	using System;

	using Castle.Model.Configuration;

	using Castle.ManagementExtensions;

	/// <summary>
	/// Summary description for DefaultMDynamicSupport.
	/// </summary>
	public class DefaultMDynamicSupport : MDynamicSupport
	{
		private object _instance; 
		private IConfiguration _managementNode;

		public DefaultMDynamicSupport(object instance, IConfiguration managementNode)
		{
			_instance = instance;
			_managementNode = managementNode;
		}

		public Object Invoke(String action, Object[] args, Type[] signature)
		{
			throw new NotImplementedException();
		}

		public Object GetAttributeValue(String name)
		{
			throw new NotImplementedException();
		}

		public void SetAttributeValue(String name, Object value)
		{
			throw new NotImplementedException();
		}

		public ManagementInfo Info
		{
			get { throw new NotImplementedException(); }
		}
	}
}
