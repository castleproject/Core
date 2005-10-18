namespace Castle.Facilities.Logging
{
    using System;
    using Castle.Model.Configuration;
    using Castle.Services.Logging;

    /// <summary>
	/// Summary description for LoggingConfiguration.
	/// </summary>
	public class LoggingConfiguration
	{
		public LoggingConfiguration(IConfiguration config)
		{
            IConfiguration defaultNode = config.Children["default"];

            String enableInterceptionAtt = defaultNode.Attributes["enableInterception"];
            this._enableInterception = DetermineInterception(enableInterceptionAtt);


            String libraryAtt = defaultNode.Attributes["type"];
            this._type = (LoggerImplementation)Enum.Parse(typeof(LoggerImplementation), libraryAtt, true);

            if(this._type == LoggerImplementation.Custom)
            {
                String customAtt = defaultNode.Attributes["custom"];
                String[] parts = customAtt.Split(Convert.ToChar(","));
                String typeName = parts[0].Trim();
                String assemblyName = parts[1].Trim();
                this._loggerFactory = System.Activator.CreateInstance(
                    assemblyName, typeName).Unwrap() as ILoggerFactory;
            }
            
		}

        private bool _enableInterception;

        public bool EnableInterception
        {
            get { return this._enableInterception; }
        }

        private LoggerImplementation _type;

        public LoggerImplementation Type
        {
            get { return this._type; }
        }

        private ILoggerFactory _loggerFactory;

        public ILoggerFactory LoggerFactory
        {
            get { return this._loggerFactory; }
        }

        private bool DetermineInterception(String input)
        {
            return Convert.ToBoolean(input);
        }
	}
}
