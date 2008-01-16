// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Framework;

	/// <summary>
	/// Represents the Scaffolding support configuration.
	/// <seealso cref="IScaffoldingSupport"/>
	/// </summary>
	public class ScaffoldConfig : ISerializedConfig
	{
		private Type scaffoldImplType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScaffoldConfig"/> class.
		/// </summary>
		public ScaffoldConfig()
		{
			TryLoadDefaultScaffoldingImplementation();
		}

		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the configuration section looking 
		/// for a 'scaffold' element with a 'type' attribute
		/// </summary>
		/// <param name="section">The section.</param>
		public void Deserialize(XmlNode section)
		{
			section = section.SelectSingleNode("scaffold");
			
			if (section == null) return;
			
			XmlAttribute typeAtt = section.Attributes["type"];
			
			if (typeAtt == null || typeAtt.Value == String.Empty)
			{
				String message = "Please specify the 'type' attribute to define an implementation for scaffolding support";
				throw new ConfigurationErrorsException(message);
			}
			
			scaffoldImplType = TypeLoadUtil.GetType(typeAtt.Value);
		}
		
		#endregion

		/// <summary>
		/// Gets the scaffolding support implementation type.
		/// <seealso cref="IScaffoldingSupport"/>
		/// </summary>
		/// <value>The type of the scaffold impl.</value>
		public Type ScaffoldImplType
		{
			get { return scaffoldImplType; }
		}

		private void TryLoadDefaultScaffoldingImplementation()
		{
			scaffoldImplType = Type.GetType("Castle.MonoRail.ActiveRecordSupport.Scaffold.ScaffoldingSupport, Castle.MonoRail.ActiveRecordSupport", false, false);
		}
	}
}
