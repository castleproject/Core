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
	/// Simple Graph implementation just to fulfill the requirements 
	/// for a topological sort of the components and handlers for a 
	/// correct disposal order.
	/// </summary>
	public class SimpleGraph
	{
		private ArrayList m_vertices = new ArrayList();
		private ArrayList m_edges = new ArrayList();
		private Hashtable m_content2Vertex = new Hashtable();

		/// <summary>
		/// Returns the Vertex associated with the content
		/// </summary>
		public Vertex this[object content]
		{
			get { return m_content2Vertex[ content ] as Vertex; }
		}

		/// <summary>
		/// Creates and returns a vertex with a specified content.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public Vertex CreateVertex(object content)
		{
			Vertex vertex = new Vertex(content);
			m_content2Vertex[ content ] = vertex;
			m_vertices.Add(vertex);
			return vertex;
		}

		/// <summary>
		/// Creates and returns an edge which connects two 
		/// vertex in a direct manner.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public Edge CreateEdge(Vertex from, Vertex to)
		{
			Edge edge = new Edge(from, to);
			m_edges.Add(edge);
			from.AddSuccessor(to);
			return edge;
		}

		/// <summary>
		/// Returns all vertices created.
		/// </summary>
		public Vertex[] Vertices
		{
			get { return (Vertex[]) m_vertices.ToArray(typeof (Vertex)); }
		}

		/// <summary>
		/// Returns all edges.
		/// </summary>
		public Edge[] Edges
		{
			get { return (Edge[]) m_edges.ToArray(typeof (Edge)); }
		}
	}
}