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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;

	public interface IConversions
	{
		int Int { get; set; }
		float Float { get; set; }
		double Double { get; set; }
		decimal Decimal { get; set; }
		String String { get; set; }
		DateTime DateTime { get; set; }
		[NewGuid]Guid Guid { get; set; }
		int? NullInt { get; set; }
		float? NullFloat { get; set; }
		double? NullDouble { get; set; }
		DateTime? NullDateTime { get; set; }
		Guid? NullGuid { get; set; }
		decimal? NullDecimal { get; set; }
	}
}