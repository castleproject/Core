// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Internal
{
	using System.Collections;
    using System.Collections.Generic;

	internal enum VertexColor
	{
		NotInThisSet,
		/// <summary>
		/// The node has not been visited yet
		/// </summary>
		White,
		/// <summary>
		/// This node is in the process of being visited
		/// </summary>
		Gray,
		/// <summary>
		/// This now was visited
		/// </summary>
		Black
	}

	/// <summary>
	/// Represents a collection of objects
	/// which are guaranted to be unique 
	/// and holds a color for them
	/// </summary>
	internal class ColorsSet
	{
        private IDictionary<IVertex, VertexColor> items = new Dictionary<IVertex, VertexColor>();

		public ColorsSet(IVertex[] items)
		{
			foreach(IVertex item in items)
			{
				Set(item, VertexColor.White);
			}
		}

		public void Set(IVertex item, VertexColor color)
		{
			items[item] = color;
		}

		public VertexColor ColorOf(IVertex item)
		{
			if (!items.ContainsKey(item)) return VertexColor.NotInThisSet;
			return (VertexColor) items[item];
		}
	}

	/// <summary>
	/// Holds a timestamp (integer) 
	/// for a given item
	/// </summary>
	internal class TimestampSet
	{
        private IDictionary<IVertex, int> items = new Dictionary<IVertex, int>();

		public TimestampSet()
		{
		}

		public void Register(IVertex item, int time)
		{
			items[item] = time;
		}

		public int TimeOf(IVertex item)
		{
			return (int) items[item];
		}
	}
}
