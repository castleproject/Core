// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework.Test
{
	using System;
	using System.Runtime.Serialization;
	using NUnit.Framework;
	using Apache.Avalon.Framework;
	
	[TestFixture]
	public class LookupExceptionTestCase
	{
		[Test] 
		public void Constructor()
		{
			Assertion.AssertNotNull( new LookupException() );
			Assertion.AssertNotNull( new LookupException( "message" ) );
			Assertion.AssertNotNull( new LookupException( "role", "message" ) );
			Assertion.AssertNotNull( new LookupException( "message", new Exception() ) );
			Assertion.AssertNotNull( new LookupException( "role", "message", new Exception() ) );
		}

		[Test] 
		public void Message()
		{
			LookupException le = new LookupException( "role", "message" );
			Assertion.AssertEquals( "Component for role 'role' could not be resolved. Detailed message: message", le.Message );
			Assertion.AssertNull( le.InnerException );

			Exception inner = new Exception( "inner" );
			le = new LookupException( "message", inner );
			Assertion.AssertEquals( "Component for role '' could not be resolved. Detailed message: message", le.Message );
			Assertion.AssertEquals( inner, le.InnerException );
			Assertion.AssertEquals( "inner", le.InnerException.Message );
		}
	}
}
