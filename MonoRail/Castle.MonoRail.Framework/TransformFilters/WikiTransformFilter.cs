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

namespace Castle.MonoRail.Framework.TransformFilters
{
	using System;
	using System.IO;
	using System.Text;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.TransformFilters.Formatters;

	/// <summary>
	/// Post process the request via Castle.WikiFormatter
	/// </summary>
	public class WikiTransformFilter : TransformFilter
	{
		/// <summary>
		/// Constructor of the WikiTransformfilter
		/// </summary>
		/// <param name="baseStream">output stream</param>
		public WikiTransformFilter(Stream baseStream) : base(baseStream)
		{
		}

		/// <summary>
		/// Pulls the http stream through the WikiFormatter filter.
		/// </summary>
		/// <param name="buffer">The content stream</param>
		/// <param name="offset">Start of the stream</param>
		/// <param name="count">Lenght of the stream</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (Closed) throw new ObjectDisposedException("WikiTransformFilter");

			string content = Encoding.Default.GetString(buffer, offset, count);

			WikiFormatter wikiFormatter = new WikiFormatter();

			content = wikiFormatter.Format(content);

			byte[] output = Encoding.Default.GetBytes(content);
			BaseStream.Write(output, 0, output.Length);
		}
	}
}
