namespace NVelocity.Util.Introspection
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>  
	/// Simple distinguishable exception, used when
	/// we run across ambiguous overloading
	/// </summary>
	[Serializable]
	public class AmbiguousException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AmbiguousException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public AmbiguousException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AmbiguousException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public AmbiguousException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AmbiguousException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
		public AmbiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}