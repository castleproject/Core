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
* Gilles Bayon (Inspired by the UIP V2)
*************************************************/
#endregion 

#region Using

using System;
using System.Resources;
using System.Reflection; 
using System.IO;
#endregion 


namespace Castle.MVC
{
	/// <summary>
	/// Access to resource data.
	/// </summary>
	public class Resource
	{

		#region Fields

		private const string RESOURCE_FILENAME = "Castle.MVC";
		private static Resource _internalResource = new Resource();
		private ResourceManager _resourceManager= null;

		#endregion 

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public Resource()
		{
			_resourceManager = new ResourceManager(this.GetType().Namespace + RESOURCE_FILENAME, Assembly.GetExecutingAssembly());						
		}
		#endregion 

		#region Properties

		/// <summary>
		/// Gets a resource manager for the assembly resource file.
		/// </summary>
		public static Resource ResourceManager
		{
			get
			{
				return _internalResource;
			}
		}



		/// <summary>
		/// Gets the message with the specified key from the assembly resource file.
		/// </summary>
		/// <param name="key">Key of the item to retrieve from the resource file.</param>
		/// <returns>Value from the resource file identified by the key.</returns>
		public string this [ string key ]
		{
			get
			{
				return _resourceManager.GetString( key, System.Globalization.CultureInfo.CurrentUICulture );																
			}
		}
		#endregion 

		#region Methods

		/// <summary>
		/// Gets a resource stream with the messages used by the UIP classes.
		/// </summary>
		/// <param name="name">The resource key.</param>
		/// <returns>A resource stream.</returns>
		public Stream GetStream( string name )
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType().Namespace + "." + name); 
		}

		/// <summary>
		/// Formats a message stored in the UIP assembly resource file.
		/// </summary>
		/// <param name="key">The resource key.</param>
		/// <param name="format">The format arguments.</param>
		/// <returns>A formatted string.</returns>
		public string FormatMessage( string key, params object[] format )
		{
			return String.Format( System.Globalization.CultureInfo.CurrentCulture, this[key], format );  
		}
		#endregion 


		/// <summary>
		/// Class used to expose constants that represent keys in the resource file.
		/// </summary>
		internal abstract class MessageKeys
		{		
			internal const string ViewAlreadyConfigured = "ViewAlreadyConfigured";	
			internal const string CantFindCommandMapping = "CantFindCommandMapping";	
			internal const string CantGetNextView = "CantGetNextView";	
			internal const string DocumentNotValidated = "DocumentNotValidated";


		}
	}
}
