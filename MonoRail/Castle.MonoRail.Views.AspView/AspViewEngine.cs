// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Configuration;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text.RegularExpressions;
	using System.Collections.Generic;
	using Framework.Configuration;
	using Compiler;
	using Compiler.Factories;
	using Framework;
	using Core;

	public class AspViewEngine : ViewEngineBase, IInitializable, IAspViewEngineTestAccess
	{
		private List<ICompilationContext> compilationContexts = new List<ICompilationContext>();
	
		private IMonoRailConfiguration monoRailConfiguration;
		static bool needsRecompiling;
		static AspViewEngineOptions options;
		static readonly Regex invalidClassNameCharacters = new Regex("[^a-zA-Z0-9\\-*#=/\\\\_.]", RegexOptions.Compiled);

		readonly Hashtable compilations = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));

		#region IAspViewEngineTestAccess
		Hashtable IAspViewEngineTestAccess.Compilations
		{
			get { return compilations; }
		}
		void IAspViewEngineTestAccess.SetViewSourceLoader(IViewSourceLoader viewSourceLoader)
		{
			ViewSourceLoader = viewSourceLoader;
		}
		void IAspViewEngineTestAccess.SetCompilationContext(List<ICompilationContext> contexts)
		{
			compilationContexts = contexts;
		}

		#endregion

		#region IInitializable Members

		public void Initialize(List<ICompilationContext> contexts, AspViewEngineOptions newOptions)
		{
			options = newOptions;
			compilationContexts = contexts;
			Initialize();
		}

		public void Initialize()
		{
			if (options == null)
				InitializeConfig();

			if (compilationContexts.Count == 0)
			{
				string siteRoot = AppDomain.CurrentDomain.BaseDirectory;
				
				compilationContexts.Add(
					new WebCompilationContext(
						monoRailConfiguration.ViewEngineConfig.ViewPathRoot,
						new DirectoryInfo(siteRoot), 
						new DirectoryInfo(options.CompilerOptions.TemporarySourceFilesDirectory)));

				foreach (string path in monoRailConfiguration.ViewEngineConfig.PathSources) 
				{
					compilationContexts.Add(
						new WebCompilationContext(
							path,
							new DirectoryInfo(siteRoot),
							new DirectoryInfo(options.CompilerOptions.TemporarySourceFilesDirectory)));
				}
			}

			LoadCompiledViews();

			if (options.CompilerOptions.AutoRecompilation)
			{
				// invalidate compiled views cache on any change to the view sources
				ViewSourceLoader.ViewChanged += delegate(object sender, FileSystemEventArgs e)
												{
													foreach(string extension in AbstractCompiler.TemplateExtensions)
													{
														if (e.Name.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
														{
															needsRecompiling = true;
														}
													}
												};
			}
		}
		#endregion

		#region ViewEngineBase implementation

		public override void Service(IServiceProvider provider)
		{
			base.Service(provider);

			monoRailConfiguration = (IMonoRailConfiguration) provider.GetService(typeof (IMonoRailConfiguration));
		}
		public override bool HasTemplate(string templateName)
		{
			string className = GetClassName(templateName);
			return compilations.ContainsKey(className);
		}

		public override void Process(string templateName, string layoutName, TextWriter output, IDictionary<string, object> parameters)
		{
			ControllerContext controllerContext = new ControllerContext();
			if (layoutName != null)
			{
				controllerContext.LayoutNames = new string[] { layoutName };
			}
			foreach (KeyValuePair<string, object> pair in parameters)
			{
				controllerContext.PropertyBag[pair.Key] = pair.Value;
			}
			
			Process(templateName, output, null, null, controllerContext);
		}

		public override void Process(string templateName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			IViewBaseInternal view = GetView(templateName, output, context, controller, controllerContext);
			if (controllerContext.LayoutNames != null)
			{
				string[] layoutNames = controllerContext.LayoutNames;
				for (int i = layoutNames.Length - 1; i >= 0; --i)
				{
					string layoutName = layoutNames[i].Trim();
					IViewBaseInternal layout = GetLayout(layoutName, output, context, controller, controllerContext);
					layout.ContentView = view;
					view = layout;
				}
			}
			if (controller != null)
			{
				controller.PreSendView(view);
			}
			view.Process();
			if (controller != null)
			{
				controller.PostSendView(view);
			}
		}

		public override void ProcessPartial(string partialName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override string ViewFileExtension
		{
			get { return "aspx"; }
		}

		private string LayoutFileExtension
		{
			get { return "master"; }
		}

		#region NJS
		public override string JSGeneratorFileExtension
		{
			get { throw new AspViewException("This version of AspView does not implements NJS."); }
		}

		public override bool SupportsJSGeneration
		{
			get { return false; }
		}

		#endregion
		#endregion

		public AspViewBase CreateView(Type type, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			AspViewBase view = (AspViewBase)FormatterServices.GetUninitializedObject(type);
			view.Initialize(this, output, context, controller, controllerContext);
			return view;
		}

		public virtual AspViewBase GetView(string templateName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			templateName = NormalizeFileName(templateName);
			string className = GetClassName(templateName);
			if (needsRecompiling)
			{
				CompileViewsInMemory();
				needsRecompiling = false;
			}

			Type viewType = compilations[className] as Type;

			if (viewType == null)
				throw new AspViewException("Cannot find view type for {0}.",
					templateName);
			// create a view instance
			AspViewBase theView;
			try
			{
				theView = CreateView(viewType, output, context, controller, controllerContext);
			}
			catch (Exception ex)
			{
				throw new AspViewException(ex, "Cannot create view instance from '{0}'.", templateName);
			}
			if (theView == null)
				throw new AspViewException(string.Format(
					"Cannot find view '{0}'", templateName));
			return theView;
		}

		protected virtual AspViewBase GetLayout(string layoutName, TextWriter output, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			string layoutTemplate = "layouts\\" + layoutName;
			if (layoutName.StartsWith("\\"))
				layoutTemplate = layoutName;
			return GetView(layoutTemplate, output, context, controller, controllerContext);
		}

		protected virtual void CompileViewsInMemory()
		{
			compilations.Clear();

			foreach (ICompilationContext compilationContext in compilationContexts) 
			{
				OnlineCompiler compiler = new OnlineCompiler(
					new CSharpCodeProviderAdapterFactory(),
					new PreProcessor(),
					compilationContext,
					options.CompilerOptions);

				LoadCompiledViewsFrom(compiler.Execute());
			}
		}

		private void CacheViewType(Type viewType)
		{
			compilations[viewType.Name] = viewType;
		}

		private void LoadCompiledViews()
		{
			if (options.CompilerOptions.AutoRecompilation)
			{
				CompileViewsInMemory();
				return;
			}
			compilations.Clear();

			List<string> viewAssemblies = new List<string>();

			foreach (ICompilationContext compilationContext in compilationContexts) {
				viewAssemblies.AddRange(
					Directory.GetFiles(Path.Combine(compilationContext.SiteRoot.FullName, "bin"), "*CompiledViews.dll",
									   SearchOption.TopDirectoryOnly)
					);
			}

			foreach (string assembly in viewAssemblies)
			{
				Assembly precompiledViews;

				try
				{
					precompiledViews = Assembly.LoadFile(Path.GetFullPath(assembly));
				}
				catch (Exception e)
				{
					string error = string.Format("Could not load views assembly [{0}]", assembly);
					Logger.ErrorFormat(error);
					throw new InvalidOperationException(error, e);
				}

				if (precompiledViews != null)
					LoadCompiledViewsFrom(precompiledViews);
			}

		}

		private void LoadCompiledViewsFrom(Assembly viewsAssembly)
		{
			if (viewsAssembly == null)
				return;
			try
			{
				foreach (Type type in viewsAssembly.GetTypes())
					CacheViewType(type);
			}
			catch (ReflectionTypeLoadException rtle)
			{
				string loaderErrors = "";
				foreach (Exception loaderException in rtle.LoaderExceptions)
				{
					loaderErrors += loaderException + Environment.NewLine;
				}

				Logger.Error(loaderErrors);
				throw new InvalidOperationException("Could not load type from views assembly because: "+ Environment.NewLine + loaderErrors, rtle);
			}
		}

		public static string GetClassName(string fileName)
		{
			fileName = fileName.ToLower();
			if (Path.HasExtension(fileName))
			{
				int lastDotIndex = fileName.LastIndexOf('.');
				fileName = fileName.Substring(0, lastDotIndex);
			}

			fileName = invalidClassNameCharacters.Replace(fileName, "_");

			string className = fileName
				.Replace('\\', '_')
				.Replace('/', '_')
				.Replace("-", "_dash_")
				.Replace("=", "_equals_")
				.Replace("*", "_star_")
				.Replace("#", "_sharp_")
				.Replace("=", "_equals_")
				.TrimStart('_')
				.Replace('.', '_');

			return className;
		}

		public static string NormalizeFileName(string fileName)
		{
			return fileName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
		}

		private static void InitializeConfig()
		{
			InitializeConfig("aspView");
			if (options == null)
				InitializeConfig("aspview");
			if (options == null)
				options = new AspViewEngineOptions();
		}

		private static void InitializeConfig(string configName)
		{
			options = (AspViewEngineOptions)ConfigurationManager.GetSection(configName);
		}

		///<summary>
		/// Implementors should return a generator instance if
		/// the view engine supports JS generation.
		///</summary>
		///
		///<param name="generatorInfo">The generator info.</param>
		///<param name="context">The request context.</param>
		///<param name="controller">The controller.</param>
		///<param name="controllerContext">The controller context.</param>
		///<returns>
		///A JS generator instance
		///</returns>
		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context,
												 IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		///<summary>
		/// Processes the js generation view template - using the templateName
		/// to obtain the correct template, and using the specified <see cref="T:System.IO.TextWriter" />
		/// to output the result.
		///</summary>
		///
		///<param name="templateName">Name of the template.</param>
		///<param name="output">The output.</param>
		///<param name="generatorInfo">The generator info.</param>
		///<param name="context">The request context.</param>
		///<param name="controller">The controller.</param>
		///<param name="controllerContext">The controller context.</param>
		public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo,
										IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}

		///<summary>
		/// Wraps the specified content in the layout using the 
		/// context to output the result.
		///</summary>
		///
		public override void RenderStaticWithinLayout(string contents, IEngineContext context, IController controller,
													  IControllerContext controllerContext)
		{
			throw new NotImplementedException();
		}
	}
}
