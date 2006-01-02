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

namespace NVelocity.Exception
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>  Application-level exception thrown when a resource of any type
	/// has a syntax or other error which prevents it from being parsed.
	/// <br>
	/// When this resource is thrown, a best effort will be made to have
	/// useful information in the exception's message.  For complete
	/// information, consult the runtime log.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ParseErrorException.cs,v 1.3 2003/10/27 13:54:08 corts Exp $
	///
	/// </version>
	[Serializable]
	public class ParseErrorException : VelocityException
	{
		public ParseErrorException(String exceptionMessage) : base(exceptionMessage)
		{
		}

		public ParseErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}