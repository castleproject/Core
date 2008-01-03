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
namespace Castle.MonoRail.Views.Brail
{
	using anrControls;

	public class MarkDownAttribute : OutputMethodAttribute
	{
		public override string TransformMethodName
		{
			get { return "Castle.MonoRail.Views.Brail.MarkDownAttribute.Transform"; }
		}

		public static string Transform(string original)
		{
			Markdown markdown = new Markdown();
			return markdown.Transform(original);
		}

		public static string Transform(object original)
		{
			return Transform(original.ToString());
		}
	}
}