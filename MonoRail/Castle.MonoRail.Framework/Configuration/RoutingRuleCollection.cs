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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Xml;

	public class RoutingRuleCollection : CollectionBase, ISerializedConfig
	{
		#region ISerializedConfig implementation
		
		public void Deserialize(XmlNode section)
		{
			XmlNodeList services = section.SelectNodes("routing/rule");
			
			foreach(XmlNode node in services)
			{
				XmlNode patternNode = node.SelectSingleNode("pattern");
				XmlNode replaceNode = node.SelectSingleNode("replace");

				if (patternNode == null || patternNode.ChildNodes.Count == 0 || patternNode.ChildNodes[0] == null)
				{
					String message = "A rule node must have a pattern (child) " + 
						"node denoting the regular expression to be matched";
#if DOTNET2
					throw new ConfigurationErrorsException(message);
#else
					throw new ConfigurationException(message);
#endif
				}
				if (replaceNode == null || replaceNode.ChildNodes.Count == 0 || replaceNode.ChildNodes[0] == null)
				{
					String message = "A rule node must have a replace (child) " + 
						"node denoting the string to be replaced";
#if DOTNET2
					throw new ConfigurationErrorsException(message);
#else
					throw new ConfigurationException(message);
#endif
				}

				String pattern = patternNode.ChildNodes[0].Value;
				String replace = replaceNode.ChildNodes[0].Value;

				RoutingRule rule = new RoutingRule(pattern.Trim(), replace.Trim());
				
				InnerList.Add(rule);
			}
		}
		
		#endregion
		
		public RoutingRule this[int index]
		{
			get { return InnerList[index] as RoutingRule; }
		}
	}
}
