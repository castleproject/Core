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

namespace Castle.MicroKernel.Model.Default
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Summary description for DefaultConstructionModel.
	/// </summary>
	public class DefaultConstructionModel : IConstructionModel
	{
		private Type m_implementation;
		private ConstructorInfo m_constructor;
		private PropertyInfo[] m_properties;

		public DefaultConstructionModel( Type implementation, ConstructorInfo constructor, PropertyInfo[] properties )
		{
			AssertUtil.ArgumentNotNull( implementation, "implementation" );
			AssertUtil.ArgumentNotNull( constructor, "constructor" );
			AssertUtil.ArgumentNotNull( properties, "properties" );

			m_implementation = implementation;
			m_constructor = constructor;
			m_properties = properties;
		}

		#region IConstructionModel Members

		public Type Implementation
		{
			get
			{
				return m_implementation;
			}
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_implementation = value;
            }
        }

		public ConstructorInfo SelectedConstructor
		{
			get
			{
				return m_constructor;
			}
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_constructor = value;
            }
        }

        public PropertyInfo[] SelectedProperties
		{
			get
			{
				return m_properties;
			}
		}

		#endregion
	}
}
