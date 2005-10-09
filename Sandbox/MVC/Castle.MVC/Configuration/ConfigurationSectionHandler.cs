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

using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Castle.MVC.Configuration
{
	/// <summary>
	/// The configuration section handler for the Castle.MVC section of the configuration file. 
	/// </summary>
	/// <remarks>
	/// Usage
	/// MVCConfigSettings ctx = ConfigurationSettings.GetConfig("mvc") as MVCConfigSettings; 
	/// </remarks>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{

		#region Fields

		private bool _isXmlValid = true;
		private string _schemaErrors = string.Empty;

		#endregion 
		
		#region Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConfigurationSectionHandler(){}
		#endregion 

		/// <summary>
		/// Factory method that creates a configuration handler for a specific section of 
		/// XML in the app.config.
		/// </summary>
		/// <param name="parent">
		/// The configuration settings in a corresponding parent
		/// configuration section.
		/// </param>
		/// <param name="configContext">
		/// The configuration context when called from the ASP.NET
		/// configuration system. Otherwise, this parameter is reserved and
		/// is a null reference.
		/// </param>
		/// <param name="section">
        /// The <see cref="System.Xml.XmlNode"/> for the section.
		/// </param>
		/// <returns>MVCConfigSettings for the section.</returns>
		public object Create(object parent, object configContext, XmlNode section) 
		{			
			return Create(parent, configContext, section, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Factory method that creates a configuration handler for a specific section of XML in the app.config.
		/// </summary>
		/// <param name="parent">
		/// The configuration settings in a corresponding parent
		/// configuration section.
		/// </param>
		/// <param name="configContext">
		/// The configuration context when called from the ASP.NET
		/// configuration system. Otherwise, this parameter is reserved and
		/// is a null reference.
		/// </param>
		/// <param name="section">
		/// The <see cref="System.Xml.XmlNode"/> for the section.
		/// </param>
		/// <param name="formatProvider">The format provider.
		/// </param>
		/// <returns>MVCConfigSettings for the section.</returns>
		public object Create(object parent, object configContext, XmlNode section, IFormatProvider formatProvider)
		{
			ValidateSchema( section );
			MVCConfigSettings config = new MVCConfigSettings(section, formatProvider); 
			return config;
		}

		private void ValidateSchema( XmlNode section )
		{
			XmlValidatingReader validatingReader = null;
			Stream xsdFile = null; 
			StreamReader streamReader = null; 
			try
			{
				//Validate the document using a schema
				validatingReader = new XmlValidatingReader( new XmlTextReader( new StringReader( section.OuterXml ) ) );
				validatingReader.ValidationType = ValidationType.Schema;

				xsdFile = Resource.ResourceManager.GetStream( "castle.mvc.xsd" ); 
				streamReader = new StreamReader( xsdFile ); 

				validatingReader.Schemas.Add( XmlSchema.Read( new XmlTextReader( streamReader ), null ) );
				validatingReader.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

				// Validate the document
				while (validatingReader.Read()){}

				if( !_isXmlValid )
				{
					throw new ConfigurationException(( Resource.ResourceManager.FormatMessage( Resource.MessageKeys.DocumentNotValidated, _schemaErrors )) );
				}
			}
			finally
			{
				if( validatingReader != null ) validatingReader.Close();
				if( xsdFile != null ) xsdFile.Close();
				if( streamReader != null ) streamReader.Close();
			}
		}

		private void ValidationCallBack( object sender, ValidationEventArgs args )
		{
			_isXmlValid = false;
			_schemaErrors += args.Message + Environment.NewLine;
		}
	}
}
