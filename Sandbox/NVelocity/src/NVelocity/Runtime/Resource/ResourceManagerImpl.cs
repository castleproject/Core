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

namespace NVelocity.Runtime.Resource
{
	using System;
	using System.Collections;
	using System.IO;
	using Commons.Collections;
	using NVelocity.Exception;
	using NVelocity.Runtime.Resource.Loader;

	/// <summary> 
	/// Class to manage the text resource for the Velocity Runtime.
	/// </summary>
	public class ResourceManagerImpl : ResourceManager
	{
		public ResourceManagerImpl()
		{
			InitBlock();
		}

		private void InitBlock()
		{
			resourceLoaders = new ArrayList();
			sourceInitializerList = new ArrayList();
		}

		/// <summary> A template resources.
		/// </summary>
		public const int RESOURCE_TEMPLATE = 1;

		/// <summary> A static content resource.
		/// </summary>
		public const int RESOURCE_CONTENT = 2;

		/// <summary> token used to identify the loader internally
		/// </summary>
		private const String RESOURCE_LOADER_IDENTIFIER = "_RESOURCE_LOADER_IDENTIFIER_";

		/// <summary>  Object implementing ResourceCache to
		/// be our resource manager's Resource cache.
		/// </summary>
		protected internal ResourceCache globalCache = null;

		/// <summary> 
		/// The List of templateLoaders that the Runtime will
		/// use to locate the InputStream source of a template.
		/// </summary>
		protected internal ArrayList resourceLoaders;

		/// <summary> This is a list of the template input stream source
		/// initializers, basically properties for a particular
		/// template stream source. The order in this list
		/// reflects numbering of the properties i.e.
		/// *
		/// <loader-id>.resource.loader.<property> = <value>
		/// </summary>
		private ArrayList sourceInitializerList;

		/// <summary> Each loader needs a configuration object for
		/// its initialization, this flags keeps track of whether
		/// or not the configuration objects have been created
		/// for the resource loaders.
		/// </summary>
		private bool resourceLoaderInitializersActive = false;

		/// <summary>
		/// switch to turn off log notice when a resource is found for
		/// the first time.
		/// </summary>
		private bool logWhenFound = true;

		protected internal RuntimeServices rsvc = null;

		/// <summary>
		/// Initialize the ResourceManager.
		/// </summary>
		public void initialize(RuntimeServices rs)
		{
			rsvc = rs;

			rsvc.info("Default ResourceManager initializing. (" + this.GetType() + ")");

			ResourceLoader resourceLoader;

			assembleResourceLoaderInitializers();

			for (int i = 0; i < sourceInitializerList.Count; i++)
			{
				ExtendedProperties configuration = (ExtendedProperties) sourceInitializerList[i];
				String loaderClass = configuration.GetString("class");

				if (loaderClass == null)
				{
					rsvc.error("Unable to find '" + configuration.GetString(RESOURCE_LOADER_IDENTIFIER) + ".resource.loader.class' specification in configuation." + " This is a critical value.  Please adjust configuration.");
					continue;
				}

				resourceLoader = ResourceLoaderFactory.getLoader(rsvc, loaderClass);
				resourceLoader.commonInit(rsvc, configuration);
				resourceLoader.init(configuration);
				resourceLoaders.Add(resourceLoader);

			}

			/*
			* now see if this is overridden by configuration
			*/

			logWhenFound = rsvc.getBoolean(RuntimeConstants_Fields.RESOURCE_MANAGER_LOGWHENFOUND, true);

			/*
			*  now, is a global cache specified?
			*/
			String claz = rsvc.getString(RuntimeConstants_Fields.RESOURCE_MANAGER_CACHE_CLASS);
			Object o = null;

			if (claz != null && claz.Length > 0)
			{
				try
				{
					Type type = Type.GetType(claz);
					o = Activator.CreateInstance(type);
				}
				catch (Exception cnfe)
				{
					String err = "The specified class for ResourceCache (" + claz + ") does not exist (or is not accessible to the current classlaoder).";
					rsvc.error(err);
					o = null;
				}

				if (!(o is ResourceCache))
				{
					String err = "The specified class for ResourceCache (" + claz + ") does not implement NVelocity.Runtime.Resource.ResourceCache." + " Using default ResourceCache implementation.";
					rsvc.error(err);
					o = null;
				}
			}

			/*
			*  if we didn't get through that, just use the default.
			*/
			if (o == null)
			{
				o = new ResourceCacheImpl();
			}

			globalCache = (ResourceCache) o;
			globalCache.initialize(rsvc);
			rsvc.info("Default ResourceManager initialization complete.");
		}

