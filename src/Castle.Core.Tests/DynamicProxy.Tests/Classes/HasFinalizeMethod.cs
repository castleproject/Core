// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.Classes
{
	public class HasFinalizeMethod
	{
#pragma warning disable 0465, 0114 //Introducing a 'Finalize' method can interfere with destructor invocation. Did you intend to declare a destructor?	e:\OSS.Code\Castle.Core\src\Castle.Core.Tests\DynamicProxy.Tests\Classes\HasFinalizeMethod.cs	20	26	Castle.Core.Tests
		protected virtual void Finalize()
		{
		}
#pragma warning restore 0465, 0114
	}
}