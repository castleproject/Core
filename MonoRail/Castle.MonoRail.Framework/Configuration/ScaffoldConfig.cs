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
	using System.Configuration;
	using System.Xml;

	public class ScaffoldConfig : ISerializedConfig
	{
		private Type scaffoldImplType;
		
		#region ISerializedConfig implementation
		
		public void Deserialize(XmlNode section)
		{
			section = section.SelectSingleNode("scaffold");
			
			if (section == null) return;
			
			XmlAttribute typeAtt = section.Attributes["type"];
			
			if (typeAtt == null || typeAtt.Value == String.Empty)
			{
				throw new ConfigurationException("Please specify the 'type' attribute to define an implementation for scaffolding support");
			}
			
			scaffoldImplType = TypeLoadUtil.GetType(typeAtt.Value);
		}
		
		#endregion

		public Type ScaffoldImplType
		{
			get { return scaffoldImplType; }
		}
	}
}
