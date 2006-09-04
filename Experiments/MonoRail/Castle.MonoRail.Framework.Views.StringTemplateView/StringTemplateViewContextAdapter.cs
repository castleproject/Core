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

namespace Castle.MonoRail.Framework.Views.StringTemplateView
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;

	using IViewComponentContext		= Castle.MonoRail.Framework.IViewComponentContext;
	using RailsException			= Castle.MonoRail.Framework.RailsException;
	using StringTemplate			= Antlr.StringTemplate.StringTemplate;
	using ConfigConstants			= Castle.MonoRail.Framework.Views.StringTemplateView.Configuration.ConfigConstants;

	public class StringTemplateViewContextAdapter : IViewComponentContext
	{
		protected readonly string componentName;
		protected TextWriter writer;
		protected string viewToRender;
		protected StringTemplate st;
		protected StAttributes2IDictionaryAdapter contextVarsAdapter;

		public StringTemplateViewContextAdapter(string componentName, StringTemplate st)
		{
			this.componentName = componentName;
			this.st = st;
			this.contextVarsAdapter = new StAttributes2IDictionaryAdapter(st);
		}

		#region IViewComponentContext

		public string ComponentName
		{
			get { return componentName; }
		}

		public IDictionary ContextVars
		{
			get { return contextVarsAdapter; }
		}

		public IDictionary ComponentParameters
		{
			get { return st.ArgumentContext; }
		}

		public string ViewToRender
		{
			get { return viewToRender; }
			set { viewToRender = value; }
		}

		public TextWriter Writer
		{
			get { return writer; }
		}

		public bool HasSection(string sectionName)
		{
			return st.ArgumentContext != null 
				&& !sectionName.Equals(ConfigConstants.COMPONENT_BODY_KEY)
				&& st.ArgumentContext.Contains(sectionName);
		}

		public void RenderBody()
		{
			RenderBody(writer);
		}

		public void RenderSection(string sectionName)
		{
			if (HasSection(sectionName))
			{
				object section = st.ArgumentContext[sectionName];
				writer.Write(section.ToString());
			}
		}

		public void RenderBody(TextWriter writer)
		{
			object body = st.ArgumentContext[ConfigConstants.COMPONENT_BODY_KEY];
			if (body == null)
			{
				throw new RailsException("This component does not have a body content to be rendered");
			}

			if (body is StringTemplate)
			{
				StringTemplate instanceST = ((StringTemplate) body).GetInstanceOf();
				instanceST.EnclosingInstance = ((StringTemplate) body).EnclosingInstance;
				body = instanceST;

				if (st.Attributes != null)
				{
					foreach(DictionaryEntry entry in st.Attributes)
					{
						((StringTemplate)body).SetAttribute(entry.Key.ToString(), entry.Value);
					}
				}
			}

			writer.Write(body.ToString());
		}

		#endregion

		internal TextWriter TextWriter
		{
			set { writer = value; }
		}

		protected class StAttributes2IDictionaryAdapter : IDictionary
		{
			protected StringTemplate st;

			public StAttributes2IDictionaryAdapter(StringTemplate st)
			{
				this.st = st;
			}

			#region IDictionary Members

			public bool IsReadOnly
			{
				get { return false; }
			}

			public IDictionaryEnumerator GetEnumerator()
			{
				if (st.Attributes == null)
					return null;
				else
					return st.Attributes.GetEnumerator();;
			}

			public object this[object key]
			{
				get
				{
					if (key is string)
					{
						return st.GetAttribute((string)key);
					}
					return null;
				}
				set
				{
					if (key is string)
					{
						st.RemoveAttribute((string)key);
						st.SetAttribute((string)key, value);
					}
					else
						throw new InvalidOperationException();
				}
			}

			public void Remove(object key)
			{
				throw new InvalidOperationException();
			}

			public bool Contains(object key)
			{
				return (st.GetAttribute((string)key) != null);
			}

			public void Clear()
			{
				throw new InvalidOperationException();
			}

			public ICollection Values
			{
				get
				{
					// TODO:  Add StAttributes2IDictionaryAdapter.Values getter implementation
					return null;
				}
			}

			public void Add(object key, object value)
			{
				throw new InvalidOperationException();
			}

			public ICollection Keys
			{
				get
				{
					// TODO:  Add StAttributes2IDictionaryAdapter.Keys getter implementation
					return null;
				}
			}

			public bool IsFixedSize
			{
				get
				{
					// TODO:  Add StAttributes2IDictionaryAdapter.IsFixedSize getter implementation
					return false;
				}
			}

			#endregion

			#region ICollection Members

			public bool IsSynchronized
			{
				get { return false; }
			}

			public int Count
			{
				get
				{
					throw new InvalidOperationException();
					//return 0;
				}
			}

			public void CopyTo(Array array, int index)
			{
				throw new InvalidOperationException();
			}

			public object SyncRoot
			{
				get
				{
					throw new InvalidOperationException();
					//return null;
				}
			}

			#endregion

			#region IEnumerable Members

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				if (st.Attributes == null)
					return null;
				else
					return st.Attributes.GetEnumerator();;
			}

			#endregion
		}
	}
}
