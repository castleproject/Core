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
	using System.Web;
	using Castle.Core;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Views.IronView.ElementProcessor;
	using IronPython.Hosting;

	/// <summary>
	/// Pendent
	/// </summary>
	public class IronPythonViewEngine : ViewEngineBase, IInitializable, ITemplateEngine
	{
		public const String TemplateExtension = ".pml";

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

			// options.ClrDebuggingEnabled = true;
			// options.ExceptionDetail = true;
			// options.ShowClrExceptions = true;
			// options.SkipFirstLine = false;

			engine = new PythonEngine(options);
			engine.Import("site");
			System.IO.FileStream fs = new System.IO.FileStream("scripting-log.txt", System.IO.FileMode.Create);
			engine.SetStandardOutput(fs);
			engine.SetStandardError(fs);

			globalModule = engine.CreateModule("mr", false);

			// TODO: processors should be configured
			parser = new IronPythonTemplateParser(new IElementProcessor[]
			                                      	{
			                                      		new ContentTag(),
			                                      		new IfTag(),
			                                      		new UnlessTag(),
			                                      		new ForTag(),
			                                      		new CodeBlock(),
			                                      		new InlineWrite(),
			                                      		new RenderPartial(),
			                                      		new ViewComponentTag(),
			                                      	});
		}

		#endregion

		#region ViewEngineBase overrides

		public override bool HasTemplate(string templateName)
		{
			return ViewSourceLoader.HasTemplate(ResolveTemplateName(templateName));
		}

		public override void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			AdjustContentType(context);

			bool hasLayout = controller.LayoutName != null;

			TextWriter writer;

			if (hasLayout)
			{
				// Because we are rendering within a layout we need to catch it first
				writer = new StringWriter();
			}
			else
			{
				// No layout so render direct to the output
				writer = context.Response.Output;
			}

			Dictionary<string, object> locals = new Dictionary<string, object>();
			PopulateLocals(writer, context, controller, locals);

			String fileName = ResolveTemplateName(templateName);
			ProcessViewTemplate(fileName, context, locals);

			if (hasLayout)
			{
				String contents = (writer as StringWriter).GetStringBuilder().ToString();
				ProcessLayout(contents, controller, context, locals);
			}
		}

		/// <summary>
		/// Need to test this
		/// </summary>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="contents"></param>
		public override void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
