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

namespace NVelocity.Util.Introspection
{
	using System;

	/// <summary>
	/// Holds information for node-local context data introspection
	/// information.
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: IntrospectionCacheData.cs,v 1.3 2003/10/27 13:54:12 corts Exp $ </version>
	public class IntrospectionCacheData
	{
		public IntrospectionCacheData()
		{
		}

		public IntrospectionCacheData(Type contextData, object thingy)
		{
			Thingy = thingy;
			ContextData = contextData;
		}

		/// <summary>
		/// Object to pair with class - currently either a Method or
		/// AbstractExecutor. It can be used in any way the using node
		/// wishes.
		/// </summary>
		public Object Thingy;

		/// <summary>
		/// Class of context data object associated with the 
		/// introspection information
		/// </summary>
		public Type ContextData;
	}
}