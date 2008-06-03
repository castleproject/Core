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

namespace Castle.MonoRail.Views.Brail
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Threading;
	using System.Web;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Pipelines;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Parser;
	using Castle.Core.Logging;
	using Core;
	using Framework;

	public class BooViewEngine : ViewEngineBase, IInitializable
	{
		public event Action<string> ViewRecompiled = delegate { };

		private static BooViewEngineOptions options;

		/// <summary>
		/// This field holds all the cache of all the 
		/// compiled types (not instances) of all the views that Brail nows of.
		/// </summary>
		private readonly Hashtable compilations = Hashtable.Synchronized(
			new Hashtable(StringComparer.InvariantCultureIgnoreCase));

		/// <summary>
		/// used to hold the constructors of types, so we can avoid using
		/// Activator (which takes a long time
		/// </summary>
		private readonly Hashtable constructors = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Used to map between type and file name, this is useful when we
		/// want to remove a script by its type.
		/// </summary>
		private readonly Hashtable typeToFileName = Hashtable.Synchronized(new Hashtable());

		private string baseSavePath;

		/// <summary>
		/// This is used to add a reference to the common scripts for each compiled scripts
		/// </summary>
		private Assembly common;

		private ILogger logger;

		public override bool SupportsJSGeneration
		{
			get { return true; }
		}

		public override string ViewFileExtension
		{
			get { return ".brail"; }
		}

		public override string JSGeneratorFileExtension
		{
			get { return ".brailjs"; }
		}

		public string ViewRootDir
		{
			get { return ViewSourceLoader.ViewRootDir; }
		}

		public BooViewEngineOptions Options
		{
			get { return options; }
			set { options = value; }
		}

		#region IInitializable Members

		public void Initialize()
		{
			if (options == null)
			{
				InitializeConfig();
			}

			string baseDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
			Log("Base Directory: " + baseDir);
			baseSavePath = Path.Combine(baseDir, options.SaveDirectory);
			Log("Base Save Path: " + baseSavePath);

			if (options.SaveToDisk && !Directory.Exists(baseSavePath))
			{
				Directory.CreateDirectory(baseSavePath);
				Log("Created directory " + baseSavePath);
			}

			CompileCommonScripts();

			ViewSourceLoader.ViewChanged += OnViewChanged;
		}

		#endregion

		// Process a template name and output the results to the user
		// This may throw if an error occured and the user is not local (which would 
		// cause the yellow screen of death)
		public override void Process(String templateName, TextWriter output, IEngineContext context, IController controller,
									 IControllerContext controllerContext)
		{
			Log("Starting to process request for {0}", templateName);
			string file = templateName + ViewFileExtension;
			BrailBase view;
			// Output may be the layout's child output if a layout exists
			// or the context.Response.Output if the layout is null
			LayoutViewOutput layoutViewOutput = GetOutput(output, context, controller, controllerContext);
			// Will compile on first time, then save the assembly on the cache.
			view = GetCompiledScriptInstance(file, layoutViewOutput.Output, context, controller, controllerContext);
			if (controller != null)
			{
				controller.PreSendView(view);
			}

			Log("Executing view {0}", templateName);

			RenderView(templateName, view);

			layoutViewOutput.RenderLayouts(this, view);

			Log("Finished executing view {0}", templateName);
			if (controller != null)
			{
				controller.PostSendView(view);
			}
		}

		private void RenderView(string templateName, BrailBase view)
		{
			try
			{
				view.Run();
			}
			catch (Exception e)
			{
				HandleException(templateName, view, e);
			}
		}

		public override void Process(string templateName, string layoutName, TextWriter output,
									 IDictionary<string, object> parameters)
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
			;
			Process(templateName, output, null, null, controllerContext);
		}

		public override void ProcessPartial(string partialName, TextWriter output, IEngineContext context,
											IController controller, IControllerContext controllerContext)
		{
			Log("Generating partial for {0}", partialName);

			try
			{
				string file = ResolveTemplateName(partialName);
				BrailBase view = GetCompiledScriptInstance(file, output, context, controller, controllerContext);
				Log("Executing partial view {0}", partialName);
				view.Run();
				Log("Finished executing partial view {0}", partialName);
			}
			catch (Exception ex)
			{
				if (Logger != null && Logger.IsErrorEnabled)
				{
					Logger.Error("Could not generate JS", ex);
				}

				throw new MonoRailException("Error generating partial: " + partialName, ex);
			}
		}

		public override object CreateJSGenerator(JSCodeGeneratorInfo generatorInfo, IEngineContext context,
												 IController controller,
												 IControllerContext controllerContext)
		{
			return new BrailJSGenerator(generatorInfo.CodeGenerator, generatorInfo.LibraryGenerator,
										generatorInfo.Extensions, generatorInfo.ElementExtensions);
		}

		public override void GenerateJS(string templateName, TextWriter output, JSCodeGeneratorInfo generatorInfo,
										IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			Log("Generating JS for {0}", templateName);

			try
			{
				object generator = CreateJSGenerator(generatorInfo, context, controller, controllerContext);
				AdjustJavascriptContentType(context);
				string file = ResolveJSTemplateName(templateName);
				BrailBase view = GetCompiledScriptInstance(file,
					//we use the script just to build the generator, not to output to the user
														   new StringWriter(),
														   context, controller, controllerContext);
				Log("Executing JS view {0}", templateName);
				view.AddProperty("page", generator);
				view.Run();

				output.WriteLine(generator);
				Log("Finished executing JS view {0}", templateName);
			}
			catch (Exception ex)
			{
				if (Logger != null && Logger.IsErrorEnabled)
				{
					Logger.Error("Could not generate JS", ex);
				}

				throw new MonoRailException("Error generating JS. Template: " + templateName, ex);
			}
		}

		/// <summary>
		/// Wraps the specified content in the layout using the
		/// context to output the result.
		/// </summary>
		/// <param name="contents"></param>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="controllerContext"></param>
		public override void RenderStaticWithinLayout(String contents, IEngineContext context, IController controller,
													  IControllerContext controllerContext)
		{
			LayoutViewOutput layoutViewOutput = GetOutput(context.Response.Output, context, controller, controllerContext);
			layoutViewOutput.Output.Write(contents);
			// here we don't need to pass parameters from the layout to the view, 
			layoutViewOutput.RenderLayouts(this, null);
		}

		private void HandleException(string templateName, BrailBase view, Exception e)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Exception on process view: ").AppendLine(templateName);
			sb.Append("Last accessed variable: ").Append(view.LastVariableAccessed);
			string msg = sb.ToString();
			sb.Append("Exception: ").AppendLine(e.ToString());
			Log(msg);
			throw new MonoRailException(msg, e);
		}

		private void OnViewChanged(object sender, FileSystemEventArgs e)
		{
			if (Path.GetExtension(e.FullPath).IndexOf(this.ViewFileExtension) == -1 &&
				Path.GetExtension(e.FullPath).IndexOf(this.JSGeneratorFileExtension) == -1 &&
				Path.GetExtension(e.FullPath).IndexOf(".boo") == -1)
			{
				return; //early return since only watching view extensions and jsgenerator extensions
			}

			string viewRoot = String.Empty;
			IViewSourceLoader loaderThatDetectedTheChange = sender as IViewSourceLoader;
			if (loaderThatDetectedTheChange == null)
			{
				viewRoot = ViewRootDir;
			}
			else
			{
				viewRoot = loaderThatDetectedTheChange.ViewRootDir;
			}

			string path = e.FullPath.Substring(viewRoot.Length);
			path = EnsurePathDoesNotStartWithDirectorySeparator(path);
			if (path.IndexOf(options.CommonScriptsDirectory) != -1)
			{
				Log("Detected a change in commons scripts directory " + options.CommonScriptsDirectory + ", recompiling site");
				// need to invalidate the entire CommonScripts assembly
				// not worrying about concurrency here, since it is assumed
				// that changes here are rare. Note, this force a recompile of the 
				// whole site!
				try
				{
					WaitForFileToBecomeAvailableForReading(e);
					CompileCommonScripts();
				}
				catch (Exception ex)
				{
					// we failed to recompile the commons scripts directory, but because we are running
					// on another thread here, and exception would kill the application, so we log it 
					// and continue on. CompileCommonScripts() will only change the global state if it has
					// successfully compiled the commons scripts directory.
					Log("Failed to recompile the commons scripts directory! {0}", ex);
				}
			}
			else
			{
				Log("Detected a change in {0}, removing from complied cache", e.Name);
				// Will cause a recompilation
				compilations[path] = null;
			}
			ViewRecompiled(path);
		}

		private string EnsurePathDoesNotStartWithDirectorySeparator(string path)
		{
			if (path.Length > 0 && (path[0] == Path.DirectorySeparatorChar ||
									path[0] == Path.AltDirectorySeparatorChar))
			{
				path = path.Substring(1);
			}
			return path;
		}

		private static void WaitForFileToBecomeAvailableForReading(FileSystemEventArgs e)
		{
			// We may need to wait while the file is being written and closed to disk
			int retries = 10;
			bool successfullyOpenedFile = false;
			while (retries != 0 && successfullyOpenedFile == false)
			{
				retries -= 1;
				try
				{
					using (File.OpenRead(e.FullPath))
					{
						successfullyOpenedFile = true;
					}
				}
				catch (IOException)
				{
					//The file is probably in locked because it is currently being written to,
					// will wait a while for it to be freed.
					// again, this isn't something that need to be very robust, it runs on a separate thread
					// and if it fails, it is not going to do any damage
					Thread.Sleep(250);
				}
			}
		}

		public void SetViewSourceLoader(IViewSourceLoader loader)
		{
			ViewSourceLoader = loader;
		}

		// Get configuration options if they exists, if they do not exist, load the default ones
		// Create directory to save the compiled assemblies if required.
		// pre-compile the common scripts
		public override void Service(IServiceProvider serviceProvider)
		{
			base.Service(serviceProvider);
			ILoggerFactory loggerFactory = serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(GetType());
			}
		}

		// Check if a layout has been defined. If it was, then the layout would be created
		// and will take over the output, otherwise, the context.Reposne.Output is used, 
		// and layout is null
		private LayoutViewOutput GetOutput(TextWriter output, IEngineContext context, IController controller,
										   IControllerContext controllerContext)
		{
			List<BrailBase> layouts = new List<BrailBase>();
			if (controllerContext.LayoutNames != null && controllerContext.LayoutNames.Length != 0)
			{
				foreach (string layoutName in controllerContext.LayoutNames)
				{
					if(layoutName==null)
						continue;
					string layoutTemplate = layoutName;
					if (layoutTemplate.StartsWith("/") == false)
					{
						layoutTemplate = "layouts\\" + layoutTemplate;
					}
					string layoutFilename = layoutTemplate + ViewFileExtension;
					BrailBase layout = GetCompiledScriptInstance(layoutFilename, output,
													   context, controller, controllerContext);
					output = layout.ChildOutput = new StringWriter();
					layouts.Add(layout);
				}
			}
			layouts.Reverse();
			return new LayoutViewOutput(output, layouts);
		}

		/// <summary>
		/// This takes a filename and return an instance of the view ready to be used.
		/// If the file does not exist, an exception is raised
		/// The cache is checked to see if the file has already been compiled, and it had been
		/// a check is made to see that the compiled instance is newer then the file's modification date.
		/// If the file has not been compiled, or the version on disk is newer than the one in memory, a new
		/// version is compiled.
		/// Finally, an instance is created and returned	
		/// </summary>
		public BrailBase GetCompiledScriptInstance(
			string file,
			TextWriter output,
			IEngineContext context,
			IController controller, IControllerContext controllerContext)
		{
			bool batch = options.BatchCompile;

			// normalize filename - replace / or \ to the system path seperator
			string filename = file.Replace('/', Path.DirectorySeparatorChar)
				.Replace('\\', Path.DirectorySeparatorChar);

			filename = EnsurePathDoesNotStartWithDirectorySeparator(filename);
			Log("Getting compiled instnace of {0}", filename);

			Type type;

			if (compilations.ContainsKey(filename))
			{
				type = (Type)compilations[filename];
				if (type != null)
				{
					Log("Got compiled instance of {0} from cache", filename);
					return CreateBrailBase(context, controller, controllerContext, output, type);
				}
				// if file is in compilations and the type is null,
				// this means that we need to recompile. Since this usually means that 
				// the file was changed, we'll set batch to false and procceed to compile just
				// this file.
				Log("Cache miss! Need to recompile {0}", filename);
				batch = false;
			}

			type = CompileScript(filename, batch);

			if (type == null)
			{
				throw new MonoRailException("Could not find a view with path " + filename);
			}

			return CreateBrailBase(context, controller, controllerContext, output, type);
		}

		private BrailBase CreateBrailBase(IEngineContext context, IController controller, IControllerContext controllerContext,
										  TextWriter output, Type type)
		{
			ConstructorInfo constructor = (ConstructorInfo)constructors[type];
			if (constructor == null)
			{
				string message = "Could not find a constructor for " + type + ", but it was found in the compilation cache. Clearing the compilation cache for the type, please try again";
				Log(message);

				object key = this.typeToFileName[type];
				if (key != null)
				{
					compilations.Remove(key);
				}

				throw new MonoRailException(message);
			}
			BrailBase self = (BrailBase)FormatterServices.GetUninitializedObject(type);
			constructor.Invoke(self, new object[] { this, output, context, controller, controllerContext });
			return self;
		}

		// Compile a script (or all scripts in a directory), save the compiled result
		// to the cache and return the compiled type.
		// If an error occurs in batch compilation, then an attempt is made to compile just the single
		// request file.
		[MethodImpl(MethodImplOptions.Synchronized)]
		public Type CompileScript(string filename, bool batch)
		{
			IDictionary<ICompilerInput, string> inputs2FileName = GetInput(filename, batch);
			string name = NormalizeName(filename);
			Log("Compiling {0} to {1} with batch: {2}", filename, name, batch);
			CompilationResult result = DoCompile(inputs2FileName.Keys, name);

			if (result.Context.Errors.Count > 0)
			{
				if (batch == false)
				{
					RaiseCompilationException(filename, inputs2FileName, result);
				}
				//error compiling a batch, let's try a single file
				return CompileScript(filename, false);
			}
			Type type;
			foreach (ICompilerInput input in inputs2FileName.Keys)
			{
				string viewName = Path.GetFileNameWithoutExtension(input.Name);
				string typeName = TransformToBrailStep.GetViewTypeName(viewName);
				type = result.Context.GeneratedAssembly.GetType(typeName);
				Log("Adding {0} to the cache", type.FullName);
				constructors[type] = type.GetConstructor(new Type[]
				                                         	{
				                                         		typeof(BooViewEngine),
				                                         		typeof(TextWriter),
				                                         		typeof(IEngineContext),
				                                         		typeof(IController),
				                                         		typeof(IControllerContext)
				                                         	});
				string compilationName = inputs2FileName[input];
				typeToFileName[type] = compilationName;
				compilations[compilationName] = type;
			}
			type = (Type)compilations[filename];
			return type;
		}

		private void RaiseCompilationException(string filename, IDictionary<ICompilerInput, string> inputs2FileName,
											   CompilationResult result)
		{
			string errors = result.Context.Errors.ToString(true);
			Log("Failed to compile {0} because {1}", filename, errors);
			StringBuilder code = new StringBuilder();
			foreach (ICompilerInput input in inputs2FileName.Keys)
			{
				code.AppendLine()
					.Append(result.Processor.GetInputCode(input))
					.AppendLine();
			}
			throw new HttpParseException("Error compiling Brail code",
										 result.Context.Errors[0],
										 filename,
										 code.ToString(), result.Context.Errors[0].LexicalInfo.Line);
		}

		// If batch compilation is set to true, this would return all the view scripts
		// in the director (not recursive!)
		// Otherwise, it would return just the single file
		private IDictionary<ICompilerInput, string> GetInput(string filename, bool batch)
		{
			Dictionary<ICompilerInput, string> input2FileName = new Dictionary<ICompilerInput, string>();
			if (batch == false)
			{
				input2FileName.Add(CreateInput(filename), filename);
				return input2FileName;
			}
			// use the System.IO.Path to get the folder name even though
			// we are using the ViewSourceLoader to load the actual file
			string directory = Path.GetDirectoryName(filename);
			foreach (string file in ViewSourceLoader.ListViews(directory, this.ViewFileExtension, this.JSGeneratorFileExtension))
			{
				ICompilerInput input = CreateInput(file);
				input2FileName.Add(input, file);
			}
			return input2FileName;
		}

		// create an input from a resource name
		public ICompilerInput CreateInput(string name)
		{
			IViewSource viewSrc = ViewSourceLoader.GetViewSource(name);
			if (viewSrc == null)
			{
				throw new MonoRailException("{0} is not a valid view", name);
			}
			// I need to do it this way because I can't tell 
			// when to dispose of the stream. 
			// It is not expected that this will be a big problem, the string
			// will go away after the compile is done with them.
			using (StreamReader stream = new StreamReader(viewSrc.OpenViewStream()))
			{
				return new StringInput(name, stream.ReadToEnd());
			}
		}

		/// <summary>
		/// Perform the actual compilation of the scripts
		/// Things to note here:
		/// * The generated assembly reference the Castle.MonoRail.MonoRailBrail and Castle.MonoRail.Framework assemblies
		/// * If a common scripts assembly exist, it is also referenced
		/// * The AddBrailBaseClassStep compiler step is added - to create a class from the view's code
		/// * The ProcessMethodBodiesWithDuckTyping is replaced with ReplaceUknownWithParameters
		///   this allows to use naked parameters such as (output context.IsLocal) without using 
		///   any special syntax
		/// * The FixTryGetParameterConditionalChecks is run afterward, to transform "if ?Error" to "if not ?Error isa IgnoreNull"
		/// * The ExpandDuckTypedExpressions is replace with a derived step that allows the use of Dynamic Proxy assemblies
		/// * The IntroduceGlobalNamespaces step is removed, to allow to use common variables such as 
		///   date and list without accidently using the Boo.Lang.BuiltIn versions
		/// </summary>
		/// <param name="files"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private CompilationResult DoCompile(IEnumerable<ICompilerInput> files, string name)
		{
			ICompilerInput[] filesAsArray = new List<ICompilerInput>(files).ToArray();
			BooCompiler compiler = SetupCompiler(filesAsArray);
			string filename = Path.Combine(baseSavePath, name);
			compiler.Parameters.OutputAssembly = filename;
			// this is here and not in SetupCompiler since CompileCommon is also
			// using SetupCompiler, and we don't want reference to the old common from the new one
			if (common != null)
			{
				compiler.Parameters.References.Add(common);
			}
			// pre procsssor needs to run before the parser
			BrailPreProcessor processor = new BrailPreProcessor(this);
			compiler.Parameters.Pipeline.Insert(0, processor);
			// inserting the add class step after the parser
			compiler.Parameters.Pipeline.Insert(2, new TransformToBrailStep(options));
			compiler.Parameters.Pipeline.Replace(typeof(ProcessMethodBodiesWithDuckTyping),
												 new ReplaceUknownWithParameters());
			compiler.Parameters.Pipeline.Replace(typeof(ExpandDuckTypedExpressions),
												 new ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods());
			compiler.Parameters.Pipeline.Replace(typeof(InitializeTypeSystemServices),
												 new InitializeCustomTypeSystem());
			compiler.Parameters.Pipeline.InsertBefore(typeof(ReplaceUknownWithParameters),
													  new FixTryGetParameterConditionalChecks());
			compiler.Parameters.Pipeline.RemoveAt(compiler.Parameters.Pipeline.Find(typeof(IntroduceGlobalNamespaces)));

			return new CompilationResult(compiler.Run(), processor);
		}

		// Return the output filename for the generated assembly
		// The filename is dependant on whatever we are doing a batch
		// compile or not, if it's a batch compile, then the directory name
		// is used, if it's just a single file, we're using the file's name.
		// '/' and '\' are replaced with '_', I'm not handling ':' since the path
		// should never include it since I'm converting this to a relative path
		public string NormalizeName(string filename)
		{
			string name = filename;
			name = name.Replace(Path.AltDirectorySeparatorChar, '_');
			name = name.Replace(Path.DirectorySeparatorChar, '_');

			return name + "_BrailView.dll";
		}

		// Compile all the common scripts to a common assemblies
		// an error in the common scripts would raise an exception.
		public bool CompileCommonScripts()
		{
			if (options.CommonScriptsDirectory == null)
			{
				return false;
			}

			// the demi.boo is stripped, but GetInput require it.
			string demiFile = Path.Combine(options.CommonScriptsDirectory, "demi.brail");
			IDictionary<ICompilerInput, string> inputs = GetInput(demiFile, true);
			ICompilerInput[] inputsAsArray = new List<ICompilerInput>(inputs.Keys).ToArray();
			BooCompiler compiler = SetupCompiler(inputsAsArray);
			string outputFile = Path.Combine(baseSavePath, "CommonScripts.dll");
			compiler.Parameters.OutputAssembly = outputFile;
			CompilerContext result = compiler.Run();
			if (result.Errors.Count > 0)
			{
				throw new MonoRailException(result.Errors.ToString(true));
			}
			common = result.GeneratedAssembly;
			compilations.Clear();
			return true;
		}

		// common setup for the compiler
		private static BooCompiler SetupCompiler(IEnumerable<ICompilerInput> files)
		{
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Ducky = true;
			compiler.Parameters.Debug = options.Debug;
			if (options.SaveToDisk)
			{
				compiler.Parameters.Pipeline = new CompileToFile();
			}
			else
			{
				compiler.Parameters.Pipeline = new CompileToMemory();
			}
			// replace the normal parser with white space agnostic one.
			compiler.Parameters.Pipeline.RemoveAt(0);
			compiler.Parameters.Pipeline.Insert(0, new WSABooParsingStep());
			foreach (ICompilerInput file in files)
			{
				compiler.Parameters.Input.Add(file);
			}
			foreach (Assembly assembly in options.AssembliesToReference)
			{
				compiler.Parameters.References.Add(assembly);
			}
			compiler.Parameters.OutputType = CompilerOutputType.Library;
			return compiler;
		}

		private static void InitializeConfig()
		{
			InitializeConfig("brail");

			if (options == null)
			{
				InitializeConfig("Brail");
			}

			if (options == null)
			{
				options = new BooViewEngineOptions();
			}
		}

		private static void InitializeConfig(string sectionName)
		{
			options = ConfigurationManager.GetSection(sectionName) as BooViewEngineOptions;
		}

		private void Log(string msg, params object[] items)
		{
			if (logger == null || logger.IsDebugEnabled == false)
			{
				return;
			}
			logger.DebugFormat(msg, items);
		}

		public bool ConditionalPreProcessingOnly(string name)
		{
			return String.Equals(
				Path.GetExtension(name),
				JSGeneratorFileExtension,
				StringComparison.InvariantCultureIgnoreCase);
		}

		#region Nested type: CompilationResult

		private class CompilationResult
		{
			private readonly CompilerContext context;
			private readonly BrailPreProcessor processor;

			public CompilationResult(CompilerContext context, BrailPreProcessor processor)
			{
				this.context = context;
				this.processor = processor;
			}

			public CompilerContext Context
			{
				get { return context; }
			}

			public BrailPreProcessor Processor
			{
				get { return processor; }
			}
		}

		#endregion

		#region Nested type: LayoutViewOutput

		private class LayoutViewOutput
		{
			private readonly List<BrailBase> layouts;
			private readonly TextWriter output;

			public LayoutViewOutput(TextWriter output, List<BrailBase> layouts)
			{
				this.layouts = layouts;
				this.output = output;
			}

			public TextWriter Output
			{
				get { return output; }
			}

			public void RenderLayouts(BooViewEngine booViewEngine, BrailBase view)
			{
				foreach(BrailBase layout in layouts)
				{
					try
					{
						layout.SetParent(view);
						layout.Run();
						view = layout;
					}
					catch(Exception e)
					{
						booViewEngine.HandleException(layout.ViewFileName, layout, e);
					}
				}
			}
		}

		#endregion
	}
}