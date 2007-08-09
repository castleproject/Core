// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

#if NET

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections;

	public abstract class AbstractEventScope : AbstractBindingScope
	{
		private object source;
		private EventArgs eventArgs;
		private BindingContext context;

		protected AbstractEventScope(BindingContext context)
		{
			this.context = context;
		}

		protected BindingContext Context
		{
			get { return context;  }
		}

		protected void DispatchAction(object source, EventArgs e)
		{
			PushEventScope(source, e);
			context.DispatchAction();
		}

		protected void DispatchAction(object source, EventArgs e, string action)
		{
			PushEventScope(source, e);
			context.DispatchAction(action);
		}

		protected override object ResolveSymbol(string symbol)
		{
			switch(symbol)
			{
				case "this":
					return source;

				case "event":
					return eventArgs;
			}

			return null;
		}

		private void PushEventScope(object source, EventArgs e)
		{
			this.source = source;
			this.eventArgs = e;
			context.PushScope(this);
		}
	}
}

#endif
