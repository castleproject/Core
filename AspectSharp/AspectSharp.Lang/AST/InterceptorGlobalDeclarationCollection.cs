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

namespace AspectSharp.Lang.AST
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for InterceptorGlobalDeclarationCollection.
	/// </summary>
	public class InterceptorGlobalDeclarationCollection : NodeCollectionBase, IDeclarationCollection
	{
		public DefinitionBase Add( String key, LexicalInfo info )
		{
			InterceptorEntryDefinition entry = new InterceptorEntryDefinition( key, info );
			Add(entry);
			return entry;
		}

		public void Add( InterceptorEntryDefinition entry )
		{
			InnerList.Add( entry );
		}

		public InterceptorEntryDefinition this[ int index ]
		{
			get { return InnerList[index] as InterceptorEntryDefinition; }
		}

		public IDictionary ToDictionary()
		{
			Hashtable dict = new Hashtable();

			foreach(InterceptorEntryDefinition entry in InnerList)
			{
				dict[ entry.Key ] = entry;
			}

			return dict;
		}

		public override void Accept(IVisitor visitor)
		{
			visitor.OnGlobalInterceptorDeclaration(this);
		}
	}
}
