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

namespace Castle.Rook.Compiler.TypeGraph
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;


	public class ListValueDictionary : DictionaryBase
	{
		public ListValueDictionary()
		{
		}

		public void Add(String key, object value)
		{
			if (Dictionary.Contains(key))
			{
				object curValue = Dictionary[key];

				Bucket bucket = (curValue as Bucket);

				if (bucket == null)
				{
					bucket = new Bucket(curValue);
					Dictionary[key] = bucket;
				}

				bucket.Add(value);
			}
			else
			{
				Bucket bucket = new Bucket(value);

				Dictionary[key] = bucket;
			}
		}

		public bool Contains(String name)
		{
			return Dictionary.Contains(name);
		}
	}

	public class Bucket : CollectionBase
	{
		public Bucket(object firstItem)
		{
			InnerList.Add(firstItem);
		}

		public void Add(object item)
		{
			InnerList.Add(item);
		}
	}
}
