// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Generator.Components.Utils
{
	using System;


	public class Pair
	{
		private String _first;
		private String _second;

		public Pair(String first, String second)
		{
			_first = first;
			_second = second;
		}

		public String First
		{
			get { return _first; }
		}

		public String Second
		{
			get { return _second; }
		}

		public override String ToString()
		{
			return _first;
		}
	}
}
