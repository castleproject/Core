#region Copyright
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
#endregion

namespace Castle.Windsor.Tests.Adapters.ComponentModel
{
	using System;
	using System.ComponentModel;

	public class TestComponent : Component
	{
		private bool _sited = false;
		private bool _disposed = false;

		public bool IsSited
		{
			get { return _sited; }
		}

		public bool IsDisposed
		{
			get { return _disposed; }
		}

		public override ISite Site
		{
			set
			{ 
				base.Site = value;
				_sited = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.Dispose( disposing );

				_disposed = true;
			}
		}
	}
}
