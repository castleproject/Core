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

namespace Castle.Components.Binder
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	public class NameValueCollectionAdapter : AbstractBindingDataSource
	{
		protected readonly NameValueCollection inner;

		protected NameValueCollectionAdapter()
		{
			this.inner = new NameValueCollection();
		}

		public NameValueCollectionAdapter(NameValueCollection inner)
		{
			if (inner == null) throw new ArgumentNullException("inner");

			this.inner = inner;
		}

		protected override String[] AllKeys
		{
			get { return inner.AllKeys; }
		}

		public override String GetEntryValue(String name)
		{
			return inner[name];
		}

		public override Object GetEntryValue(String name, Type desiredType, out bool conversionSucceeded)
		{
			throw new NotImplementedException();
		}

		public override String GetMetaEntryValue(String name)
		{
			throw new NotImplementedException();
		}

		public override object this[object key]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

	}
}
