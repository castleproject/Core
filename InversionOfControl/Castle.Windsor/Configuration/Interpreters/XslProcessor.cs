// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Configuration.Interpreters
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Xsl;

	public class XslProcessor
	{
		private XslTransform transform;

		public XslProcessor()
		{
			const String stylesheet = "Castle.Windsor.Configuration.Interpreters.XslProcessor.xslt";

			using( Stream stream = this.GetType().Assembly.GetManifestResourceStream(stylesheet) )
			{
				StreamReader reader = new StreamReader(stream);

				XmlDocument xmlDoc = new XmlDocument();

				xmlDoc.LoadXml( reader.ReadToEnd());

				transform = new XslTransform();

				transform.Load( xmlDoc, new XmlUrlResolver(), AppDomain.CurrentDomain.Evidence );
			}
		}

		public XmlDocument Process(XmlDocument input, XslContext context)
		{			
			if( !NeedsProcessing( input ) ) return input;

			XsltArgumentList args = new XsltArgumentList();

			args.AddExtensionObject( "urn:castle", context );

			StringWriter output = new StringWriter();

			transform.Transform( input, args, output, new XmlUrlResolver() );
			
			XmlDocument result = new XmlDocument();

			result.LoadXml( output.ToString() );

			return result;
		}

		private bool NeedsProcessing( XmlDocument input )
		{
			return input.SelectSingleNode( " //*[ contains( 'if,define,undef,choose' , name() )] " ) != null;
		}
	}
}
