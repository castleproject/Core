// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

using Castle.MicroKernel;
using Castle.Model;
using NHibernate.Cfg;

namespace Castle.Facilities.NHibernate
{
	using System;

	using Castle.MicroKernel.ComponentActivator;

	/// <summary>
	/// Summary description for SessionFactoryActivator.
	/// </summary>
	public class SessionFactoryActivator : AbstractComponentActivator
	{
		public SessionFactoryActivator(ComponentModel model, 
			IKernel kernel, ComponentInstanceDelegate onCreation, 
			ComponentInstanceDelegate onDestruction) : 
			base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object InternalCreate()
		{
			Configuration config = (Configuration) 
				Model.ExtendedProperties[ NHibernateFacility.ConfiguredObject ];

			
			return config.BuildSessionFactory();
		}

		protected override void InternalDestroy(object instance)
		{
		}
	}
}
