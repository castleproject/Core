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


	public class ResourceImpl : IResource
	{
		private bool _started;
		private bool _rolledback;
		private bool _committed;

		public ResourceImpl()
		{
		}

		public bool Started
		{
			get { return _started; }
		}

		public bool Rolledback
		{
			get { return _rolledback; }
		}

		public bool Committed
		{
			get { return _committed; }
		}

		#region IResource Members

		public void Start()
		{
			if (_started) throw new ApplicationException("Start called before");

			_started = true;
		}

		public void Rollback()
		{
			if (_rolledback) throw new ApplicationException("Rollback called before");

			_rolledback = true;
		}

		public void Commit()
		{
			if (_committed) throw new ApplicationException("Commit called before");

			_committed = true;
		}

		#endregion
	}
}
