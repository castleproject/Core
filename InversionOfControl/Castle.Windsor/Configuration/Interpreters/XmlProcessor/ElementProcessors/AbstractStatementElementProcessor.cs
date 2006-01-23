// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors
{
	using System;
	using System.Xml;

	public abstract class AbstractStatementElementProcessor : AbstractXmlNodeProcessor
	{
		private static readonly String DefinedAttrName = "defined";
		private static readonly String NotDefinedAttrName = "not-defined";

		public AbstractStatementElementProcessor()
		{
		}

		protected bool ProcessStatement(XmlElement element, IXmlProcessorEngine engine)
		{
			if (!element.HasAttribute(DefinedAttrName) &&
				!element.HasAttribute(NotDefinedAttrName))
			{
				throw new XmlProcessorException("'if' elements expects a non empty defined or not-defined attribute");
			}

			if (element.HasAttribute(DefinedAttrName) &&
				element.HasAttribute(NotDefinedAttrName))
			{
				throw new XmlProcessorException("'if' elements expects a non empty defined or not-defined attribute");
			}

			bool processContents = false;

			if (element.HasAttribute(DefinedAttrName))
			{
				processContents = engine.HasFlag(element.GetAttribute(DefinedAttrName));
			}
			else
			{
				processContents = !engine.HasFlag(element.GetAttribute(NotDefinedAttrName));
			}

			return processContents;
		}
	}
}