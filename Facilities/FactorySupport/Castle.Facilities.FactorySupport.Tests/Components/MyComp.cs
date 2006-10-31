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

namespace Castle.Facilities.FactorySupport.Tests.Components
{
	using System;
	using System.Collections;


	public class MyComp
	{
		private IMyService serv;
		private string storeName;
		private IDictionary props;

		internal MyComp()
		{
		}

		internal MyComp(IMyService serv)
		{
			this.serv = serv;
		}

		internal MyComp(String storeName, IDictionary props)
		{
			this.storeName = storeName;
			this.props = props;
		}

		public IMyService Service
		{
			get { return serv; }
		}

		public string StoreName
		{
			get { return storeName; }
		}

		public IDictionary Props
		{
			get { return props; }
		}
	}
}
