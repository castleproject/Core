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

namespace Castle.Services.Transaction.Tests
{
	using System;


	public class SynchronizationImpl : ISynchronization
	{
		private DateTime _after = DateTime.MinValue;
		private DateTime _before = DateTime.MinValue;

		public SynchronizationImpl()
		{
		}

		public DateTime After
		{
			get { return _after; }
		}

		public DateTime Before
		{
			get { return _before; }
		}

		public void AfterCompletion()
		{
			_after = DateTime.Now;
		}

		public void BeforeCompletion()
		{
			_before = DateTime.Now;
		}
	}
}
