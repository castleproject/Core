#region Apache Notice
/*****************************************************************************
 * 
 * Castle.MVC
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

#region Autors

/************************************************
* Gilles Bayon
*************************************************/
#endregion 

#region Using

using System;
using System.Collections.Specialized;
using System.Xml;
#endregion 

namespace Castle.MVC.Configuration
{
	/// <summary>
	/// Represents a commands configuration file.
	/// </summary>
	public class CommandsSetting
	{
		/// <summary>
		/// Key used to retrieve command configuation element.
		/// </summary>
		internal const string NODE_COMMAND_XPATH = "command";
		/// <summary>
		/// Key used to retrieve id view attribute.
		/// </summary>
		internal const string ATTRIBUTE_VIEW = "view";
		/// <summary>
		/// Key used to retrieve id command attribute.
		/// </summary>
		internal const string ATTRIBUTE_ID = "id";

		#region Fields

		private string _view = string.Empty;
		private StringDictionary _commands = new StringDictionary();
		#endregion 

		#region Properties
		/// <summary>
		/// Gets the specifed view id to navigate.
		/// </summary>
		public string this[ string commandId ]
		{
			get{ return _commands[ commandId ]; }
		}

		/// <summary>
		/// Gets the view name.
		/// </summary>
		public string View
		{
			get{ return _view; }
		}
		#endregion

		/// <summary>
		/// Initializes an instance of the NodeSettings class using the specified configNode.
		/// </summary>
		/// <param name="configNode">The XmlNode from the configuration file.</param>
		public CommandsSetting(XmlNode configNode)
		{
			XmlNode currentAttribute = configNode.Attributes[ATTRIBUTE_VIEW];
			if( currentAttribute.Value.Trim().Length > 0  )
				_view = currentAttribute.Value;

			foreach(XmlNode commandNode in configNode.SelectNodes( NODE_COMMAND_XPATH ) )
			{
				string id = commandNode.Attributes[ATTRIBUTE_ID].Value;
				string view = commandNode.Attributes[ATTRIBUTE_VIEW].Value;
				_commands.Add( id, view );
			}
		}


	}
}
