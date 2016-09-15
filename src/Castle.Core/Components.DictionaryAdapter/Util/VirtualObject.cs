// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections.Generic;

	public abstract class VirtualObject<TNode> : IVirtual<TNode>
	{
		private List<IVirtualSite<TNode>> sites;

		protected VirtualObject() { }

		protected VirtualObject(IVirtualSite<TNode> site)
		{
			sites = new List<IVirtualSite<TNode>> { site };
		}

		public abstract bool IsReal { get; }

		protected void AddSite(IVirtualSite<TNode> site)
		{
			if (sites != null)
			{
				sites.Add(site);
			}
		}

		void IVirtual<TNode>.AddSite(IVirtualSite<TNode> site)
		{
			AddSite(site);
		}

		protected void RemoveSite(IVirtualSite<TNode> site)
		{
			if (sites != null)
			{
				var index = sites.IndexOf(site);
				if (index != -1)
					sites.RemoveAt(index);
			}
		}

		void IVirtual<TNode>.RemoveSite(IVirtualSite<TNode> site)
		{
		    RemoveSite(site);
		}

		public TNode Realize()
		{
			TNode node;
			if (TryRealize(out node))
			{
				if (sites != null)
				{
					var count = sites.Count;
					for (var i = 0; i < count; i++)
						sites[i].OnRealizing(node);
					sites = null;
				}

				OnRealized();
			}
			return node;
		}

		void IVirtual.Realize()
		{
			Realize();
		}

		protected abstract bool TryRealize(out TNode node);

		public event EventHandler Realized;
		protected virtual void OnRealized()
		{
			var handler = Realized;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
				Realized = null;
			}
		}
	}
}
