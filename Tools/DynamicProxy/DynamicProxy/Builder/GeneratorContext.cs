// Copyright 2004 The Apache Software Foundation
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

namespace Apache.Avalon.DynamicProxy
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for GeneratorContext.
	/// </summary>
	public sealed class GeneratorContext : DictionaryBase
	{
		private EnhanceTypeDelegate m_enhance;
		private ScreenInterfacesDelegate m_screenInterfaces;
		private IList m_skipInterfaces = new ArrayList();

		public GeneratorContext()
		{
		}

		public GeneratorContext(EnhanceTypeDelegate enhanceDelegate, 
			ScreenInterfacesDelegate screenDelegate)
		{
			m_enhance = enhanceDelegate;
			m_screenInterfaces = screenDelegate;
		}

		public EnhanceTypeDelegate EnhanceType
		{
			get { return m_enhance; }
			set { m_enhance = value; }
		}

		public ScreenInterfacesDelegate ScreenInterfaces
		{
			get { return m_screenInterfaces; }
			set { m_screenInterfaces = value; }
		}

		public bool ShouldSkip( Type interfaceType )
		{
			return m_skipInterfaces.Contains( interfaceType );
		}

		public void AddInterfaceToSkip( Type interfaceType )
		{
			m_skipInterfaces.Add( interfaceType );
		}

		public object this[ String key ]
		{
			get { return Dictionary[key]; }
			set { Dictionary[key] = value; }
		}
	}
}
