// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Property|AttributeTargets.Method)]
	public class MyAttribute : Attribute
	{
		private string _name;

		public MyAttribute(String name)
		{
			_name = name;
		}

		public string name
		{
			get { return _name; }
		}
	}

	[MyAttribute("class attribute")]
	public class ServiceStatusImpl : IServiceStatus
	{
		private State _state = State.Invalid;

		public ServiceStatusImpl()
		{
		}

		[MyAttribute("Requests")]
		public int Requests
		{
			get { return 10; }
		}

		[MyAttribute("ActualState")]
		public State ActualState
		{
			get { return _state; }
		}

		[MyAttribute("ChangeState")]
		public void ChangeState(State state)
		{
			_state = state;
		}
	}
}