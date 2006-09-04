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

namespace Castle.Rook.RuntimeSupport
{
	using System;
	using System.Collections;
	using System.Reflection;

	public sealed class Dispatcher
	{
		private MethodInfo[] methods;
		private DynamicDispatchTable _dynTable;

		public Dispatcher( Type type, params MethodInfo[] methods )
		{
			_dynTable = new DynamicDispatchTable(type);

			// Build BinaryTree for methods? Key(name, argCount)
			// Or a hashtable with custom hashcodeprovider?
			this.methods = methods;
		}

		public void AddDynamicDef()
		{
			
		}

		public object Send(object instance, ref bool matched, String symbol, params object[] args)
		{
			object ret = null;

			if (instance == null)
			{
				ret = _dynTable.StaticSend( matched, symbol, args );
			}
			else
			{
				ret = _dynTable.Send( instance, matched, symbol, args );
			}

			return ret;
		}
	}
}
