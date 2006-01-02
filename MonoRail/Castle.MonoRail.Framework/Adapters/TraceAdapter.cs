// Copyright 2004-2006 Castle Proje\ct - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.Web;

	using Castle.MonoRail.Framework;
	
	public class TraceAdapter : ITrace
	{
		private TraceContext _trace;

		public TraceAdapter( TraceContext traceContext )
		{
			_trace = traceContext;
		}

		public void Warn( String message )
		{
			_trace.Warn( message );
		}

		public void Warn( String category, String message )
		{
			_trace.Warn( category, message );
		}

		public void Warn( String category, String message, Exception errorInfo )
		{
			_trace.Warn( category, message, errorInfo );
		}

		public void Write( String message )
		{
			_trace.Write( message );
		}

		public void Write( String category, String message )
		{
			_trace.Write( category, message );
		}

		public void Write( String category, String message, Exception errorInfo )
		{
			_trace.Write( category, message, errorInfo );
		}
	}
}