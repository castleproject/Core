using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Facilities.Logging.Tests.Classes
{
	using Castle.Core.Logging;

	public class ComplexLoggingComponent
	{
		IExtendedLogger logger;
		public ComplexLoggingComponent(IExtendedLogger logger)
		{
			this.logger = logger;
		}

		public void DoSomeContextual()
		{
			using (logger.ThreadStacks["NDC"].Push("Outside"))
      {
				for (int i = 0; i < 3; i++)
				{
					using (logger.ThreadStacks["NDC"].Push("Inside" + i))
          {
						logger.ThreadProperties["foo"] = "bar";
						logger.GlobalProperties["flim"] = "flam";
						logger.Debug("Bim, bam boom.");
          }
				}
      }
		}
	}
}
