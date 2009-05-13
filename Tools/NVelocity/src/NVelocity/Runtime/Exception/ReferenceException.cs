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

namespace NVelocity.Runtime.Exception
{
	using System;
	using System.Runtime.Serialization;
	using NVelocity.Runtime.Parser.Node;

	/// <summary> Exception thrown when a bad reference is found.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ReferenceException.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	[Serializable]
	public class ReferenceException : Exception
	{
		public ReferenceException(String exceptionMessage, INode node)
			: base(
				string.Format("{0} [line {1},column {2}] : {3} is not a valid reference.", exceptionMessage, node.Line, node.Column,
				              node.Literal))
		{
		}

		public ReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}