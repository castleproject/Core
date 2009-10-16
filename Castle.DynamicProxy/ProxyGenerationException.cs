using System;

namespace Castle.DynamicProxy
{
	public class ProxyGenerationException : Exception
	{
		public ProxyGenerationException(string message) : base(message)
		{}
	}
}
