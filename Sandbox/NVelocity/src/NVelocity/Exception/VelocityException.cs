namespace NVelocity.Exception
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>  
	/// Base class for Velocity exceptions thrown to the
	/// application layer.
	/// </summary>
	[Serializable]
	public class VelocityException : Exception
	{
		public VelocityException(String exceptionMessage) : base(exceptionMessage)
		{
		}

		public VelocityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}