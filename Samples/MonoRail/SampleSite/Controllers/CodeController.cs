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
