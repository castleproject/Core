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

using System;

/// <summary>
/// Specifies the lifecycle of a message.
/// </summary>
[Flags]
public enum MessageLifecycle
{
	/// <summary>
	/// The outgoing request.
	/// </summary>
	OutgoingRequest = 0x01,
	/// <summary>
	/// The incoming response.
	/// </summary>
	IncomingResponse = 0x02,
	/// <summary>
	/// The outgoing request.
	/// </summary>
	IncomingRequest = 0x04,
	/// <summary>
	/// The incoming response.
	/// </summary>
	OutgoingResponse = 0x08,
	/// <summary>
	/// All incoming messages.
	/// </summary>
	IncomingMessages = IncomingRequest | IncomingResponse,
	/// <summary>
	/// All outgoing messages.
	/// </summary>
	OutgoingMessages = OutgoingRequest | OutgoingResponse,
	/// <summary>
	/// A solitic/response exchange.
	/// </summary>
	OutgoingRequestResponse = OutgoingRequest | IncomingResponse,
	/// <summary>
	/// A request/response exchange.
	/// </summary>
	IncomingRequestResponse = IncomingRequest | OutgoingResponse,
	/// <summary>
	/// All requests.
	/// </summary>
	Requests = IncomingRequest | OutgoingRequest,
	/// <summary>
	/// All requests.
	/// </summary>
	Responses = IncomingResponse | OutgoingResponse,
	/// <summary>
	/// All message.
	/// </summary>,
	All = IncomingMessages | OutgoingMessages
}
