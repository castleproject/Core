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

namespace Castle.MonoRail.ActiveRecordScaffold
{
	using System;

	using Castle.ActiveRecord;
	using Castle.MonoRail.Framework;


	public class ScaffoldingSupport : IScaffoldingSupport
	{
		public void Process(Controller controller)
		{
			object[] attrs = controller.GetType().GetCustomAttributes( typeof(ScaffoldingAttribute), false );

			ScaffoldingAttribute scaffoldAtt = attrs[0] as ScaffoldingAttribute;

			String name = scaffoldAtt.Model.Name;

			controller.CustomActions[ String.Format("new{0}", name) ] = new NewAction( scaffoldAtt.Model ); 
		}
	}

	public class NewAction : IDynamicAction
	{
		private readonly Type modelType;

		public NewAction( Type modelType )
		{
			this.modelType = modelType;
		}

		public void Execute(Controller controller)
		{
			controller.RenderText( modelType.FullName );
		}
	}
}