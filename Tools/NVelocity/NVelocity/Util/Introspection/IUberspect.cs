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

	/// <summary> 'Federated' introspection/reflection interface to allow the introspection
	/// behavior in Velocity to be customized.
	/// *
	/// </summary>
	/// <author>  <a href="mailto:geirm@apache.org">Geir Magusson Jr.</a>
	/// </author>
	/// <version>  $Id: Uberspect.cs,v 1.1 2004/12/27 05:55:08 corts Exp $
	/// 
	/// </version>
	public interface IUberspect
	{
		/// <summary>  Initializer - will be called before use
		/// </summary>
		void Init();

		/// <summary>  Returns a general method, corresponding to $foo.bar( $woogie )
		/// </summary>
		IVelMethod GetMethod(Object obj, String method, Object[] args, Info info);

		/// <summary> Property getter - returns VelPropertyGet appropos for #set($foo = $bar.woogie)
		/// </summary>
		IVelPropertyGet GetPropertyGet(Object obj, String identifier, Info info);

		/// <summary> Property setter - returns VelPropertySet appropos for #set($foo.bar = "geir")
		/// </summary>
		IVelPropertySet GetPropertySet(Object obj, String identifier, Object arg, Info info);
	}
}