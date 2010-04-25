// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System.Collections;
	using System.Reflection;
	using System.Reflection.Emit;

	/// This class works around BCL performance bug: https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=472622
	/// see DYNPROXY-ISSUE-98 for more details
	internal class WorkaroundIssue98 : ArrayList
	{
		private static readonly FieldInfo typeBuilderList = typeof(Module).GetField("m__TypeBuilderList",
		                                                                            BindingFlags.Instance |
		                                                                            BindingFlags.NonPublic);

		private static readonly PropertyInfo InternalModule = typeof(Module).GetProperty("InternalModule",
		                                                                                 BindingFlags.Instance |
		                                                                                 BindingFlags.NonPublic);

		private static readonly ArrayList instance = new WorkaroundIssue98();

		public override int Count
		{
			get { return 0; }
		}


		public override int Add(object value)
		{
			return 0;
		}

		public override void AddRange(ICollection c)
		{
		}

		public override void Insert(int index, object value)
		{
		}

		public override void InsertRange(int index, ICollection c)
		{
		}

		public static void ForModule(ModuleBuilder module)
		{
			var internalModule = InternalModule.GetValue(module, null);
			typeBuilderList.SetValue(internalModule, instance);
		}
	}
}