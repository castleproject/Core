using System.Text.RegularExpressions;
using Castle.Facilities.ActiveRecordGenerator.Utils;
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

namespace Castle.Facilities.ActiveRecordGenerator.CodeGenerator.Default
{
	using System;


	public class NamingService : INamingService
	{
		private Pair[] singularRules = new Pair[] 
			{
				new Pair("(x|ch|ss)es$", @"$1"), 
				new Pair("movies$", "movie"),
				new Pair("([^aeiouy]|qu)ies$", "$1y"),
				new Pair("([lr])ves$", @"$1f"),
				new Pair("([^f])ves$", @"$1fe"),
				new Pair("(analy|ba|diagno|parenthe|progno|synop|the)ses$", @"$1sis"),
				new Pair("([ti])a$", @"$1um"),
				new Pair("people$", @"person"),
				new Pair("men$", @"man"),
				new Pair("status$", @"status"),
				new Pair("children$", @"child"),
				new Pair("s$", @"")
			};

		public NamingService()
		{
		}

		public String CreateRelationName(String referencedTableName)
		{
			throw new NotImplementedException();
		}

		public String CreateClassName(String tableName)
		{
			tableName = Regex.Replace(tableName, "(tb_|_)", "");

			return Singularize(tableName);
		}

		public String CreatePropertyName(String columnName)
		{
			MatchEvaluator UpCaser = new MatchEvaluator(UpCaserDef);

			columnName = Regex.Replace(columnName, "([a-z])_([a-z])", UpCaser);
			columnName = Regex.Replace(columnName, "(tb_|_)", "");
			columnName = Regex.Replace(columnName, "^[a-z]", UpCaser);

			return columnName;
		}

		public String CreateFieldName(String columnName)
		{
			MatchEvaluator UpCaser = new MatchEvaluator(UpCaserDef);
			MatchEvaluator DownCaser = new MatchEvaluator(DownCaserDef);

			columnName = Regex.Replace(columnName, "([a-z])_([a-z])", UpCaser);
			columnName = Regex.Replace(columnName, "(tb_|_)", "");
			columnName = Regex.Replace(columnName, "^[A-Z]",  DownCaser);

			return columnName;
		}

		private String UpCaserDef(Match match)
		{
			if (match.Value.Length == 1)
			{
				return match.Value.ToUpper();
			}

			return String.Format( "{0}_{1}", match.Value[0], match.Value.ToUpper()[2] );
		}

		private String DownCaserDef(Match match)
		{
			return match.Value.ToLower();
		}

		private string Singularize(string name)
		{
			foreach(Pair pair in singularRules)
			{
				String result = Regex.Replace(name, pair.First, pair.Second);
				if (result != name)
				{
					return result;
				}
			}

			return name; 
		}
	}
}
