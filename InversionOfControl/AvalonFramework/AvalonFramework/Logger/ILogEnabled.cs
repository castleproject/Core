// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework{	using System;

	/// <summary>	/// Components that need to log can implement this interface	/// to be provided Loggers.	/// </summary>	public interface ILogEnabled	{		/// <summary>		/// Provide component with a logger.		/// </summary>		/// <param name="logger"> the logger. Must not be null.</param>		void EnableLogging(ILogger logger);	}}
