﻿// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace CastleTests
{
#if !SILVERLIGHT && !NETCORE

	using System.IO;
	using System.Xml;

	using Xunit;

	public class XmlConfigurationDeserializerTests
	{
		[Fact]
		[Bug("CORE-37")]
		public void Tab_character_is_not_trimmed_from_config_value()
		{
			string result = Castle.Core.Configuration.Xml.XmlConfigurationDeserializer.GetConfigValue("\t");
			Assert.Equal("\t", result);
		}

		[Fact]
		[Bug("CORE-37")]
		public void Tab_character_is_not_trimmed_from_config_value_XML()
		{
			var node = new XmlDocument().ReadNode(XmlReader.Create(new StringReader("<foo><![CDATA[\t]]></foo>")));
			var result = Castle.Core.Configuration.Xml.XmlConfigurationDeserializer.GetDeserializedNode(node);
			Assert.Equal("\t", result.Value);
		}
	}
#endif
}