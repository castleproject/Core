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

namespace Castle.DynamicProxy.Test.Mixins
{
	using System;

	public interface IFirst
	{
		void DoFirst();
	}

	public interface ISecond : IFirst
	{
		void DoSecond();
	}

	public interface IThird : ISecond
	{
		void DoThird();
	}

	/// <summary>
	/// Summary description for ComplexMixin.
	/// </summary>
	[Serializable]
	public class ComplexMixin : IThird
	{
		public ComplexMixin()
		{
		}

		public void DoThird()
		{
		}

		public void DoSecond()
		{
		}

		public void DoFirst()
		{
		}
	}
}
