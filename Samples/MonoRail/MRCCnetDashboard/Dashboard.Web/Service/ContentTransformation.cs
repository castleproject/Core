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

namespace Dashboard.Web.Service
{
	using System;
	using System.IO;
	using System.Xml.XPath;

	using ThoughtWorks.CruiseControl.Remote;
	using ThoughtWorks.CruiseControl.Core.Publishers;


	public enum DetailEnum
	{
		Summary,
		Modifications,
		Compilation,
		UnitTestsSummary,
		UnitTestsDetail
	}


	public class ContentTransformation
	{
		private readonly ICruiseManager cruiseManager;
		private readonly IBuildLogTransformer logTransformer;

		public ContentTransformation(ICruiseManager cruiseManager, IBuildLogTransformer logTransformer)
		{
			this.logTransformer = logTransformer;
			this.cruiseManager = cruiseManager;
		}

		public String GetSummary(String name, String log)
		{
			String xsl = GetXslFullFileName("header.xsl");

			String content = cruiseManager.GetLog(name, log);

			XPathDocument document = new XPathDocument(new StringReader(content));

			return logTransformer.Transform(document, xsl);
		}

		public String GetDetail(String name, String log, DetailEnum detail)
		{
			String xsl = null;

			if (detail == DetailEnum.Summary)
			{
				return GetSummary(name, log);
			}
			else if (detail == DetailEnum.Compilation)
			{
				xsl = GetXslFullFileName("compile.xsl");
			}
			else if (detail == DetailEnum.Modifications)
			{
				xsl = GetXslFullFileName("modifications.xsl");
			}
			else if (detail == DetailEnum.UnitTestsDetail)
			{
				xsl = GetXslFullFileName("unittests.xsl");
			}
			else if (detail == DetailEnum.UnitTestsSummary)
			{
				xsl = GetXslFullFileName("AlternativeNUnitDetails.xsl");
			}

			String content = cruiseManager.GetLog(name, log);

			XPathDocument document = new XPathDocument(new StringReader(content));

			return logTransformer.Transform(document, xsl);
		}

		private String GetXslFullFileName(String xslFile)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xsl/" + xslFile);
		}
	}
}
