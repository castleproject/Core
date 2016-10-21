// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

#if FEATURE_DICTIONARYADAPTER_XML

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System.Xml.XPath;
	using System.Xml.Xsl;

	public class CompiledXPath
	{
		private XPathExpression path;
		private CompiledXPathStep firstStep;
		private int depth;

		internal CompiledXPath() { }

		public XPathExpression Path
		{
			get { return path; }
			internal set { path = value; }
		}

		public CompiledXPathStep FirstStep
		{
			get { return firstStep; }
			internal set { firstStep = value; }
		}

		public CompiledXPathStep LastStep
		{
			get
			{
				var step = null as CompiledXPathStep;
				var next = firstStep;

				while (next != null)
				{
					step = next;
					next = step.NextStep;
				}

				return step;
			}
		}

		public int Depth
		{
			get { return depth; }
			internal set { depth = value; }
		}

		public bool IsCreatable
		{
			get { return firstStep != null; }
		}

		internal void MakeNotCreatable()
		{
			firstStep = null;
			depth = 0;
		}

		internal void Prepare()
		{
			if (firstStep != null)
				firstStep.Prepare();
		}

		public void SetContext(XsltContext context)
		{
			path.SetContext(context);

			if (firstStep != null)
				firstStep.SetContext(context);
		}
	}
}

#endif