		/// <summary> This will produce a List of Hashtables, each
		/// hashtable contains the intialization info for
		/// a particular resource loader. This Hastable
		/// will be passed in when initializing the
		/// the template loader.
		/// </summary>
		private void assembleResourceLoaderInitializers()
		{
			if (resourceLoaderInitializersActive)
			{
				return;
			}

			ArrayList resourceLoaderNames = rsvc.Configuration.GetVector(RuntimeConstants_Fields.RESOURCE_LOADER);

			for (int i = 0; i < resourceLoaderNames.Count; i++)
			{
				/*
				 * The loader id might look something like the following:
				 *
				 * file.resource.loader
				 *
				 * The loader id is the prefix used for all properties
				 * pertaining to a particular loader.
				 */
				String loaderID = resourceLoaderNames[i] + "." + RuntimeConstants_Fields.RESOURCE_LOADER;

				ExtendedProperties loaderConfiguration = rsvc.Configuration.Subset(loaderID);

				/*
				 *  we can't really count on ExtendedProperties to give us an empty set
				 */
				if (loaderConfiguration == null)
				{
					rsvc.warn("ResourceManager : No configuration information for resource loader named '" + resourceLoaderNames[i] + "'. Skipping.");
					continue;
				}

				/*
				 *  add the loader name token to the initializer if we need it
				 *  for reference later. We can't count on the user to fill
				 *  in the 'name' field
				 */
				loaderConfiguration.SetProperty(RESOURCE_LOADER_IDENTIFIER, resourceLoaderNames[i]);

				/*
				 * Add resources to the list of resource loader
				 * initializers.
				 */
				sourceInitializerList.Add(loaderConfiguration);
			}

			resourceLoaderInitializersActive = true;
		}

		/// <summary> Gets the named resource.  Returned class type corresponds to specified type
		/// (i.e. <code>Template</code> to <code>RESOURCE_TEMPLATE</code>).
		/// *
		/// </summary>
		/// <param name="resourceName">The name of the resource to retrieve.
		/// </param>
		/// <param name="resourceType">The type of resource (<code>RESOURCE_TEMPLATE</code>,
		/// <code>RESOURCE_CONTENT</code>, etc.).
		/// </param>
		/// <param name="encoding"> The character encoding to use.
		/// </param>
		/// <returns>Resource with the template parsed and ready.
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception if a problem in parse
		///
		/// </returns>
		public Resource getResource(String resourceName, int resourceType, String encoding)
		{
			/*
	    * Check to see if the resource was placed in the cache.
	    * If it was placed in the cache then we will use
	    * the cached version of the resource. If not we
	    * will load it.
	    */

			Resource resource = globalCache.get(resourceName);

			if (resource != null)
			{
				/*
				*  refresh the resource
				*/

				try
				{
					refreshResource(resource, encoding);
				}
				catch (ResourceNotFoundException rnfe)
				{
					/*
		    *  something exceptional happened to that resource
		    *  this could be on purpose, 
		    *  so clear the cache and try again
		    */

					globalCache.remove(resourceName);

					return getResource(resourceName, resourceType, encoding);
				}
				catch (ParseErrorException pee)
				{
					rsvc.error("ResourceManager.getResource() exception: " + pee);

					throw pee;
				}
				catch (Exception eee)
				{
					rsvc.error("ResourceManager.getResource() exception: " + eee);

					throw eee;
				}
			}
			else
			{
				try
				{
					/*
		    *  it's not in the cache, so load it.
		    */

					resource = loadResource(resourceName, resourceType, encoding);

					if (resource.ResourceLoader.isCachingOn())
					{
						globalCache.put(resourceName, resource);
					}
				}
				catch (ResourceNotFoundException rnfe2)
				{
					rsvc.error("ResourceManager : unable to find resource '" + resourceName + "' in any resource loader.");

					throw rnfe2;
				}
				catch (ParseErrorException pee)
				{
					rsvc.error("ResourceManager.getResource() parse exception: " + pee);

					throw pee;
				}
				catch (Exception ee)
				{
					rsvc.error("ResourceManager.getResource() exception new: " + ee);

					throw ee;
				}
			}

			return resource;
		}

		/// <summary>
		/// Loads a resource from the current set of resource loaders
		/// </summary>
		/// <param name="resourceName">The name of the resource to retrieve.</param>
		/// <param name="resourceType">The type of resource (<code>RESOURCE_TEMPLATE</code>,
		/// <code>RESOURCE_CONTENT</code>, etc.).
		/// </param>
		/// <param name="encoding"> The character encoding to use.</param>
		/// <returns>Resource with the template parsed and ready.
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception if a problem in parse
		/// </returns>
		protected internal Resource loadResource(String resourceName, int resourceType, String encoding)
		{
			Resource resource = ResourceFactory.getResource(resourceName, resourceType);

			resource.RuntimeServices = rsvc;

			resource.Name = resourceName;
			resource.Encoding = encoding;

			/*
	    * Now we have to try to find the appropriate
	    * loader for this resource. We have to cycle through
	    * the list of available resource loaders and see
	    * which one gives us a stream that we can use to
	    * make a resource with.
	    */

			long howOldItWas = 0; // Initialize to avoid warnings

			ResourceLoader resourceLoader = null;

			for (int i = 0; i < resourceLoaders.Count; i++)
			{
				resourceLoader = (ResourceLoader) resourceLoaders[i];
				resource.ResourceLoader = resourceLoader;

				/*
		*  catch the ResourceNotFound exception
		*  as that is ok in our new multi-loader environment
		*/

				try
				{
					if (resource.Process())
					{
						/*
			*  FIXME  (gmj)
			*  moved in here - technically still 
			*  a problem - but the resource needs to be 
			*  processed before the loader can figure 
			*  it out due to to the new 
			*  multi-path support - will revisit and fix
			*/

						if (logWhenFound)
						{
							rsvc.info("ResourceManager : found " + resourceName + " with loader " + resourceLoader.ClassName);
						}

						howOldItWas = resourceLoader.getLastModified(resource);
						break;
					}
				}
				catch (ResourceNotFoundException)
				{
					/*
		    *  that's ok - it's possible to fail in
		    *  multi-loader environment
		    */
				}
			}

			/*
	    * Return null if we can't find a resource.
	    */
			if (resource.Data == null)
			{
				throw new ResourceNotFoundException("Unable to find resource '" + resourceName + "'");
			}

			/*
	    *  some final cleanup
	    */

			resource.LastModified = howOldItWas;

			resource.ModificationCheckInterval = resourceLoader.ModificationCheckInterval;

			resource.Touch();

			return resource;
		}

