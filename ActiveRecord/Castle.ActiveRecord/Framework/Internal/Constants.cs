// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Internal
{
	class Constants
	{
		public const string XmlPI= "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
		public const string XmlHeader = "<hibernate-mapping {0}{1} xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
				"xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:nhibernate-mapping-2.2\">";

		public const string XmlFooter = "</hibernate-mapping>";
	}
}
