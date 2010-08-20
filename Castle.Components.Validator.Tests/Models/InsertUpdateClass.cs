// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator.Tests.Models
{
	public class InsertUpdateClass
	{
		private string prop1, prop2, prop3, prop4;

		[ValidateNonEmpty(FriendlyName="Property 1", RunWhen=RunWhen.Insert)]
		public string Prop1
		{
			get { return prop1; }
			set { prop1 = value; }
		}

		[ValidateNonEmpty(FriendlyName = "Property 2", RunWhen = RunWhen.Insert)]
		public string Prop2
		{
			get { return prop2; }
			set { prop2 = value; }
		}

		[ValidateNonEmpty(FriendlyName = "Property 3", RunWhen = RunWhen.Update)]
		public string Prop3
		{
			get { return prop3; }
			set { prop3 = value; }
		}

		[ValidateNonEmpty(FriendlyName = "Property 4", RunWhen = RunWhen.Update)]
		public string Prop4
		{
			get { return prop4; }
			set { prop4 = value; }
		}
	}
}