		/// <summary>  Takes an existing resource, and 'refreshes' it.  This
		/// generally means that the source of the resource is checked
		/// for changes according to some cache/check algorithm
		/// and if the resource changed, then the resource data is
		/// reloaded and re-parsed.
		/// *
		/// </summary>
		/// <param name="resource">resource to refresh
		/// *
		/// @throws ResourceNotFoundException if template not found
		/// from current source for this Resource
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception if a problem in parse
		///
		/// </param>
		protected internal void refreshResource(Resource resource, String encoding)
		{
			/*
	    * The resource knows whether it needs to be checked
	    * or not, and the resource's loader can check to
	    * see if the source has been modified. If both
	    * these conditions are true then we must reload
	    * the input stream and parse it to make a new
	    * AST for the resource.
	    */
			if (resource.RequiresChecking())
			{
				/*
		*  touch() the resource to reset the counters
		*/

				resource.Touch();

				if (resource.IsSourceModified())
				{
					/*
		    *  now check encoding info.  It's possible that the newly declared
		    *  encoding is different than the encoding already in the resource
		    *  this strikes me as bad...
		    */

					if (!resource.Encoding.Equals(encoding))
					{
						rsvc.error("Declared encoding for template '" + resource.Name + "' is different on reload.  Old = '" + resource.Encoding + "'  New = '" + encoding);

						resource.Encoding = encoding;
					}

					/*
		    *  read how old the resource is _before_
		    *  processing (=>reading) it
		    */
					long howOldItWas = resource.ResourceLoader.getLastModified(resource);

					/*
		    *  read in the fresh stream and parse
		    */

					resource.Process();

					/*
		    *  now set the modification info and reset
		    *  the modification check counters
		    */

					resource.LastModified = howOldItWas;
				}
			}
		}

		/// <summary> Gets the named resource.  Returned class type corresponds to specified type
		/// (i.e. <code>Template</code> to <code>RESOURCE_TEMPLATE</code>).
		/// *
		/// </summary>
		/// <param name="resourceName">The name of the resource to retrieve.
		/// </param>
		/// <param name="resourceType">The type of resource (<code>RESOURCE_TEMPLATE</code>,
		/// <code>RESOURCE_CONTENT</code>, etc.).
		/// </param>
		/// <returns>Resource with the template parsed and ready.
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception if a problem in parse
		/// *
		/// </returns>
		/// <deprecated>Use
		/// {@link #getResource(String resourceName, int resourceType,
		/// String encoding )}
		///
		/// </deprecated>
		public Resource getResource(String resourceName, int resourceType)
		{
			return getResource(resourceName, resourceType, RuntimeConstants_Fields.ENCODING_DEFAULT);
		}

		/// <summary>  Determines is a template exists, and returns name of the loader that
		/// provides it.  This is a slightly less hokey way to support
		/// the Velocity.templateExists() utility method, which was broken
		/// when per-template encoding was introduced.  We can revisit this.
		/// *
		/// </summary>
		/// <param name="resourceName">Name of template or content resource
		/// </param>
		/// <returns>class name of loader than can provide it
		///
		/// </returns>
		public String getLoaderNameForResource(String resourceName)
		{
			ResourceLoader resourceLoader = null;

			/*
	    *  loop through our loaders...
	    */
			for (int i = 0; i < resourceLoaders.Count; i++)
			{
				resourceLoader = (ResourceLoader) resourceLoaders[i];

				Stream is_Renamed = null;

				/*
		*  if we find one that can provide the resource,
		*  return the name of the loaders's Class
		*/
				try
				{
					is_Renamed = resourceLoader.getResourceStream(resourceName);

					if (is_Renamed != null)
					{
						return resourceLoader.GetType().ToString();
					}
				}
				catch (ResourceNotFoundException e)
				{
					/*
		    * this isn't a problem.  keep going
		    */
				}
				finally
				{
					/*
		    *  if we did find one, clean up because we were 
		    *  returned an open stream
		    */
					if (is_Renamed != null)
					{
						try
						{
							is_Renamed.Close();
						}
						catch (IOException ioe)
						{
						}
					}
				}
			}

			return null;
		}
	}
}