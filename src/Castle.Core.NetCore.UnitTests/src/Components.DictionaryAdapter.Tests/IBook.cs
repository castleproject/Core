﻿// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

#if !SILVERLIGHT && !NETCORE && !DOTNET35
	public interface IBook
	{
		IDeweyDecimalNumber DDC { get; set; }

		string Title { get; set; }

		bool Printed { get; set; }

		ISet<IBook> RelatedBooks { get; set; }
	}
#endif

	public interface IDeweyDecimalNumber
	{
		int Category { get; set; }

		int SubCategory { get; set; }

		int SubDivision { get; set; }
	}
}