using System;

namespace Castle.DynamicProxy
{
	public class ProxyGenerationException : Exception
	{
		public ProxyGenerationException(string message) : base(message)
		{}

		public ProxyGenerationException(string message, Exception innerException):base(message,innerException)
		{
		}
	}
}
