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
	/// Represents a node in a Graph.
	/// </summary>
	public class Vertex
	{
		private object m_content;
		private ArrayList m_successors = new ArrayList();

		/// <summary>
		/// Constructs the Vertex.
		/// </summary>
		/// <param name="content"></param>
		public Vertex(object content)
		{
			m_content = content;
		}

		/// <summary>
		/// Returns the Content related with the Vertex instance.
		/// </summary>
		public object Content
		{
			get { return m_content; }
		}

		/// <summary>
		/// Returns the accessible Vertex from this Vertex instance.
		/// </summary>
		public Vertex[] Successors
		{
			get { return (Vertex[]) m_successors.ToArray(typeof (Vertex)); }
		}

		/// <summary>
		/// Adds a vertex accessible from this Vertex instance.
		/// </summary>
		/// <param name="target"></param>
		public void AddSuccessor(Vertex target)
		{
			m_successors.Add( target );
		}
	}
}