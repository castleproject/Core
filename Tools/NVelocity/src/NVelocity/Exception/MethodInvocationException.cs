// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NVelocity.Exception
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Application-level exception thrown when a reference method is
	/// invoked and an exception is thrown.
	/// <br/>
	/// When this exception is thrown, a best effort will be made to have
	/// useful information in the exception's message.  For complete
	/// information, consult the runtime log.
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: MethodInvocationException.cs,v 1.3 2003/10/27 13:54:08 corts Exp $ </version>
	[Serializable]
	public class MethodInvocationException : VelocityException
	{
		private readonly String methodName = string.Empty;
		private String referenceName = string.Empty;

		/// <summary>
		/// Wraps the passed in exception for examination later
		/// </summary>
		public MethodInvocationException(String message, Exception innerException, String methodName)
			: base(message, innerException)
		{
			this.methodName = methodName;
		}

		#region Serialization Support

		protected MethodInvocationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			methodName = info.GetString("methodName");
			referenceName = info.GetString("referenceName");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("methodName", methodName);
			info.AddValue("referenceName", referenceName);
			base.GetObjectData(info, context);
		}

		#endregion

		public String MethodName
		{
			get { return methodName; }
		}

		public String ReferenceName
		{
			get { return referenceName; }
			set { referenceName = value; }
		}
	}
}