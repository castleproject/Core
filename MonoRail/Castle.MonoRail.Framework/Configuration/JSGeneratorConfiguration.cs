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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Pendent
	/// </summary>
	public class JSGeneratorConfiguration
	{
		private readonly List<LibraryConfiguration> libraries = new List<LibraryConfiguration>();
		private LibraryConfiguration defaultLibrary;

		/// <summary>
		/// Adds the library.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="mainGenerator">The main generator.</param>
		/// <returns></returns>
		public LibraryConfigurationBuilder AddLibrary(string name, Type mainGenerator)
		{
			LibraryConfiguration config = new LibraryConfiguration(name, mainGenerator);
			libraries.Add(config);

			return new LibraryConfigurationBuilder(this, config);
		}

		/// <summary>
		/// Gets the libraries configuration.
		/// </summary>
		/// <value>The libraries.</value>
		public List<LibraryConfiguration> Libraries
		{
			get { return libraries; }
		}

		/// <summary>
		/// Gets the default library configuration.
		/// </summary>
		/// <value>The default library.</value>
		public LibraryConfiguration DefaultLibrary
		{
			get { return defaultLibrary; }
		}

		/// <summary>
		/// Configures a JS generator for specific JS library
		/// </summary>
		public class LibraryConfigurationBuilder
		{
			private readonly JSGeneratorConfiguration configuration;
			private readonly LibraryConfiguration config;
			private readonly ElementGeneratorConfigBuilder elementConfig;

			/// <summary>
			/// Initializes a new instance of the <see cref="LibraryConfigurationBuilder"/> class.
			/// </summary>
			/// <param name="configuration">The configuration.</param>
			/// <param name="config">The config.</param>
			public LibraryConfigurationBuilder(JSGeneratorConfiguration configuration, LibraryConfiguration config)
			{
				this.configuration = configuration;
				this.config = config;
				elementConfig = new ElementGeneratorConfigBuilder(this);
			}

			/// <summary>
			/// Adds an extension.
			/// </summary>
			/// <param name="extensionType">Type of the extension.</param>
			/// <returns></returns>
			public LibraryConfigurationBuilder AddExtension(Type extensionType)
			{
				config.MainExtensions.Add(extensionType);
				return this;
			}

			/// <summary>
			/// Sets the browser validator provider for the library
			/// </summary>
			/// <param name="browserValidatorProvider">The browser validator provider.</param>
			public LibraryConfigurationBuilder BrowserValidatorIs(Type browserValidatorProvider)
			{
				config.BrowserValidatorProvider = browserValidatorProvider;
				return this;
			}

			/// <summary>
			/// Gets the element generator.
			/// </summary>
			/// <value>The element generator.</value>
			public ElementGeneratorConfigBuilder ElementGenerator
			{
				get { return elementConfig; }
			}

			/// <summary>
			/// Sets this JS library as the default one.
			/// </summary>
			public void SetAsDefault()
			{
				configuration.defaultLibrary = config;
			}

			/// <summary>
			/// Configures the element section of the library config
			/// </summary>
			public class ElementGeneratorConfigBuilder
			{
				private readonly LibraryConfigurationBuilder builder;

				/// <summary>
				/// Initializes a new instance of the <see cref="ElementGeneratorConfigBuilder"/> class.
				/// </summary>
				/// <param name="builder">The builder.</param>
				public ElementGeneratorConfigBuilder(LibraryConfigurationBuilder builder)
				{
					this.builder = builder;
				}

				/// <summary>
				/// Adds an element only extension.
				/// </summary>
				/// <param name="extensionType">Type of the extension.</param>
				/// <returns></returns>
				public ElementGeneratorConfigBuilder AddExtension(Type extensionType)
				{
					builder.config.ElementExtension.Add(extensionType);
					return this;
				}

				/// <summary>
				/// Go back to the main library configuration
				/// </summary>
				public LibraryConfigurationBuilder Done
				{
					get { return builder; }
				}
			}
		}
	}

	/// <summary>
	/// Configuration for JS generation for an specific JS library
	/// </summary>
	public class LibraryConfiguration
	{
		private readonly string name;
		private readonly Type mainGenerator;
		private readonly List<Type> mainExtensions = new List<Type>();
		private readonly List<Type> elementExtension = new List<Type>();
		private Type browserValidatorProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="LibraryConfiguration"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="mainGenerator">The main generator.</param>
		public LibraryConfiguration(string name, Type mainGenerator)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (mainGenerator == null)
			{
				throw new ArgumentNullException("mainGenerator");
			}

			this.name = name;
			this.mainGenerator = mainGenerator;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the main generator.
		/// </summary>
		/// <value>The main generator.</value>
		public Type MainGenerator
		{
			get { return mainGenerator; }
		}

		/// <summary>
		/// Gets the main extensions.
		/// </summary>
		/// <value>The main extensions.</value>
		public List<Type> MainExtensions
		{
			get { return mainExtensions; }
		}

		/// <summary>
		/// Gets the element extension.
		/// </summary>
		/// <value>The element extension.</value>
		public List<Type> ElementExtension
		{
			get { return elementExtension; }
		}

		/// <summary>
		/// Gets or sets the browser validator provider.
		/// </summary>
		/// <value>The browser validator provider.</value>
		public Type BrowserValidatorProvider
		{
			get { return browserValidatorProvider; }
			set { browserValidatorProvider = value; }
		}
	}
}
