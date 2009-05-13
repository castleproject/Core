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

namespace SampleSite.Controllers
{
	using System;
	using System.IO;
	using System.Text;

	using Castle.MonoRail.Framework;


	public class CodeController : SmartDispatcherController
	{
		public void ShowCode(String file)
		{
			FileInfo info = new FileInfo( Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file) );

			if (info.Exists && 
				(info.Extension.Equals(".cs") || info.Extension.Equals(".vm") || info.Extension.Equals(".aspx")))
			{
				bool encodeTags = info.Extension.Equals(".vm") || info.Extension.Equals(".aspx");

				StringBuilder sb = new StringBuilder();

				using(FileStream fs = File.OpenRead( info.FullName ))
				{
					StreamReader reader = new StreamReader(fs);
					String line;
					while( (line = reader.ReadLine()) != null )
					{
						line = line.Replace("\t", "    ");
						if (encodeTags)
						{
							line = line.Replace("<", "&lt;");
							line = line.Replace(">", "&gt;");
						}
						sb.Append( line );
						sb.Append( "\r\n" );
					}
				}

				PropertyBag.Add("file", info.FullName);
				PropertyBag.Add("code", sb.ToString());
			}
			else
			{
				RenderText( String.Format("Source file {0} not found", info.Name) );
			}
		}
	}
}
