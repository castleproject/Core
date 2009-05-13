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

namespace NVelocity.Context
{
	using System;
	using System.Collections;

	/// <summary>  interface to bring all necessary internal and user contexts together.
	/// this is what the AST expects to deal with.  If anything new comes
	/// along, add it here.
	/// *
	/// I will rename soon :)
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: InternalContextAdapter.cs,v 1.3 2003/10/27 13:54:08 corts Exp $
	///
	/// </version>
	public interface IInternalContextAdapter : IInternalHousekeepingContext, IContext, IInternalWrapperContext,
	                                           IInternalEventContext, IDictionary
	{
		/// <summary>
		/// Need to define this method here otherwise since both <see cref="IDictionary"/> and <see cref="IContext"/> 
		/// contains a Remove(Object key) method we will need to cast the object to either interface
		/// before calling this method, for backward compatibility we make the IContext.Remove the default
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		new Object Remove(object key);
	}
}