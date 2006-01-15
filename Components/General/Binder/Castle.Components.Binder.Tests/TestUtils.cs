// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections.Specialized;

	public class TestUtils
	{
		/// <summary>
		/// Parse a string in this format:
		/// @" 
		/// 		Person@count   = 2
		/// 		Person[0].Name = Gi   Joe
		/// 		Person[0].Age  = 32
		/// 		Person[1].Name = Mary
		/// 		Person[1].Age  = 16
		/// 	";
		/// and return a NameValueCollection with these elements
		/// 
		/// "Person@count"   => "2"
		/// "Person[0].Name" => "Gi   Joe"
		/// "Person[0].Age"  => "32"
		/// "Person[1].Name" => "Mary"
		/// "Person[1].Age"  => "16" 		
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		/// <remarks>
		/// Notice that any that leading and trailing spaces are discarded
		/// </remarks>
		public static NameValueCollection ParseNameValueString(string data)
		{
			NameValueCollection args = new NameValueCollection();
			data = data.Trim();
			foreach(string nameValue in data.Split('\n'))
			{
				if (nameValue.Trim() == "") continue;

				string[] pair = nameValue.Split('=');
				args.Add(pair[0].Trim(), pair[1].Trim());
			}
			return args;
		}
	}
}
