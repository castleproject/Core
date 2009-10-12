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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if !SILVERLIGHT
namespace Castle.Core.Tests
{
	using System.Web;
	using NUnit.Framework;
	using Castle.Core.Logging;

	[TestFixture]
	public class WebLoggerTests
	{
		[Test]
		public void When_there_is_no_Current_Http_Context_WebLogger_should_just_silently_do_nothing()
		{
			new StubWebLoggerWithNullTraceContext().Debug("this shouldn't cause an exception");
		}
	}

	public class StubWebLoggerWithNullTraceContext : WebLogger
	{
		protected override TraceContext TryToGetTraceContext()
		{
			return null;
		}
	}
}
#endif
