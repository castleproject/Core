 // Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Graph
{
	using System;
	using System.Collections;

	/// <summary>
	/// Implements a Topological Sort algorithm.
	/// </summary>
	public abstract class TopologicalSort
	{
		/// <summary>
		/// Arrange the Vertex making the less accessible 
		/// vertex coming before than others. Returns the
		/// Vertex array with the result.
		/// </summary>
		/// <param name="graph"></param>
		/// <returns></returns>
		public static Vertex[] Perform(SimpleGraph graph)
		{
			ArrayList result = new ArrayList();
			Hashtable inDegree = new Hashtable();
			Queue queue = new Queue();

			foreach(Edge edge in graph.Edges)
			{
				IncrementDegree(edge.Target, inDegree);
			}

			foreach(Vertex vertex in graph.Vertices)
			{
				if (ObtainInDegree(vertex, inDegree) == 0)
				{
					queue.Enqueue(vertex);
				}
			}

			while(queue.Count != 0)
			{
				Vertex v = (Vertex) queue.Dequeue();

				result.Add(v);

				foreach(Vertex to in v.Successors)
				{
					if (DecrementDegree(to, inDegree) == 0)
					{
						queue.Enqueue(to);
					}
				}
			}

			return (Vertex[]) result.ToArray(typeof (Vertex));
		}

		private static int ObtainInDegree(Vertex vertex, Hashtable inDegree)
		{
			int value = 0;
			if (inDegree.ContainsKey(vertex))
			{
				value = (int) inDegree[ vertex ];
			}
			return value;
		}

		private static void IncrementDegree(Vertex vertex, Hashtable inDegree)
		{
			int value = ObtainInDegree(vertex, inDegree);
			inDegree[ vertex ] = ++value;
		}

		private static int DecrementDegree(Vertex vertex, Hashtable inDegree)
		{
			int value = ObtainInDegree(vertex, inDegree);
			inDegree[ vertex ] = --value;
			return value;
		}
	}
}