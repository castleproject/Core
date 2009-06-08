// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Compiler;
	using Compiler.MarkupTransformers;
	using Compiler.PreCompilationSteps;

	public class CompilerOptionsBuilder
	{
		private bool allowPartiallyTrustedCallers;
		private bool autoRecompilation;
		private bool keepTemporarySourceFiles;
		private bool debug;
		private string temporarySourceFilesDirectory;
		readonly List<ReferencedAssembly> assembliesToReference = new List<ReferencedAssembly>();
		private Type preCompilationStepsProviderType;
		private Type markupTransformersProviderType;

		public AspViewCompilerOptions BuildOptions()
		{
			var providers = new Dictionary<Type, Type>();
			if (preCompilationStepsProviderType != null)
				providers.Add(typeof(IPreCompilationStepsProvider), preCompilationStepsProviderType);
			if (markupTransformersProviderType != null)
				providers.Add(typeof(IMarkupTransformersProvider), markupTransformersProviderType);
			return new AspViewCompilerOptions(debug, autoRecompilation, allowPartiallyTrustedCallers, temporarySourceFilesDirectory, keepTemporarySourceFiles, assembliesToReference, providers);
		}

		/// <summary>
		/// Add references used by the views
		/// </summary>
		/// <param name="referencesToAdd">One (or more) assemblies to reference</param>
		/// <returns>Options Builder</returns>
		public CompilerOptionsBuilder AddReferences(params ReferencedAssembly[] referencesToAdd)
		{
			assembliesToReference.AddRange(referencesToAdd);
			return this;
		}

		/// <summary>
		/// Set IMarkupTransformersProvider
		/// </summary>
		/// <typeparam name="T">IMarkupTransformersProvider implementation</typeparam>
		/// <returns>OptionsBuilder</returns>
		public CompilerOptionsBuilder MarkupTransformersProviderIs<T>()
			where T : IMarkupTransformersProvider
		{
			markupTransformersProviderType = typeof(T);
			return this;
		}

		/// <summary>
		/// Set IMarkupTransformersProvider
		/// </summary>
		/// <param name="type">IMarkupTransformersProvider implementation</param>
		/// <returns>OptionsBuilder</returns>
		public CompilerOptionsBuilder MarkupTransformersProviderIs(Type type)
		{
			markupTransformersProviderType = type;
			return this;
		}

		/// <summary>
		/// Set IPreCompilationStepsProvider
		/// </summary>
		/// <typeparam name="T">IPreCompilationStepsProvider implementation</typeparam>
		/// <returns>OptionsBuilder</returns>
		public CompilerOptionsBuilder PreCompilationStepsProviderIs<T>()
			where T : IPreCompilationStepsProvider
		{
			preCompilationStepsProviderType = typeof(T);
			return this;
		}

		/// <summary>
		/// Set IPreCompilationStepsProvider
		/// </summary>
		/// <param name="type">IPreCompilationStepsProvider implementation</param>
		/// <returns>OptionsBuilder</returns>
		public CompilerOptionsBuilder PreCompilationStepsProviderIs(Type type)
		{
			preCompilationStepsProviderType = type;
			return this;
		}

		/// <summary>
		/// Set the CompiledViews assembly with APTCA
		/// </summary>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder AllowPartiallyTrustedCallers()
		{
			return AllowPartiallyTrustedCallers(true);
		}

		/// <summary>
		/// Set the CompiledViews assembly with APTCA
		/// </summary>
		/// <param name="value">Defaults to false</param>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder AllowPartiallyTrustedCallers(bool value)
		{
			allowPartiallyTrustedCallers = value;
			return this;
		}

		/// <summary>
		/// Autorecompilation mode (detect view template changes)
		/// </summary>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder AutoRecompilation()
		{
			return AutoRecompilation(true);
		}

		/// <summary>
		/// Autorecompilation mode (detect view template changes)
		/// </summary>
		/// <param name="value">Defaults to false</param>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder AutoRecompilation(bool value)
		{
			autoRecompilation = value;
			return this;
		}

		/// <summary>
		/// Keep the intermediate c# version of the views
		/// </summary>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder KeepTemporarySourceFiles()
		{
			return KeepTemporarySourceFiles(true);
		}

		/// <summary>
		/// Keep the intermediate c# version of the views
		/// </summary>
		/// <param name="value">Defaults to false</param>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder KeepTemporarySourceFiles(bool value)
		{
			keepTemporarySourceFiles = value;
			return this;
		}

		/// <summary>
		/// Set the compiler to DEBUG
		/// </summary>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder CompileForDebugging()
		{
			return CompileForDebugging(true);
		}

		/// <summary>
		/// Set the compiler to DEBUG
		/// </summary>
		/// <param name="value">Defaults to false</param>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder CompileForDebugging(bool value)
		{
			debug = value;
			return this;
		}

		/// <summary>
		/// A directory on the computer for storing the views c# sources
		/// </summary>
		/// <param name="directory">Where to store the files</param>
		/// <returns>Options builder</returns>
		public CompilerOptionsBuilder UsingTemporarySourceFilesDirectory(string directory)
		{
			temporarySourceFilesDirectory = directory;
			return this;
		}

		/// <summary>
		/// Apply configuration overrides from web.config (or vcompile.exe.config)
		/// </summary>
		/// <param name="config">The settings from config file</param>
		/// <returns>The builder</returns>
		public CompilerOptionsBuilder ApplyConfigurableOverrides(AspViewConfigurationSection.Model config)
		{
			var overrides = config.CompilerOptions;
			if (overrides.AllowPartiallyTrustedCallers.HasValue)
				AllowPartiallyTrustedCallers(overrides.AllowPartiallyTrustedCallers.Value);

			if (overrides.AutoRecompilation.HasValue)
				AutoRecompilation(overrides.AutoRecompilation.Value);

			if (overrides.Debug.HasValue)
				CompileForDebugging(overrides.Debug.Value);

			if (overrides.SaveFiles.HasValue)
				KeepTemporarySourceFiles(overrides.SaveFiles.Value);

			if (string.IsNullOrEmpty(overrides.TemporarySourceFilesDirectory) == false)
				UsingTemporarySourceFilesDirectory(overrides.TemporarySourceFilesDirectory);

			if (config.Providers != null)
				foreach (var type in config.Providers.Keys)
				{
					if (type == typeof(IPreCompilationStepsProvider))
						PreCompilationStepsProviderIs(config.Providers[type]);
					if (type == typeof(IMarkupTransformersProvider))
						MarkupTransformersProviderIs(config.Providers[type]);
				}

			AddReferences(config.References.ToArray());
			return this;
		}
	}
}