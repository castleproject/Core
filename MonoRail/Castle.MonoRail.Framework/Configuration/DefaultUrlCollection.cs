namespace Castle.MonoRail.Framework.Configuration
{
	using System.Collections;
	using System.Xml;

	public class DefaultUrlCollection : CollectionBase, ISerializedConfig
	{
		#region ISerializedConfig implementation

		public void Deserialize(XmlNode section)
		{
			XmlNodeList urls = section.SelectNodes("defaultUrls/add");

			foreach(XmlNode node in urls)
			{
				DefaultUrl entry = new DefaultUrl();

				entry.Deserialize(node);

				InnerList.Add(entry);
			}
		}

		#endregion

	}
}
