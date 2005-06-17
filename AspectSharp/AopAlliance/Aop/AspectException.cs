using System;
using System.Runtime.Serialization;

namespace AopAlliance.Aop
{
	/// <summary>
	/// Superclass for all AOP infrastructure exceptions.
	/// </summary>
	/// <author>Aleksandar Seovic</author>
	/// <version>$Id: AspectException.cs,v 1.1 2004/11/20 21:13:04 markpollack Exp $</version>
	[Serializable]
	public class AspectException : Exception
	{
        /// <summary>
        /// Default constructor for AspectException.
        /// </summary>
        public AspectException() : base()
        {
        }
        
        /// <summary>
		/// Constructor for AspectException.
		/// </summary>
		/// <param name="message">Error message</param>
		public AspectException(string message) : base(message)
		{
		}

        /// <summary>
		/// Constructor for AspectException.
		/// </summary>
		/// <param name="message">Error message</param>
        /// <param name="innerException">Root exception cause</param>
        public AspectException(string message, Exception innerException) : base(message, innerException)
		{
		}

        /// <summary>
        /// Creates a new instance of the AspectException class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
        /// that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/>
        /// that contains contextual information about the source or destination.
        /// </param>
        protected AspectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
	}
}
