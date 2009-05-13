// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or Chainied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MicroKernel.Tests.ClassComponents
{
	using System;
	
	public class CustomerChain1 : CustomerImpl
	{
		public ICustomer CustomerBase;

		public CustomerChain1(ICustomer customer)
		{
			CustomerBase = customer;
		}
	}

	public class CustomerChain2 : CustomerChain1
	{
		public CustomerChain2(ICustomer customer) : base(customer)
		{
		}
	}

	public class CustomerChain3 : CustomerChain1
	{
		public CustomerChain3(ICustomer customer) : base(customer)
		{
		}
	}

	public class CustomerChain4 : CustomerChain1
	{
		public CustomerChain4(ICustomer customer) : base(customer)
		{
		}
	}

	public class CustomerChain5 : CustomerChain1
	{
		public CustomerChain5(ICustomer customer) : base(customer)
		{
		}
	}

	public class CustomerChain6 : CustomerChain1
	{
		public CustomerChain6(ICustomer customer) : base(customer)
		{
		}
	}

	public class CustomerChain7 : CustomerChain1
	{
		public CustomerChain7(ICustomer customer) : base(customer)
		{
		}
	}

	[Serializable]
	public class CustomerChain8 : CustomerChain1
	{
		public CustomerChain8(ICustomer customer) : base(customer)
		{
		}
	}

	[Serializable]
	public class CustomerChain9 : CustomerChain1
	{
		public CustomerChain9(ICustomer customer) : base(customer)
		{
		}
	}
}
