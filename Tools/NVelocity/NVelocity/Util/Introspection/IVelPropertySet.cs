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

	/// <summary>  Interface used for setting values that appear to be properties in
	/// Velocity.  Ex.
	/// *
	/// #set($foo.bar = "hello")
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version>  $Id: VelPropertySet.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// 
	/// </version>
	public interface IVelPropertySet
	{
		/// <summary>  specifies if this VelPropertySet is cacheable and able to be
		/// reused for this class of object it was returned for
		/// *
		/// </summary>
		/// <returns> true if can be reused for this class, false if not
		/// 
		/// </returns>
		bool Cacheable { get; }

		/// <summary>  returns the method name used to set this 'property'
		/// </summary>
		String MethodName { get; }

		/// <summary>  method used to set the value in the object
		/// *
		/// </summary>
		/// <param name="o">Object on which the method will be called with the arg
		/// </param>
		/// <param name="arg">value to be set
		/// </param>
		/// <returns> the value returned from the set operation (impl specific)
		/// 
		/// </returns>
		Object Invoke(Object o, Object arg);
	}
}