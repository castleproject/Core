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

	/// <summary>
	/// Represents a direct path between two Vertex.
	/// </summary>
	public class Edge
	{
		private Vertex m_source;
		private Vertex m_target;

		/// <summary>
		/// Constructs a Edge.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		public Edge(Vertex from, Vertex to)
		{
			m_source = from;
			m_target = to;
		}

		/// <summary>
		/// The source node.
		/// </summary>
		public Vertex Source
		{
			get { return m_source; }
		}

		/// <summary>
		/// The target node.
		/// </summary>
		public Vertex Target
		{
			get { return m_target; }
		}
	}
}