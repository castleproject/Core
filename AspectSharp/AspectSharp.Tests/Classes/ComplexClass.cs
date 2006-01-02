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

namespace AspectSharp.Tests.Classes
{
	using System;

	/// <summary>
	/// Summary description for ComplexClass.
	/// </summary>
	[Serializable]
	public class ComplexClass 
	{
		private int _doMethodExecutionCount;
		private String _name;
		private bool _started;

		public ComplexClass()
		{
		}

		public ComplexClass(string name)
		{
			_name = name;
		}

		public ComplexClass(string name, bool started)
		{
			_name = name;
			_started = started;
		}

		public virtual void DoNothing()
		{
		}

		public virtual int DoSomething()
		{
			return 1;
		}

		public virtual int DoSomething(int value1)
		{
			return value1;
		}

		public virtual int DoSomething(int value1, String text)
		{
			return value1;
		}

		public virtual int Pong(ref int i)
		{
			return i;
		}

		public virtual int DoMethodExecutionCount
		{
			get { return _doMethodExecutionCount; }
			set { _doMethodExecutionCount = value; }
		}

		public virtual String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual bool Started
		{
			get { return _started; }
			set { _started = value; }
		}
	}
}
