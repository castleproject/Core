// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Runtime.Serialization;

	// NOTE TO MAINTAINERS:
	// Prefer throwing Base Class Library exception types wherever appropriate.
	// This exception type is to be used mostly when something inside DynamicProxy goes wrong.
	// Think of it as a "failed assertion" / "bug" exception.
	[Serializable]
	public sealed class DynamicProxyException : Exception
	{
		internal DynamicProxyException(string message) : base(message)
		{
		}

		internal DynamicProxyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		internal DynamicProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
