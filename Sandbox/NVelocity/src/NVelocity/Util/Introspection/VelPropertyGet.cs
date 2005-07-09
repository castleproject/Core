namespace NVelocity.Util.Introspection
{
	using System;
	/*
	* Copyright 2002-2004 The Apache Software Foundation.
	*
	* Licensed under the Apache License, Version 2.0 (the "License")
	* you may not use this file except in compliance with the License.
	* You may obtain a copy of the License at
	*
	*     http://www.apache.org/licenses/LICENSE-2.0
	*
	* Unless required by applicable law or agreed to in writing, software
	* distributed under the License is distributed on an "AS IS" BASIS,
	* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	* See the License for the specific language governing permissions and
	* limitations under the License.
	*/

	/// <summary>  Interface defining a 'getter'.  For uses when looking for resolution of
	/// property references
	/// *
	/// $foo.bar
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version>  $Id: VelPropertyGet.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// 
	/// </version>
	public interface VelPropertyGet
	{
		/// <summary>  specifies if this VelPropertyGet is cacheable and able to be
		/// reused for this class of object it was returned for
		/// *
		/// </summary>
		/// <returns> true if can be reused for this class, false if not
		/// 
		/// </returns>
		bool Cacheable { get; }

		/// <summary>  returns the method name used to return this 'property'
		/// </summary>
		String MethodName { get; }

		/// <summary>  invocation method - called when the 'get action' should be
		/// preformed and a value returned
		/// </summary>
		Object invoke(Object o);
	}
}