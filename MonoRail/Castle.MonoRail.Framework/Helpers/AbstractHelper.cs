// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;

	public abstract class AbstractHelper : IControllerAware
	{
		private Controller _controller;

		#region IControllerAware Members

		public void SetController(Controller controller)
		{
			_controller = controller;
		}

		#endregion

		public Controller Controller
		{
			get { return _controller; }
		}

		protected void MergeOptions(IDictionary userOptions, IDictionary defaultOptions)
		{
			foreach(DictionaryEntry entry in defaultOptions)
			{
				if (!userOptions.Contains(entry.Key))
				{
					userOptions[entry.Key] = entry.Value;
				}
			}
		}

		protected string GetAttributes(IDictionary attributes)
		{
			if (attributes == null) return String.Empty;

			String contents = "";

			foreach (DictionaryEntry entry in attributes)
			{
				if (entry.Value == null || entry.Value.ToString() == String.Empty)
				{
					contents += String.Format("{0} ", entry.Key);
				}
				else
				{
					contents += String.Format("{0}=\"{1}\" ", entry.Key, entry.Value);
				}
			}

			return contents;
		}
	}
}
