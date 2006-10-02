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

namespace Anakia
{
	using System;
	using System.Collections;

	public class DocumentNodeCollection : IEnumerable
	{
		private readonly Folder owner;
		private SortedList list = new SortedList(DocumentNodeComparer.Instance);
		private DocumentNode navNode;
		private DocumentNode indexNode;

		public DocumentNodeCollection(Folder owner)
		{
			this.owner = owner;
		}

		public DocumentNode NavigationNode
		{
			get { return navNode; }
		}

		public DocumentNode IndexNode
		{
			get { return indexNode; }
		}

		public void Add(DocumentNode node)
		{
			node.ParentFolder = owner;
			
			if (node.NodeType == NodeType.Navigation)
			{
				navNode = node;
			}
			else if (node.NodeType == NodeType.Index)
			{
				indexNode = node;
			}
			
			list.Add(node.Meta.Order, node);
		}

		public IEnumerator GetEnumerator()
		{
			return list.Values.GetEnumerator();
		}
	}
}
