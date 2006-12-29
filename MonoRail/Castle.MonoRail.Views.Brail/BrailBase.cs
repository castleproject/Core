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

using System.Diagnostics;
using System.Text;

namespace Castle.MonoRail.Views.Brail
{
	using System;
	using System.Collections;
	using System.IO;
	using Castle.MonoRail.Framework;
	// Base class for all the view scripts, this is the class that is responsible for
	// support all the behind the scenes magic such as variable to PropertyBag trasnlation, 
	// resources usage, etc.
	public abstract class BrailBase
	{
		private TextWriter outputStream;

		// This is used by layout scripts only, for outputing the child's content
		protected TextWriter childOutput;
		private Hashtable properties;
		// list of dictionaries where additional properties can be stored.
		private ArrayList extendedProperties = new ArrayList();

		protected IRailsEngineContext context;
		protected Controller controller;
		protected BooViewEngine viewEngine;

		//usually used by the layout to refer to its view
		protected BrailBase parent;

		public BrailBase(BooViewEngine viewEngine, TextWriter output, IRailsEngineContext context, Controller controller)
		{
			this.viewEngine = viewEngine;
			outputStream = output;
			this.context = context;
			this.controller = controller;
			InitProperties(context, controller);
		}

		public abstract void Run();

		// The path of the script, this is filled by AddBrailBaseClassStep
		// and is used for sub views
		public virtual string ScriptDirectory
		{
			get { return viewEngine.ViewRootDir; }
		}

		public BooViewEngine ViewEngine
		{
			get { return viewEngine; }
		}

		// Output the subview to the client, this is either a relative path "SubView" which
		// is relative to the current /script/ or an "absolute" path "/home/menu" which is
		// actually relative to ViewDirRoot
		public void OutputSubView(string subviewName)
		{
			OutputSubView(subviewName, new Hashtable());
		}

		// Similiar to the above function, but with a bunch of parameters that are used
		// just for this subview. This parameters are /not/ inheritable.
		public void OutputSubView(string subviewName, IDictionary parameters)
		{
			string subViewFileName = GetSubViewFilename(subviewName);
			BrailBase subView = viewEngine.GetCompiledScriptInstance(subViewFileName, outputStream, context, controller);
			foreach(DictionaryEntry entry in parameters)
			{
				subView.properties[entry.Key] = entry.Value;
			}
			subView.Run();
		}

		// Get the sub view file name, if the subview starts with a '/' 
		// then the filename is considered relative to ViewDirRoot
		// otherwise, it's relative to the current script directory
		public string GetSubViewFilename(string subviewName)
		{
			//absolute path from Views directory
            if (subviewName[0] == '/')
                return subviewName.Substring(1) + viewEngine.ViewFileExtension;
			return Path.Combine(ScriptDirectory, subviewName) + viewEngine.ViewFileExtension;
		}

		// this is called by ReplaceUnknownWithParameters step to create a more dynamic experiance
		// any uknown identifier will be translate into a call for GetParameter('identifier name').
		// This mean that when an uknonwn identifier is in the script, it will only be found on runtime.
		public object GetParameter(string name)
		{
			ParameterSearch search = GetParameterInternal(name);
			if (search.Found == false)
				throw new RailsException("Parameter '" + name + "' was not found!");
			return search.Value;
		}

		// this is called by ReplaceUnknownWithParameters step to create a more dynamic experiance
		// any uknown identifier with the prefix of ? will be translated into a call for 
		// TryGetParameter('identifier name without the ? prefix').
		// This method will return null if the value it not found.
		public object TryGetParameter(string name)
		{
			ParameterSearch search = GetParameterInternal(name);
			return search.Value;
		}

		private ParameterSearch GetParameterInternal(string name)
		{
			if (properties.Contains(name))
				return new ParameterSearch(properties[name], true);
			foreach(IDictionary extendedProperty in extendedProperties)
			{
				if (extendedProperty.Contains(name))
					return new ParameterSearch(extendedProperty[name], true);
			}
			if (parent != null)
				return parent.GetParameterInternal(name);
			return new ParameterSearch(null, false);
		}

		public void AddProperties(IDictionary moreProperties)
		{
			extendedProperties.Add(moreProperties);
		}

		public void SetParent(BrailBase myParent)
		{
			parent = myParent;
		}

		// Allows to check that a parameter was defined
		public bool IsDefined(string name)
		{
			ParameterSearch search = GetParameterInternal(name);
			return search.Found;
		}

		// allows explicit access to the Flash
		public Flash Flash
		{
			get { return context.Flash; }
		}

		public TextWriter OutputStream
		{
			get { return outputStream; }
		}

		/// <summary>
		/// This is required because we may want to replace the output stream and get the correct
		/// behavior from components call RenderText() or RenderSection()
		/// </summary>
		public IDisposable SetOutputStream(TextWriter newOutputStream)
		{
			ReturnOutputStreamToInitialWriter disposable = new ReturnOutputStreamToInitialWriter(OutputStream, this);
			this.outputStream = newOutputStream;
			return disposable;
		}

		public TextWriter ChildOutput
		{
			get { return childOutput;  }
			set {  childOutput = value;  }
		}
	    
	    internal void ExtendDictionaryWithProperties(IDictionary dictionary)
	    {
	        foreach(DictionaryEntry entry in properties)
	        {
                dictionary[entry.Key] = entry.Value;
	        }
	    }

        /// <summary>
        /// Note that this will overwrite any existing property.
        /// </summary>
        public void AddProperty(string name, object item)
        {
            properties[name] = item;
        }

		// Initialize all the properties that a script may need
		// One thing to note here is that resources are wrapped in ResourceToDuck wrapper
		// to enable easy use by the script
		private void InitProperties(IRailsEngineContext myContext, Controller myController)
		{
			properties = new Hashtable(
#if DOTNET2
				StringComparer.InvariantCultureIgnoreCase
#else
				CaseInsensitiveHashCodeProvider.Default,
				CaseInsensitiveComparer.Default
#endif
				);

			properties.Add("request", myContext.Request);
			properties.Add("response", myContext.Response);
			properties.Add("session", myContext.Session);

			if (myController.Resources != null)
			{
				foreach(object key in myController.Resources.Keys)
				{
					properties.Add(key, new ResourceToDuck(myController.Resources[key]));
				}
			}

			foreach(DictionaryEntry entry in myController.Helpers)
			{
				properties.Add(entry.Key, entry.Value);
			}

			foreach(string key in myController.Params.AllKeys)
			{
				if (key == null)
					continue;
				properties[key] = myContext.Params[key];
			}

			foreach(DictionaryEntry entry in myContext.Flash)
			{
				properties[entry.Key] = entry.Value;
			}

			foreach(DictionaryEntry entry in myController.PropertyBag)
			{
				properties[entry.Key] = entry.Value;
			}

			properties["siteRoot"] = myContext.ApplicationPath;
		}

		private class ParameterSearch
		{
			private bool found;
			private object value;

			public bool Found
			{
				get { return found; }
			}

			public object Value
			{
				get { return value; }
			}

			public ParameterSearch(object value, bool found)
			{
				this.found = found;
				this.value = value;
			}
		}

		private class ReturnOutputStreamToInitialWriter : IDisposable
		{
			private TextWriter initialWriter;
			private BrailBase parent;

			public ReturnOutputStreamToInitialWriter(TextWriter initialWriter, BrailBase parent)
			{
				this.initialWriter = initialWriter;
				this.parent = parent;
			}

			public void Dispose()
			{
				parent.outputStream = initialWriter;
			}
		}
	}
}
