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

namespace Castle.ActiveRecord.Generator
{
	using System;


	public abstract class ImageConstants
	{
		public static readonly int Database_Catalog = 0;

		public static readonly int Database_Schemas = 1;

		public static readonly int Database_Schema = 2;

		public static readonly int Database_Tables = 3;

		public static readonly int Database_Table = 4;

		public static readonly int Database_Fields = 5;

		public static readonly int Database_Field = 6;

		public static readonly int Database_Views = 7;

		public static readonly int Classes_Entities = 8;

		public static readonly int Classes_Entity = 9;

		public static readonly int Classes_Property = 10;

		public static readonly int Classes_Operation = 11;
		
		public static readonly int Classes_Operation_Protected = 12;
		
		public static readonly int Classes_Private_Field = 13;
		
		public static readonly int Classes_Namespace = 15;
	}
}
