﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml.XPath;
	using System.Xml.Xsl;

	public class CompiledXPathStep : CompiledXPathNode
	{
		private XPathExpression path;

		internal CompiledXPathStep() { }

		public XPathExpression Path
		{
			get { return path; }
			internal set { path = value; }
		}

		public CompiledXPathStep NextStep
		{
			get { return (CompiledXPathStep) NextNode; }
		}

		internal override void SetContext(XsltContext context)
		{
			path.SetContext(context);
			base.SetContext(context);
		}
	}
}
