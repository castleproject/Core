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

namespace AspectSharp.Lang.Steps.Semantic
{
	using System;
	using System.Collections;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for SemanticAnalizerBase.
	/// TODO: Rewrite using the DepthFirstVisitor
	/// </summary>
	public abstract class SemanticAnalizerBase : AbstractStep
	{
		protected void AssertUnique(IDictionary values, NodeBase node, object value, String message)
		{
			if (values.Contains(value))
			{
				RaiseErrorEvent( node.LexicalInfo, message );
			}
			else
			{
				values.Add(value, String.Empty);
			}
		}

		protected void AssertEntriesAreValid(IDictionary types)
		{
			foreach(DictionaryEntry entry in types)
			{
				DefinitionBase type = entry.Value as DefinitionBase;
				AssertNotEmpty( type, entry.Key as String, "A key must be specified to identify the type in the map" );
			}
		}

		protected void AssertNotNull( NodeBase node, object value, String message )
		{
			if (value == null)
			{
				RaiseErrorEvent( node.LexicalInfo, message );
			}
		}

		protected void AssertNotEmpty( NodeBase node, String value, String message )
		{
			AssertNotNull(node, value, message);
			if (String.Empty.Equals(value))
			{
				RaiseErrorEvent( node.LexicalInfo, message );
			}
		}

		protected void AssertKeyExists( IDictionary names, NodeBase node, object key, String message )
		{
			if (!names.Contains(key))
			{
				RaiseErrorEvent( node.LexicalInfo, message );
			}
		}
	}
}