//			Dictionary<string, object> locals = new Dictionary<string, object>();
//
//			PopulateLocals(context.Response.Output, context, controller, locals);
//
//			String script = parser.CreateScriptBlock(
//				new StringReader(contents), "static content", serviceProvider, this);
//
//			compiledCode = engine.Compile(script);
//
//			ExecuteScript(compiledCode, context, locals);
		}

		#endregion

		public CompiledCode CompilePartialTemplate(IViewSource source, string partialViewName, 
		                                           out string functionName, params String[] parameters)
		{
			partialViewName = partialViewName.Replace('/', '_').Replace('\\', '_').Replace('.', '_').ToLowerInvariant();
			functionName = "render" + partialViewName;
			
			ViewInfo viewInfo;
			bool compileView;
			CompiledCode compiledCode = null;

			if (name2View.TryGetValue(partialViewName.ToLower(), out viewInfo))
			{
				compileView = viewInfo.LastModified != source.LastModified;
				compiledCode = viewInfo.CompiledCode;
			}
			else
			{
				compileView = true;
			}

			Stream viewSourceStream = source.OpenViewStream();

			if (compileView)
			{
				using(StreamReader reader = new StreamReader(viewSourceStream))
				{
					String script = parser.CreateFunctionScriptBlock(reader, partialViewName,
					                                                 functionName, serviceProvider, this, parameters);

					compiledCode = engine.Compile("def something(out):\n\tout.Write('hellloo')\n");

					// Evaluates the function
					compiledCode.Execute(globalModule);
					// compiledCode.Execute();

					viewInfo = new ViewInfo();
					viewInfo.LastModified = source.LastModified;
					viewInfo.CompiledCode = compiledCode;
#if DEBUG
					viewInfo.Script = script;
#endif

					name2View[partialViewName.ToLower()] = viewInfo;
				}

#if DEBUG
				HttpContext.Current.Response.Output.Write("<!-- Partial script\r\n");
				HttpContext.Current.Response.Output.Write(viewInfo.Script);
				HttpContext.Current.Response.Output.Write("\r\n-->");
				HttpContext.Current.Response.Output.Flush();
#endif
				
				
			}

			return compiledCode;
		}

		protected string ResolveTemplateName(string templateName)
		{
			return templateName + TemplateExtension;
		}

		protected string ResolveTemplateName(string area, string templateName)
		{
			return String.Format("{0}{1}{2}", area,
			                     Path.DirectorySeparatorChar, ResolveTemplateName(templateName));
		}

		private void ProcessLayout(string innerViewContents, Controller controller,
		                           IRailsEngineContext context, Dictionary<string, object> locals)
		{
			String layout = ResolveTemplateName("layouts", controller.LayoutName);

			locals["childContent"] = innerViewContents;
			locals["output"] = context.Response.Output;

			ProcessViewTemplate(layout, context, locals);
		}

		private void ProcessViewTemplate(String templateFile, IRailsEngineContext context,
		                                 Dictionary<string, object> locals)
		{
			IViewSource source = ViewSourceLoader.GetViewSource(templateFile);

			if (source == null)
			{
				throw new RailsException("Could not find view template: " + templateFile);
			}

			CompiledCode compiledCode = CompileTemplate(source, templateFile);

			ExecuteScript(compiledCode, context, locals);
		}

		private CompiledCode CompileTemplate(IViewSource source, string templateFile)
		{
			bool compileView;
			ViewInfo viewInfo;
			CompiledCode compiledCode = null;

			if (name2View.TryGetValue(templateFile.ToLower(), out viewInfo))
			{
				compileView = viewInfo.LastModified != source.LastModified;
				compiledCode = viewInfo.CompiledCode;
			}
			else
			{
				compileView = true;
			}

			Stream viewSourceStream = source.OpenViewStream();

			if (compileView)
			{
				using(StreamReader reader = new StreamReader(viewSourceStream))
				{
					String script = parser.CreateScriptBlock(reader, templateFile, serviceProvider, this);

					compiledCode = engine.Compile(script);

					viewInfo = new ViewInfo();
					viewInfo.LastModified = source.LastModified;
					viewInfo.CompiledCode = compiledCode;
#if DEBUG
					viewInfo.Script = script;
#endif

					name2View[templateFile.ToLower()] = viewInfo;
				}

#if DEBUG
				HttpContext.Current.Response.Output.Write("<!-- Template script \r\n");
				HttpContext.Current.Response.Output.Write(viewInfo.Script);
				HttpContext.Current.Response.Output.Write("\r\n-->");
				HttpContext.Current.Response.Output.Flush();
#endif
			}

			return compiledCode;
		}

		private void ExecuteScript(CompiledCode script,
		                           IRailsEngineContext context,
		                           Dictionary<string, object> locals)
		{
			NullLocalDecorator decorator = new NullLocalDecorator(locals);

			try
			{
				script.Execute(globalModule, decorator);
			}
			catch(Exception ex)
			{
				context.Response.Write("<p>");
				context.Response.Write(ex.Message);
				context.Response.Write("</p>");
				context.Response.Write(ex.InnerException);
				context.Response.Write("<pre>");
				context.Response.Write(ex.StackTrace);
				context.Response.Write("</pre>");
			}
		}

		private static void PopulateLocals(TextWriter output, IRailsEngineContext context,
		                                   Controller controller,
		                                   Dictionary<string, object> locals)
		{
			locals["controller"] = controller;
			locals["context"] = context;
			locals["request"] = context.Request;
			locals["response"] = context.Response;
			locals["session"] = context.Session;
			locals["output"] = output;

			if (controller.Resources != null)
			{
				foreach(String key in controller.Resources.Keys)
				{
					locals[key] = controller.Resources[key];
				}
			}

			foreach(object key in controller.Helpers.Keys)
			{
				locals[key.ToString()] = controller.Helpers[key];
			}

			foreach(DictionaryEntry entry in controller.PropertyBag)
			{
				locals[entry.Key.ToString()] = entry.Value;
			}

			foreach(String key in context.Params.AllKeys)
			{
				if (key == null) continue; // Nasty bug?
				object value = context.Params[key];
				if (value == null) continue;
				locals[key] = value;
			}

			locals[Flash.FlashKey] = context.Flash;

			foreach(DictionaryEntry entry in context.Flash)
			{
				if (entry.Value == null) continue;
				locals[entry.Key.ToString()] = entry.Value;
			}

			locals["siteroot"] = context.ApplicationPath;
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