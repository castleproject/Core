// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Xml;

	/// <summary>
	/// Represents a set of MonoRail extensions
	/// </summary>
	public class ExtensionEntryCollection : CollectionBase, ISerializedConfig
	{
		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified section.
		/// </summary>
		/// <param name="section">The section.</param>
		public void Deserialize(XmlNode section)
		{
			XmlNodeList services = section.SelectNodes("extensions/extension");
			
			foreach(XmlNode node in services)
			{
				ExtensionEntry entry = new ExtensionEntry();
				
				entry.Deserialize(node);
				
				InnerList.Add(entry);
			}
		}
		
		#endregion

		/// <summary>
		/// Gets the <see cref="Castle.MonoRail.Framework.Configuration.ExtensionEntry"/> at the specified index.
		/// </summary>
		/// <value></value>
		public ExtensionEntry this[int index]
		{
			get { return InnerList[index] as ExtensionEntry; }
		}
	}
}
