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
	public class ConfigurationExceptionTest
	{
		[Test] 
		public void Constructor()
		{
			Assertion.AssertNotNull( new ConfigurationException() );
			Assertion.AssertNotNull( new ConfigurationException( "message" ) );
			Assertion.AssertNotNull( new ConfigurationException( "message", new Exception() ) );
		}

		[Test] 
		public void Message()
		{
			ConfigurationException ce = new ConfigurationException("message");
			Assertion.AssertEquals( "message", ce.Message );
			Assertion.AssertNull( ce.InnerException );

			Exception inner = new Exception("inner");
			ce = new ConfigurationException("message", inner);
			Assertion.AssertEquals("message", ce.Message);
			Assertion.AssertEquals( inner, ce.InnerException );
			Assertion.AssertEquals( "inner", ce.InnerException.Message );
		}
	}
}
