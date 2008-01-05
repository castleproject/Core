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
	using System.Diagnostics;

	public abstract class TopologicalSortAlgo
	{
		public static IVertex[] Sort(IVertex[] graphNodes)
		{
			ColorsSet colors = new ColorsSet(graphNodes);
			TimestampSet discovery = new TimestampSet();
			TimestampSet finish = new TimestampSet();
			LinkedList list = new LinkedList();

			int time = 0;

			foreach(IVertex node in graphNodes)
			{
				if (colors.ColorOf(node) == VertexColor.White)
				{
					Visit(node, colors, discovery, finish, list, ref time);
				}
			}

			return (IVertex[]) list.ToArray(typeof(IVertex));
		}

		private static void Visit(IVertex node, ColorsSet colors,
		                          TimestampSet discovery, TimestampSet finish, LinkedList list, ref int time)
		{
			colors.Set(node, VertexColor.Gray);

			discovery.Register(node, time++);

			foreach(IVertex child in node.Adjacencies)
			{
				if (colors.ColorOf(child) == VertexColor.White)
				{
					Visit(child, colors, discovery, finish, list, ref time);
				}
			}

			finish.Register(node, time++);

#if DEBUG
			Debug.Assert(discovery.TimeOf(node) < finish.TimeOf(node));
#endif

			list.AddFirst(node);

			colors.Set(node, VertexColor.Black);
		}
	}
}