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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// This class is used just to minimize the number of params being passed between
	/// calls, also to make it easier to make modifications in the future,
	/// notice that the recursive nature of the algorithm prevent us from adding all
	/// params here unless we implement some kind of stack. 
	/// </summary>
	public class DataBindContext
	{
		public NameValueCollection ParamList; 
		public IDictionary Files; 
		public IList Errors;
		public String[] ExcludedProperties;
		public String[] AllowedProperties;
		public String Root;
			
		public DataBindContext(String root, NameValueCollection paramList, IDictionary files, 
		                       IList errorList, String[] excludedProperties, String[] allowedProperties)
		{
			Root = root;
			ParamList = paramList;
			Files = files;
			Errors = errorList;
			ExcludedProperties = excludedProperties;
			AllowedProperties = allowedProperties;
		}
	}
}
