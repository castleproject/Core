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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System.Collections;
	using System.Reflection;
	using System.Text;

	/// <summary>
	/// Generate xml from assembly level attributes.
	/// This is useful if we need to have type-less configuration, such as imports, named queries, etc.
	/// </summary>
	public class AssemblyXmlGenerator
	{
		private StringBuilder xml;
		/// <summary>
		/// Create a new instnace
		/// </summary>
		public AssemblyXmlGenerator()
		{
			Reset();
		}

		/// <summary>
		/// Reset this generator and prepare to generate xml from new assembly.
		/// </summary>
		private void Reset()
		{
			xml = new StringBuilder();
		}

		/// <summary>
		/// Generate XML from assembly attributes.
		/// If it can't find relevant attributes, returns null.
		/// </summary>
		public string[] CreateXmlConfigurations(Assembly assembly)
		{
			object[] atts = assembly.GetCustomAttributes(true);
			ArrayList namedHqlQueries = new ArrayList();
			ArrayList namedSqlQueries = new ArrayList();
			ArrayList imports = new ArrayList();
			ArrayList rawXml = new ArrayList();
			foreach (object attribute in atts)
			{
				if (attribute is HqlNamedQueryAttribute)
				{
					namedHqlQueries.Add(attribute);
				}
				else if (attribute is SqlNamedQueryAttribute)
				{
					namedSqlQueries.Add(attribute);
				}
				else if (attribute is ImportAttribute)
				{
					imports.Add(attribute);
				}
				else if (attribute is RawXmlMappingAttribute)
				{
					string[] result = ((RawXmlMappingAttribute)attribute).GetMappings();
					rawXml.AddRange(result);
				}
			}
			xml.Append(Constants.XmlPI);
			xml.AppendFormat(Constants.XmlHeader, "", "");
			//note that there is a meaning to the order of import vs. named queries, imports must come first.
			foreach (ImportAttribute attribute in imports)
			{
				AppendImport(attribute);
			}
			foreach (HqlNamedQueryAttribute attribute in namedHqlQueries)
			{
				AppendNamedHqlQuery(attribute, assembly);
			}
			foreach (SqlNamedQueryAttribute attribute in namedSqlQueries)
			{
				AppendNamedSqlQuery(attribute, assembly);
			}
			xml.AppendLine(Constants.XmlFooter);
			bool hasQueriesOrImportsToAdd = namedHqlQueries.Count != 0 || namedSqlQueries.Count != 0 || imports.Count != 0;
			if (hasQueriesOrImportsToAdd)
			{
				rawXml.Insert(0,xml.ToString());
			}
			Reset();
			return (string[])rawXml.ToArray(typeof(string));
		}

		private void AppendImport(ImportAttribute attribute)
		{
			xml.AppendFormat("<import class=\"{0}\" rename=\"{1}\"/>", XmlGenerationVisitor.MakeTypeName(attribute.Type), attribute.Rename);
		}

		private void AppendNamedHqlQuery(HqlNamedQueryAttribute attribute, Assembly assembly)
		{
			if (attribute.Name == "" || attribute.Query == "")
			{
				throw new ActiveRecordException("Error generating XML for HqlNamedQuery in " + assembly.FullName +
												". Query must have both name and query.");
			}

			xml.AppendFormat(@"
	<query name='{0}'>
		 <![CDATA[{1}]]>
	 </query>
", attribute.Name, attribute.Query);

		}

		private void AppendNamedSqlQuery(SqlNamedQueryAttribute attribute, Assembly assembly)
		{
			if (attribute.Name == "" || attribute.Query == "")
			{
				throw new ActiveRecordException("Error generating XML for SqlNamedQuery in " + assembly.FullName +
												". Query must have both name and query.");
			}

			xml.AppendFormat(@"
	<sql-query name='{0}'>
		 <![CDATA[{1}]]>
	 </sql-query>
", attribute.Name, attribute.Query);

		}
	}
}
