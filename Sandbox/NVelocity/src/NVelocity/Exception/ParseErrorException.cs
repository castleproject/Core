namespace NVelocity.Exception
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>  Application-level exception thrown when a resource of any type
	/// has a syntax or other error which prevents it from being parsed.
	/// <br>
	/// When this resource is thrown, a best effort will be made to have
	/// useful information in the exception's message.  For complete
	/// information, consult the runtime log.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ParseErrorException.cs,v 1.3 2003/10/27 13:54:08 corts Exp $
	///
	/// </version>
	[Serializable]
	public class ParseErrorException : VelocityException
	{
		public ParseErrorException(String exceptionMessage) : base(exceptionMessage)
		{
		}

		public ParseErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}