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

	/// <summary>
	/// Summary description for DefaultDependencyModel.
	/// </summary>
	public class DefaultDependencyModel : IDependencyModel
	{
		private Type m_service;

		private bool m_optional;

		private String m_key;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="service"></param>
		/// <param name="key"></param>
		/// <param name="optional"></param>
		public DefaultDependencyModel( Type service, String key, bool optional )
		{
			AssertUtil.ArgumentNotNull( service, "service" );

			m_service = service;
			m_optional = optional;
			m_key = key;
		}

		#region IDependencyModel Members

		public Type Service
		{
			get
			{
				return m_service;
			}
		}

		public String LookupKey
		{
			get
			{
				return m_key;
			}
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_key = value;
            }        
        }

		public bool Optional
		{
			get
			{
				return m_optional;
			}
            set
            {
                AssertUtil.ArgumentNotNull(value, "value");
                m_optional = value;
            }
        }

		#endregion
	
		public override bool Equals(object obj)
		{
			DefaultDependencyModel other = obj as DefaultDependencyModel;
			if (other == null)
			{
				return false;
			}
			return other.Service.Equals( m_service );
		}
	
		public override int GetHashCode()
		{
			return m_service.GetHashCode();
		}
	}
}
