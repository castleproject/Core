namespace Castle.MonoRail.Framework.Configuration
{
	using System.Collections;
	using System.Xml;

	/// <summary>
	/// Represents a set of url mappings
	/// </summary>
	public class DefaultUrlCollection : CollectionBase, ISerializedConfig
	{
		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified section.
		/// </summary>
		/// <param name="section">The section.</param>
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
