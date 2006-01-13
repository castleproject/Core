// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	/*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2001 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */

	/// <summary>
	/// Application-level exception thrown when a reference method is
	/// invoked and an exception is thrown.
	/// <br>
	/// When this exception is thrown, a best effort will be made to have
	/// useful information in the exception's message.  For complete
	/// information, consult the runtime log.
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: MethodInvocationException.cs,v 1.3 2003/10/27 13:54:08 corts Exp $ </version>
	[Serializable]
	public class MethodInvocationException : VelocityException
	{
		private String methodName = "";
		private String referenceName = "";

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