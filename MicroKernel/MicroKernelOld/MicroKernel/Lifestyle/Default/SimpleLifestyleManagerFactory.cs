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

namespace Castle.MicroKernel.Lifestyle.Default
{
	using System;
	
	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Model;

	/// <summary>
	/// Summary description for SimpleLifestyleManagerFactory.
	/// </summary>
	public class SimpleLifestyleManagerFactory : ILifestyleManagerFactory
	{
		public SimpleLifestyleManagerFactory()
		{
		}

		#region ILifestyleManagerFactory Members

		public ILifestyleManager Create( IComponentFactory factory, IComponentModel model )
		{
			if (model.SupportedLifestyle == Apache.Avalon.Framework.Lifestyle.Singleton)
			{
				return new SingletonLifestyleManager( factory );
			}
			else if (model.SupportedLifestyle == Apache.Avalon.Framework.Lifestyle.Thread)
			{
				return new PerThreadLifestyleManager( factory );
			}
			else if (model.SupportedLifestyle == Apache.Avalon.Framework.Lifestyle.Transient)
			{
				return new TransientLifestyleManager( factory );
			}
			else
			{
				throw new UnsupportedLifestyleException(
					String.Format("Lifestyle requested by component {0} is not supported.", 
					model.ConstructionModel.Implementation));
			}
		}

		#endregion
	}
}
