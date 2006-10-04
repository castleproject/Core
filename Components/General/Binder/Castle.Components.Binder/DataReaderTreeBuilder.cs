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

namespace Castle.Components.Binder
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;

	/// <summary>
	/// 
	/// </summary>
	public class DataReaderTreeBuilder
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public CompositeNode BuildSourceNode(IDataReader reader, String prefix)
		{
			CompositeNode root = new CompositeNode("root");
			
			PopulateTree(root, reader, prefix);
			
			return root;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="reader"></param>
		/// <param name="prefix"></param>
		public void PopulateTree(CompositeNode root, IDataReader reader, String prefix)
		{
			string[] fields = GetFields(reader);

			int[] indexesToSkip = FindDuplicateFields(fields);
			
			IndexedNode indexNode = new IndexedNode(prefix);
			
			int row = 0;

			while(reader.Read())
			{
				CompositeNode node = new CompositeNode(row.ToString());

				for(int i=0; i<reader.FieldCount; i++)
				{
					// Is in the skip list?
					if (Array.IndexOf(indexesToSkip, i) >= 0) continue;
					
					// Is null?
					if (reader.IsDBNull(i)) continue;
					
					Type fieldType = reader.GetFieldType(i);
					
					node.AddChildNode(new LeafNode(fieldType, fields[i], reader.GetValue(i)));
				}

				indexNode.AddChildNode(node);
			
				row++;
			}
			
			root.AddChildNode(indexNode);
		}

		private string[] GetFields(IDataReader reader)
		{
			String[] fields = new String[reader.FieldCount];
	
			for(int i=0; i< reader.FieldCount; i++)
			{
				fields[i] = reader.GetName(i);
			}

			return fields;
		}
		
		/// <summary>
		/// Check the fields for duplicates.
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		/// <remarks>
		/// I have to add this check as some stored procedures
		/// return duplicate columns (doh!) and this isn't good
		/// for the binder.
		/// </remarks>
		private int[] FindDuplicateFields(string[] fields)
		{
			HybridDictionary dict = new HybridDictionary(fields.Length, true);
			ArrayList duplicateList = new ArrayList();
			
			for(int i=0; i < fields.Length; i++)
			{
				String field = fields[i];
				
				if (dict.Contains(field))
				{
					duplicateList.Add(i);
					continue;
				}	
				
				dict.Add(field, String.Empty);
			}
			
			return (int[]) duplicateList.ToArray(typeof(int));
		}
	}
}
