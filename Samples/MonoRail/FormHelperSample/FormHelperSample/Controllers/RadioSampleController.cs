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

namespace FormHelperSample.Controllers
{
	using System.IO;
	using Castle.MonoRail.Framework;

	[Layout("default"), Rescue("generalerror")]
	public class RadioSampleController : SmartDispatcherController
	{
		public void Radio()
		{
			// Values to preselect
			
			PropertyBag["mode"] = FileMode.Truncate;
			PropertyBag["agreement"] = false;
		}
		
		public void Process(FileMode mode, bool agreement)
		{
			PropertyBag["mode"] = mode;
			PropertyBag["agreement"] = agreement;
			
			RenderView("result");
		}
	}
}
