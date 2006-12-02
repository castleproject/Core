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

namespace Castle.MonoRail.Views.IronView
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization;
	using Castle.Core;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Views.IronView.ElementProcessor;
	using IronPython.Hosting;
	
	/// <summary>
	/// Pendent
	/// </summary>
	public class IronPythonViewEngine : ViewEngineBase, IInitializable
	{
		internal const String TemplateExtension = ".pml";
		
		private PythonEngine engine;
		private EngineModule globalModule;
		private IronPythonTemplateParser parser;
		private Dictionary<string, ViewInfo> name2View;

		/// <summary>
		/// Initializes a new instance of the <see cref="IronPythonViewEngine"/> class.
		/// </summary>
		public IronPythonViewEngine()
		{
			name2View = new Dictionary<string, ViewInfo>();
		}

		#region IInitializable
		
		public void Initialize()
		{
			EngineOptions options = new EngineOptions();
			
			options.ClrDebuggingEnabled = true;
			options.ExceptionDetail = true;
			options.ShowClrExceptions = true;
			options.SkipFirstLine = false;

			engine = new PythonEngine(options);

			globalModule = engine.CreateModule("viewengine", false);

			// TODO: processors should be configured
			// TODO: Create render and viewcomponent processors
			parser = new IronPythonTemplateParser(new IElementProcessor[]
			                                      	{
			                                      		new IfTag(),
			                                      		new UnlessTag(),
			                                      		new ForTag(),
			                                      		new CodeBlock(),
			                                      		new InlineWrite()
			                                      	});
		}

		#endregion

		public override bool HasTemplate(string templateName)
		{
			return ViewSourceLoader.HasTemplate(ResolveTemplateName(templateName));
		}

		public override void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			String fileName = ResolveTemplateName(templateName);

			IViewSource source = ViewSourceLoader.GetViewSource(fileName);
			
			Stream viewSourceStream = source.OpenViewStream();
			
			using(StreamReader reader = new StreamReader(viewSourceStream))
			{
				bool compileView; ViewInfo viewInfo; 
				CompiledCode compiledCode = null;

				if (name2View.TryGetValue(templateName.ToLower(), out viewInfo))
				{
					compileView = viewInfo.LastModified != source.LastModified;
					compiledCode = viewInfo.CompiledCode;
				}
				else
				{
					compileView = true;
				}

				if (compileView)
				{
					String script = parser.CreateScriptBlock(reader, fileName, serviceProvider);

					compiledCode = CompileScript(script);
					
					viewInfo = new ViewInfo();
					viewInfo.LastModified = source.LastModified;
					viewInfo.CompiledCode = compiledCode;
#if DEBUG
					viewInfo.Script = script;
#endif
					
					name2View[templateName.ToLower()] = viewInfo;
				}
				
#if DEBUG
				context.Response.Write("<!--\r\n");
				context.Response.Write(viewInfo.Script);
				context.Response.Write("\r\n-->");
#endif

				ExecuteScript(compiledCode, context, controller);
			}
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Resolves the template name into a velocity template file name.
		/// </summary>
		protected virtual string ResolveTemplateName(string templateName)
		{
			return templateName + TemplateExtension;
		}

		private CompiledCode CompileScript(string script)
		{
			return engine.Compile(script);
		}

		private void ExecuteScript(CompiledCode script, IRailsEngineContext context, Controller controller)
		{
			Dictionary<string, object> locals = new Dictionary<string, object>();

			locals["controller"] = controller;
			locals["context"] = context;
			locals["output"] = context.Response.Output;

			foreach (DictionaryEntry entry in controller.PropertyBag)
			{
				locals[entry.Key.ToString()] = entry.Value;
			}

			NullLocalDecorator decorator = new NullLocalDecorator(locals);

			try
			{
				script.Execute(globalModule, decorator);
			}
			catch (Exception ex)
			{
				context.Response.Write("<p>");
				context.Response.Write(ex.Message);
				context.Response.Write("</p>");
				context.Response.Write("<pre>");
				context.Response.Write(ex.StackTrace);
				context.Response.Write("</pre>");
			}
		}
	}
	
	internal class ViewInfo
	{
		public long LastModified;
		public CompiledCode CompiledCode;
		public String Script;
	}

	/// <summary>
	/// This class prevents a request to a 
	/// undefined local variable. 
	/// </summary>
	internal class NullLocalDecorator : IDictionary<string, object>
	{
		private readonly Dictionary<string, object> locals;

		public NullLocalDecorator(Dictionary<string, object> locals)
		{
			this.locals = locals;
		}

		#region IDictionary<string, object>

		public void Add(string key, object value)
		{
			locals.Add(key, value);
		}

		public void Clear()
		{
			locals.Clear();
		}

		public bool ContainsKey(string key)
		{
			return locals.ContainsKey(key);
		}

		public bool ContainsValue(object value)
		{
			return locals.ContainsValue(value);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			locals.GetObjectData(info, context);
		}

		public void OnDeserialization(object sender)
		{
			locals.OnDeserialization(sender);
		}

		public bool Remove(string key)
		{
			return locals.Remove(key);
		}

		public bool TryGetValue(string key, out object value)
		{
			if (!locals.ContainsKey(key))
			{
				value = null;
				return true;
			}
			
			return locals.TryGetValue(key, out value);
		}

		public IEqualityComparer<string> Comparer
		{
			get { return locals.Comparer; }
		}

		public int Count
		{
			get { return locals.Count; }
		}

		public object this[string key]
		{
			get { return locals[key]; }
			set { locals[key] = value; }
		}

		public ICollection<string> Keys
		{
			get { return locals.Keys; }
		}

		public ICollection<object> Values
		{
			get { return locals.Values; }
		}

		#endregion

		#region ICollection

		public void Add(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return locals.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return locals.GetEnumerator();
		}

		#endregion
	}
}
