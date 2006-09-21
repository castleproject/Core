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

namespace Castle.MicroKernel.Tests.RuntimeParameters
{
	using Castle.Core;

	public class CompA
	{
		public CompA()
		{
		}
	}

	[Transient]
	public class CompB
	{
		private string myArgument = string.Empty;
		private CompC compc = null;

		public CompB(CompA ca, CompC cc, string myArgument)
		{
			this.compc = cc;
			this.myArgument = myArgument;
		}

		public CompC Compc
		{
			get { return compc; }
			set { compc = value; }
		}

		public string MyArgument
		{
			get { return myArgument; }
		}
	}

	public class CompC
	{
		public CompC(int test)
		{
		}
	}
}
