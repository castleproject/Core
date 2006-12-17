#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
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


using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Castle.Igloo.Configuration
{
	/// <summary>
	/// The configuration section handler for the Castle.Igloo section of the configuration file. 
	/// </summary>
	/// <remarks>
	/// Usage
	/// IglooConfigSettings ctx = ConfigurationSettings.GetConfig("igloo") as IglooConfigSettings; 
	/// </remarks>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		private bool _isXmlValid = true;
		private string _schemaErrors = string.Empty;	

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
		/// <returns>IglooConfigSettings for the section.</returns>
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
		/// <returns>IglooConfigSettings for the section.</returns>
		public object Create(object parent, object configContext, XmlNode section, IFormatProvider formatProvider)
		{
			ValidateSchema( section );
			IglooConfigSettings config = new IglooConfigSettings(section, formatProvider); 
			return config;
		}

		private void ValidateSchema( XmlNode section )
		{
            XmlReader validatingReader = null;
			Stream xsdFile = null; 
			StreamReader streamReader = null; 
			
            try
			{
                xsdFile = Resource.ResourceManager.GetStream( "Castle.Igloo.xsd" );

                if (xsdFile == null)
                {
                    // TODO: avoid using hard-coded value "IBatisNet.DataMapper"
                    throw new ConfigurationErrorsException("Unable to locate embedded resource [castle.igloo.xsd] If you are building from source, verfiy the file is marked as an embedded resource.");
                }

                XmlSchema schema = XmlSchema.Read(xsdFile, new ValidationEventHandler(ValidationCallBack));

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;

                // Create the XmlSchemaSet class.
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                settings.Schemas = schemas;
                validatingReader = XmlReader.Create(new XmlNodeReader(section), settings);

                // Wire up the call back.  The ValidationEvent is fired when the
                // XmlValidatingReader hits an issue validating a section of the xml
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

                ////Validate the document using a schema
                //validatingReader = new XmlValidatingReader( new XmlTextReader( new StringReader( section.OuterXml ) ) );
                //validatingReader.ValidationType = ValidationType.Schema;

                //xsdFile = Resource.ResourceManager.GetStream( "castle.igloo.xsd" ); 
                //streamReader = new StreamReader( xsdFile ); 

                //validatingReader.Schemas.Add( XmlSchema.Read( new XmlTextReader( streamReader ), null ) );
                //validatingReader.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

				// Validate the document
				while (validatingReader.Read()){}

				if( !_isXmlValid )
				{
                    throw new ConfigurationErrorsException((Resource.ResourceManager.FormatMessage(Resource.MessageKeys.DocumentNotValidated, _schemaErrors)));
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
