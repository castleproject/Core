namespace Castle.Windsor.Tests
{
	using System.Configuration;
	using System.IO;

	sealed class ConfigHelper
	{
		public static string ResolveConfigPath(string configFilePath)
		{
#if DOTNET2
			return Path.Combine(ConfigurationManager.AppSettings["tests.src"], configFilePath);
#else
			return Path.Combine(ConfigurationSettings.AppSettings["tests.src"], configFilePath);
#endif
		}
	}
}
