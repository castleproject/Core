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

namespace Castle.DynamicProxy.Test.ClassInterfaces
{
	using System;

	/// <summary>
	/// Summary description for MyInterfaceImpl.
	/// </summary>
	[Serializable]
	public class MyInterfaceImpl : IMyInterface
	{
		private String _name;
		private bool _started;

		public MyInterfaceImpl()
		{
		}

		#region IMyInterface Members

		public virtual String Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public virtual bool Started
		{
			get
			{
				return _started;
			}
			set
			{
				_started = value;
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
