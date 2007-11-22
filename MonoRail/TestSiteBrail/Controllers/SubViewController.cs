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

namespace Castle.MonoRail.Views.Brail.TestSite.Controllers
{
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Views.Brail;
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;

	[Serializable]
	public class SubViewController : Controller
	{
		public void Index()
		{
		}

		public void SubViewWithLayout()
		{
			this.LayoutName = "master";
			this.RenderView("index");
		}

		public void SubViewWithParameters()
		{
			this.RenderView("CallSubViewWithParameters");
		}

		public void SubViewWithPath()
		{
		}

		public void useLotsOfSubViews()
		{
			//this is ugly, but the other way is to open up things that really shouldn't be opened...
			IViewEngineManager viewEngineManager = (IViewEngineManager)this.ServiceProvider.GetService(typeof(IViewEngineManager));
			MethodInfo method = viewEngineManager.GetType().GetMethod("ResolveEngine", BindingFlags.Instance|BindingFlags.NonPublic, null,new Type[] { typeof(string) },new ParameterModifier[0]);
			IViewEngine engine1 = (IViewEngine)method.Invoke(viewEngineManager, new object[] { "dummy.brail" });
			Hashtable hashtable1 = (Hashtable)typeof(BooViewEngine).GetField("compilations", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(engine1);
			Hashtable hashtable2 = (Hashtable)typeof(BooViewEngine).GetField("constructors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(engine1);
			if (this.Context.Request.QueryString["replaceSubView"] == "reset")
			{
				hashtable1[@"subview\listItem.brail"] = null;
			}
			else if (this.Context.Request.QueryString["replaceSubView"] == "true")
			{
				hashtable1[@"subview\listItem.brail"] = typeof(DummySubView);
				Type[] typeArray1 = new Type[] { typeof(BooViewEngine), typeof(TextWriter), typeof(IRailsEngineContext), typeof(Controller) };
				hashtable2[typeof(DummySubView)] = typeof(DummySubView).GetConstructor(typeArray1);
			}
		}

	}
}

