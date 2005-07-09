namespace NVelocity.Exception
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>  
	/// Application-level exception thrown when a resource of any type
	/// isn't found by the Velocity engine.
	/// <br>
	/// When this exception is thrown, a best effort will be made to have
	/// useful information in the exception's message.  For complete
	/// information, consult the runtime log.
	/// </summary>
	[Serializable]
	public class ResourceNotFoundException : VelocityException
	{
		public ResourceNotFoundException(String exceptionMessage) : base(exceptionMessage)
		{
		}

		public ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}