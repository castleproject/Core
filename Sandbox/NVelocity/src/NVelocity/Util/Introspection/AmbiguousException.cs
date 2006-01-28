namespace NVelocity.Util.Introspection
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>  simple distinguishable exception, used when
	/// we run across ambiguous overloading
	/// </summary>
	[Serializable]
	public class AmbiguousException : Exception
	{
		public AmbiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}