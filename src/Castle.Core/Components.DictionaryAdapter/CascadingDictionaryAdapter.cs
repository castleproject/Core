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

namespace Castle.Components.DictionaryAdapter
{
	using System.Collections;

	public class CascadingDictionaryAdapter : AbstractDictionaryAdapter
	{
		private readonly IDictionary primary;
		private readonly IDictionary secondary;

		public CascadingDictionaryAdapter(IDictionary primary, IDictionary secondary)
		{
			this.primary = primary;
			this.secondary = secondary;
		}

		public IDictionary Primary
		{
			get { return primary; }
		}

		public IDictionary Secondary
		{
			get { return secondary; }
		}

		public override bool IsReadOnly
		{
			get { return primary.IsReadOnly; }
		}

		public override bool Contains(object key)
		{
			return primary.Contains(key) || secondary.Contains(key);
		}

		public override object this[object key]
		{
			get { return primary[key] ?? secondary[key]; }
			set { primary[key] = value; }
		}
	}
}
