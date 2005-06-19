
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
using System.Configuration;
using System.Xml;
#endregion 

namespace Castle.MVC.Configuration
{
	/// <summary>
	/// MVCConfigSettings.
	/// </summary>
	public class MVCConfigSettings
	{
		#region Const
		/// <summary>
		/// Token to retrieve configuration node
		/// </summary>
		private const string NODE_WEB_VIEW_XPATH = "webViews/view";
		private const string NODE_WIN_VIEW_XPATH = "winViews/view";
		private const string ATTRIBUTE_ID = "id";
		private const string ATTRIBUTE_VIEW = "view";
		private const string ATTRIBUTE_PATH = "path";
		private const string ATTRIBUTE_TYPE = "type";
		private const string NODE_COMMANDS_XPATH = "command-mappings/commands";
		private const string NODE_GLOBAL_COMMANDS_XPATH = "global-commands/command";

		#endregion

		#region Fields

		private StringDictionary _urls = new StringDictionary();
		private StringDictionary _webViews= new StringDictionary();
		private HybridDictionary _types = new HybridDictionary();
		private HybridDictionary _winViews= new HybridDictionary();
		private HybridDictionary _mappings = new HybridDictionary();
		private StringDictionary _globalCommands = new StringDictionary();

		#endregion 

		#region Constructor

		/// <summary>
		/// Creates MVCConfigSettings from an XmlNode read of the web.config and an IFormatProvider.
		/// </summary>
		/// <param name="configNode">The XmlNode from the configuration file.</param>
		/// <param name="formatProvider">The provider.</param>
		public MVCConfigSettings(XmlNode configNode, IFormatProvider formatProvider) 
		{
			LoadWebViews(configNode, formatProvider);
			LoadWinViews(configNode, formatProvider);
			LoadGlobalCommand(configNode);
			LoadCommandMapping(configNode);
		}
		#endregion 

		#region Methods

		/// <summary>
		/// Load the global commands
		/// </summary>
		/// <param name="configNode">The XmlNode from the configuration file.</param>
		private void LoadGlobalCommand(XmlNode configNode)
		{
			foreach(XmlNode commandNode in configNode.SelectNodes( NODE_GLOBAL_COMMANDS_XPATH ) )
			{
				string id = commandNode.Attributes[ATTRIBUTE_ID].Value;
				string view = commandNode.Attributes[ATTRIBUTE_VIEW].Value;
				_globalCommands.Add( id, view );
			}
		}

		/// <summary>
		/// Load the command mapping
		/// </summary>
		/// <param name="configNode">The XmlNode from the configuration file.</param>
		private void LoadCommandMapping(XmlNode configNode)
		{		
			foreach( XmlNode currentNode in configNode.SelectNodes( NODE_COMMANDS_XPATH ) )
			{
				CommandsSetting setting = new CommandsSetting( currentNode );
				_mappings.Add( setting.View, setting );
			}
		}

		/// <summary>
		/// Load the views
		/// </summary>
		/// <param name="configNode">The XmlNode from the configuration file.</param>
		/// <param name="formatProvider">The provider.</param>
		private void LoadWebViews(XmlNode configNode, IFormatProvider formatProvider)
		{
			//Get the views
			foreach( XmlNode viewNode in configNode.SelectNodes( NODE_WEB_VIEW_XPATH ) )
			{
				string viewName = viewNode.Attributes[ATTRIBUTE_ID].Value;
				string viewUrl = viewNode.Attributes[ATTRIBUTE_PATH].Value;

				if( !_webViews.ContainsKey( viewName ) )
				{
					_urls.Add( viewName, viewUrl ); 
					_webViews.Add( viewUrl, viewName ); 
				}
				else 
				{
					throw new ConfigurationException( Resource.ResourceManager.FormatMessage( Resource.MessageKeys.ViewAlreadyConfigured, viewName ) ); 
				}
			}
		}

		/// <summary>
		/// Load the windows views
		/// </summary>
		/// <param name="configNode">The XmlNode from the configuration file.</param>
		/// <param name="formatProvider">The provider.</param>
		private void LoadWinViews(XmlNode configNode, IFormatProvider formatProvider)
		{
			//Get the views
			foreach( XmlNode viewNode in configNode.SelectNodes( NODE_WIN_VIEW_XPATH ) )
			{
				string viewId = viewNode.Attributes[ATTRIBUTE_ID].Value;
				string viewType = viewNode.Attributes[ATTRIBUTE_TYPE].Value;

				// infer type from viewType
				Type type = null;
				if( !_winViews.Contains( type ) )
				{
					_types.Add( viewId, type ); 
					//_winViews.Add( type, viewId ); 
				}
				else 
				{
					throw new ConfigurationException( Resource.ResourceManager.FormatMessage( Resource.MessageKeys.ViewAlreadyConfigured, viewId ) ); 
				}
			}
		}

		///<summary>
		///Looks up a url view based on view name.
		///</summary>  
		///<param name="viewName">The name of the view to retrieve the settings for.</param>
		public virtual string GetUrl( string viewName )
		{
			return _urls[viewName] as string;
		}

		///<summary>
		/// Looks up a web view based on his url.
		///</summary>  
		///<param name="url">The URL.</param>
		public virtual string GetView(string url)
		{
			return _webViews[url] as string;
		}

		///<summary>
		/// Looks up a windows view based on his type.
		///</summary>  
		///<param name="type">The view type.</param>
		public virtual string GetView(Type type)
		{
			return _winViews[type] as string;
		}

		///<summary>
		///Looks up a next view based on current command id and and current view id.
		///</summary>  
		///<param name="commandID">The id of the current command.</param>
		///<param name="viewID">The id of the current view.</param>
		///<returns>The next web view to go.</returns>
		public virtual string GetNextView(string viewID, string commandID)
		{
			string nextView = string.Empty;

			if (_globalCommands.ContainsKey(commandID))
			{
				nextView = _globalCommands[commandID];
			}
			else
			{
				CommandsSetting setting= null;
				if( _mappings.Contains( viewID ) )
				{
					setting = _mappings[viewID] as CommandsSetting;
				}
				else
				{
					throw new ConfigurationException( Resource.ResourceManager.FormatMessage( Resource.MessageKeys.CantFindCommandMapping, viewID ) ); 
				}
				nextView = setting[commandID];
				if( nextView==null )
				{
					throw new ConfigurationException( Resource.ResourceManager.FormatMessage( Resource.MessageKeys.CantGetNextView, viewID, commandID ) ); 
				}				
			}

			return nextView;
		}
		#endregion 


	}
}