// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Test.Mixins
{
	using System;

	public interface ISimpleMixin
	{
		event EventHandler MyEvent;
		String Name { get; set; }
		int DoSomething();
	}

	/// <summary>
	/// Summary description for SimpleMixin.
	/// </summary>
	[Serializable]
	public class SimpleMixin : ISimpleMixin
	{
		private String name;
		public event EventHandler MyEvent;

		public SimpleMixin()
		{
			name = String.Empty;
		}

		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		public int DoSomething()
		{
			if (MyEvent != null)
			{
				MyEvent(this, EventArgs.Empty);
			}
			return 1;
		}
	}
}
