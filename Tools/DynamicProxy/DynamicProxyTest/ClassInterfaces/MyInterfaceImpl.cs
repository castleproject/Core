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

namespace Apache.Avalon.DynamicProxy.Test.ClassInterfaces
{
	using System;

	/// <summary>
	/// Summary description for MyInterfaceImpl.
	/// </summary>
	public class MyInterfaceImpl : IMyInterface
	{
		private String m_name;
		private bool m_started;

		public MyInterfaceImpl()
		{
		}

		#region IMyInterface Members

		public String Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public bool Started
		{
			get
			{
				return m_started;
			}
			set
			{
				m_started = value;
			}
		}

		public virtual int Calc(int x, int y)
		{
			return x + y;
		}

		public virtual int Calc(int x, int y, int z, Single k)
		{
			return x + y + z + (int)k;
		}

		#endregion
	}
}
