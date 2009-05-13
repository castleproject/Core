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

namespace Castle.Facilities.WcfIntegration.Tests.Behaviors
{
	using System;
	using System.ServiceModel;
	using Castle.Facilities.WcfIntegration.Service;

	public class ServiceHostListener : AbstractServiceHostAware
	{
		private static bool created, opening, opened, closing, closed, faulted;

		public static bool CreatedCalled
		{
			get { return created; }
		}

		public static bool OpeningCalled
		{
			get { return opening; }
		}

		public static bool OpenedCalled
		{
			get { return opened; }
		}

		public static bool ClosingCalled
		{
			get { return closing; }
		}

		public static bool ClosedCalled
		{
			get { return closed; }
		}

		public static bool FaultedCalled
		{
			get { return faulted; }
		}

		public static void Reset()
		{
			created = opening = opened = closing = closed = faulted = false;	
		}

		public override void Created(ServiceHost serviceHost)
		{
			created = true;
		}

		public override void Opening(ServiceHost serviceHost)
		{
			opening = true;
		}

		public override void Opened(ServiceHost serviceHost)
		{
			opened = true;
		}

		public override void Closing(ServiceHost serviceHost)
		{
			closing = true;
		}

		public override void Closed(ServiceHost serviceHost)
		{
			closed = true;
		}

		public override void Faulted(ServiceHost serviceHost)
		{
			faulted = true;
		}
	}
}
