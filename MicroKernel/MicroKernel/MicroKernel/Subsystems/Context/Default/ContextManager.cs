// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Subsystems.Context.Default
{
	using System;

	using Apache.Avalon.Framework;

	/// <summary>
	/// Summary description for ContextManager.
	/// </summary>
	public class ContextManager : AbstractSubsystem, IContextManager
	{
		public ContextManager()
		{
			
		}

		#region IContextManager Members

		public IContext CreateContext(AvalonContextAttribute contextAtt, AvalonEntryAttribute[] entries)
		{
			if (contextAtt != null)
			{
				return CreateCustomContext(contextAtt, entries);
			}
			else
			{
				return CreateDefaultContext(entries);
			}
		}
		
		#endregion

		protected IContext CreateCustomContext(AvalonContextAttribute contextAtt, AvalonEntryAttribute[] entries)
		{
			return null;
		}

		protected IContext CreateDefaultContext(AvalonEntryAttribute[] entries)
		{
			DefaultContext context = new DefaultContext();

			/*
			foreach(AvalonEntryAttribute entry in entries)
			{
				
			}*/

			context.MakeReadOnly();

			return context;
		}
	}
}
