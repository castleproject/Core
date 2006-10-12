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

namespace Anakia.DocData
{
	using System;
	using System.Xml;

	public class ParameterDocData
	{
		private readonly string name;
		private readonly string type;
		private readonly XmlElement paramDocNode;

		public ParameterDocData(String name, String type, XmlElement paramDocNode)
		{
			this.name = name;
			this.type = type;
			this.paramDocNode = paramDocNode;
		}

		public string Name
		{
			get { return name; }
		}

		public string Type
		{
			get { return type; }
		}

		public XmlElement ParamDocNode
		{
			get { return paramDocNode; }
		}
	}
}
