// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Helpers
{
	using Services;

	/// <summary>
	/// Provides utilities methods to work with JSON.
	/// </summary>
	public class JSONHelper : AbstractHelper
	{
		/// <summary>
		/// Converts a instance of the model to its JSON representation.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>The JSON representation of the model.</returns>
		/// <example>
		/// Suppose you have a car object instance, like this one:
		/// <code>
		/// Car car = new Car();
		/// 
		/// car.Wheels = 4;
		/// car.Model = "Cheap";
		/// car.Year = 2007;
		/// </code>
		/// And to transform it to JSON, you must invoke the method passing the instance.
		/// <code>
		/// $helper.ToJSON(car)
		/// </code>
		/// Which will generate the JSON string:
		/// <code>
		/// {Wheels=4,Year=2007,Model='Cheap'}
		/// </code>
		/// </example>
		public string ToJSON(object model)
		{
			IJSONSerializer serializer = Context.Services.JSONSerializer;

			if (serializer == null)
			{
				throw new MonoRailException("Attempt to serialize object failed because the serializer is not available");
			}

			return serializer.Serialize(model);
		}
	}
}
