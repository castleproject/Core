using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Castle.Core.Configuration;
using NHibernate.Cfg;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using NHibernate.Type;

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	/// <summary>
	/// Serializes the Configuration for subsequent initializations.
	/// </summary>
	public class PersistentConfigurationBuilder:DefaultConfigurationBuilder
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(PersistentConfigurationBuilder));
		private BinaryFormatter binaryFormatter;
		/// <summary>
		/// Initializes the presistent <see cref="Configuration"/> builder.
		/// </summary>
		public PersistentConfigurationBuilder()
		{
			this.binaryFormatter = new BinaryFormatter();
		}
		#region IConfigurationBuilder Members
		/// <summary>
		/// Returns the Deserialized Configuration
		/// </summary>
		/// <param name="config">The configuration node.</param>
		/// <returns>NHibernate Configuration</returns>
		public override Configuration GetConfiguration(IConfiguration config)
		{
			log.Debug("Building the Configuration");

			string fileName = config.Attributes["fileName"];

			IConfiguration dependsOn = config.Children["dependsOn"];
			IList<string> list = new List<string>();

			foreach (var on in dependsOn.Children)
				list.Add(on.Value);

			Configuration cfg;
			if (IsNewConfigurationRequired(fileName, list))
			{
				log.Debug("Configuration is either old or some of the dependencies have changed");
				using(var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
				{
					cfg = base.GetConfiguration(config);
					this.WriteConfigurationToStream(fileStream, cfg);
				}
			}
			else
			{
				using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
				{
					cfg = this.GetConfigurationFromStream(fileStream);
				}
			}
			return cfg;
		}
		/// <summary>
		/// Writes the <see cref="Configuration"/> to stream
		/// </summary>
		/// <param name="stream">The stream to be written</param>
		/// <param name="cfg"></param>
		protected virtual void WriteConfigurationToStream(Stream stream,Configuration cfg)
		{
			binaryFormatter.Serialize(stream, cfg);
		}
		/// <summary>
		/// Gets the <see cref="Configuration"/> from stream.
		/// </summary>
		/// <param name="fs">The stream from which the configuration will be deserialized</param>
		/// <returns>The <see cref="Configuration"/></returns>
		protected virtual Configuration GetConfigurationFromStream(Stream fs)
		{
			return binaryFormatter.Deserialize(fs) as Configuration;
		}

		/// <summary>
		/// Checks if a new <see cref="Configuration"/> is required or a serialized one should be used.
		/// </summary>
		/// <param name="fileName">The file that contains serialized <see cref="Configuration"/></param>
		/// <param name="dependencies">Files that the serialized configuration depends on. </param>
		/// <returns>If the <see cref="Configuration"/> should be created or not.</returns>
		protected virtual bool IsNewConfigurationRequired(string fileName,IList<string> dependencies)
		{
			if (!File.Exists(fileName))
				return true;
			DateTime lastModified = File.GetLastWriteTime(fileName);
			bool requiresNew=false;
			for (int i = 0; i < dependencies.Count && !requiresNew; i++)
			{
				DateTime dependencyLastModified = File.GetLastWriteTime(dependencies[i]);
				requiresNew |= dependencyLastModified > lastModified;
			}
			return requiresNew;
		}
		#endregion
	}
}
