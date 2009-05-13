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

namespace NVelocity.Tool
{
	using System;

	/// <summary>
	/// <p>A view tool that allows template designers to load
	/// an arbitrary object into the context. Any object
	/// with a public constructor without parameters can be used
	/// as a view tool.</p>
	/// <p>THIS CLASS IS HERE AS A PROOF OF CONCEPT ONLY. IT IS NOT
	/// INTENDED FOR USE IN PRODUCTION ENVIRONMENTS. USE AT YOUR OWN RISK.</p>
	/// </summary>
	/// <author><a href="mailto:sidler@teamup.com">Gabe Sidler</a></author>
	/// <author><a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
	public class ToolLoader
	{
		/// <summary>
		/// Creates and returns an object of the specified classname.
		/// The object must have a valid default constructor.
		/// </summary>
		/// <param name="className">the fully qualified class name of the object</param>
		/// <returns>an instance of the specified class or null if the class
		/// could not be instantiated.</returns>
		public Object Load(String className)
		{
			try
			{
				Type type = Type.GetType(className);
				Object o = Activator.CreateInstance(type);
				return o;
			}
			catch(Exception)
			{
				return null;
			}
		}
	}
}