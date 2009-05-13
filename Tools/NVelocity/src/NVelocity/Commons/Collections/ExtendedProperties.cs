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

namespace Commons.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	/// <summary>
	/// This class extends normal Java properties by adding the possibility
	/// to use the same key many times concatenating the value strings
	/// instead of overwriting them.
	///
	/// <para>The Extended Properties syntax is explained here:
	///
	/// <ul>
	/// <li>
	/// Each property has the syntax <code>key = value</code>
	/// </li>
	/// <li>
	/// The <i>key</i> may use any character but the equal sign '='.
	/// </li>
	/// <li>
	/// <i>value</i> may be separated on different lines if a backslash
	/// is placed at the end of the line that continues below.
	/// </li>
	/// <li>
	/// If <i>value</i> is a list of strings, each token is separated
	/// by a comma ','.
	/// </li>
	/// <li>
	/// Commas in each token are escaped placing a backslash right before
	/// the comma.
	/// </li>
	/// <li>
	/// If a <i>key</i> is used more than once, the values are appended
	/// like if they were on the same line separated with commas.
	/// </li>
	/// <li>
	/// Blank lines and lines starting with character '#' are skipped.
	/// </li>
	/// <li>
	/// If a property is named "include" (or whatever is defined by
	/// setInclude() and getInclude() and the value of that property is
	/// the full path to a file on disk, that file will be included into
	/// the ConfigurationsRepository. You can also pull in files relative
	/// to the parent configuration file. So if you have something
	/// like the following:
	///
	/// include = additional.properties
	///
	/// Then "additional.properties" is expected to be in the same
	/// directory as the parent configuration file.
	///
	/// Duplicate name values will be replaced, so be careful.
	///
	/// </li>
	/// </ul>
	/// </para>
	/// <para>Here is an example of a valid extended properties file:
	/// </para>
	/// <para><pre>
	/// # lines starting with # are comments
	///
	/// # This is the simplest property
	/// key = value
	///
	/// # A long property may be separated on multiple lines
	/// longvalue = aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa \
	/// aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
	///
	/// # This is a property with many tokens
	/// tokens_on_a_line = first token, second token
	///
	/// # This sequence generates exactly the same result
	/// tokens_on_multiple_lines = first token
	/// tokens_on_multiple_lines = second token
	///
	/// # commas may be escaped in tokens
	/// commas.excaped = Hi\, what'up?
	/// </pre>
	/// </para>
	/// <para><b>NOTE</b>: this class has <b>not</b> been written for
	/// performance nor low memory usage.  In fact, it's way slower than it
	/// could be and generates too much memory garbage.  But since
	/// performance is not an issue during intialization (and there is not
	/// much time to improve it), I wrote it this way.  If you don't like
	/// it, go ahead and tune it up!</para>
	/// </summary>
	public class ExtendedProperties : Hashtable
	{
		private static readonly Byte DEFAULT_BYTE = 0;
		private static readonly Boolean DEFAULT_BOOLEAN = false;
		private static readonly Int32 DEFAULT_INT32 = 0;
		private static readonly Single DEFAULT_SINGLE = 0;
		private static readonly Int64 DEFAULT_INT64 = 0;
		private static readonly Double DEFAULT_DOUBLE = 0;

		/// <summary> Default configurations repository.
		/// </summary>
		private ExtendedProperties defaults;

		/// <summary>
		/// The file connected to this repository (holding comments and such).
		/// </summary>
		protected internal String file;

		/// <summary>
		/// Base path of the configuration file used to create
		/// this ExtendedProperties object.
		/// </summary>
		protected internal String basePath;

		/// <summary>
		/// File separator.
		/// </summary>
		protected internal String fileSeparator = Path.AltDirectorySeparatorChar.ToString();

		/// <summary>
		/// Has this configuration been initialized.
		/// </summary>
		protected internal bool isInitialized = false;

		/// <summary>
		/// This is the name of the property that can point to other
		/// properties file for including other properties files.
		/// </summary>
		protected internal static String include = "include";

		/// <summary>
		/// These are the keys in the order they listed
		/// in the configuration file. This is useful when
		/// you wish to perform operations with configuration
		/// information in a particular order.
		/// </summary>
		protected internal ArrayList keysAsListed = new ArrayList();

		/// <summary>
		/// Creates an empty extended properties object.
		/// </summary>
		public ExtendedProperties()
		{
		}

		/// <summary>
		/// Creates and loads the extended properties from the specified
		/// file.
		/// </summary>
		/// <param name="file">A String.</param>
		/// <exception cref="IOException" />
		public ExtendedProperties(String file) : this(file, null)
		{
		}

		/// <summary>
		/// Creates and loads the extended properties from the specified
		/// file.
		/// </summary>
		/// <param name="file">A String.</param>
		/// <param name="defaultFile">File to load defaults from.</param>
		/// <exception cref="IOException" />
		public ExtendedProperties(String file, String defaultFile)
		{
			this.file = file;

			basePath = new FileInfo(file).FullName;
			basePath = basePath.Substring(0, (basePath.LastIndexOf(fileSeparator) + 1) - (0));

			Load(new FileStream(file, FileMode.Open, FileAccess.Read));

			if (defaultFile != null)
			{
				defaults = new ExtendedProperties(defaultFile);
			}
		}

		//Not used
		///// <summary>
		///// Private initializer method that sets up the generic
		///// resources.
		///// </summary>
		///// <exception cref="IOException">if there was an I/O problem.</exception>
		//private void Init(ExtendedProperties exp)
		//{
		//    isInitialized = true;
		//}

		/// <summary>
		/// Indicate to client code whether property
		/// resources have been initialized or not.
		/// </summary>
		public bool IsInitialized()
		{
			return isInitialized;
		}

		public String Include
		{
			get { return include; }
			set { include = value; }
		}

		public new IEnumerable Keys
		{
			get { return keysAsListed; }
		}

		public void Load(Stream input)
		{
			Load(input, null);
		}

		/// <summary>
		/// Load the properties from the given input stream
		/// and using the specified encoding.
		/// </summary>
		/// <param name="input">An InputStream.
		/// </param>
		/// <param name="encoding">An encoding.
		/// </param>
		/// <exception cref="IOException" />
		public void Load(Stream input, String encoding)
		{
			lock(this)
			{
				PropertiesReader reader = null;
				if (encoding != null)
				{
					try
					{
						reader = new PropertiesReader(new StreamReader(input, Encoding.GetEncoding(encoding)));
					}
					catch(IOException)
					{
						// Get one with the default encoding...
					}
				}

				if (reader == null)
				{
					reader = new PropertiesReader(new StreamReader(input));
				}

				try
				{
					while(true)
					{
						String line = reader.ReadProperty();

						if (line == null)
						{
							break;
						}

						int equalSignIndex = line.IndexOf('=');

						if (equalSignIndex > 0)
						{
							String key = line.Substring(0, (equalSignIndex) - (0)).Trim();
							String value = line.Substring(equalSignIndex + 1).Trim();

							/*
							 * Configure produces lines like this ... just
							 * ignore them.
							 */
							if (String.Empty.Equals(value))
							{
								continue;
							}

							if (Include != null && key.ToUpper().Equals(Include.ToUpper()))
							{
								/*
								 * Recursively load properties files.
								 */
								FileInfo file;

								if (value.StartsWith(fileSeparator))
								{
									/*
									 * We have an absolute path so we'll
									 * use this.
									 */
									file = new FileInfo(value);
								}
								else
								{
									/*
									 * We have a relative path, and we have
									 * two possible forms here. If we have the
									 * "./" form then just strip that off first
									 * before continuing.
									 */
									if (value.StartsWith(string.Format(".{0}", fileSeparator)))
									{
										value = value.Substring(2);
									}
									file = new FileInfo(basePath + value);
								}

								bool tmpBool;
								if (File.Exists(file.FullName))
								{
									tmpBool = true;
								}
								else
								{
									tmpBool = Directory.Exists(file.FullName);
								}
								// TODO: make sure file is readable or handle exception appropriately
								//if (file != null && tmpBool && file.canRead()) {
								if (tmpBool)
								{
									Load(new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
								}
							}
							else
							{
								AddProperty(key, value);
							}
						}
					}
				}
				catch(NullReferenceException)
				{
					/*
					 * Should happen only when EOF is reached.
					 */
					return;
				}
				reader.Close();
			}
		}

		/// <summary>  Gets a property from the configuration.
		/// *
		/// </summary>
		/// <param name="key">property to retrieve
		/// </param>
		/// <returns>value as object. Will return user value if exists,
		/// if not then default value if exists, otherwise null
		///
		/// </returns>
		public Object GetProperty(String key)
		{
			/*
	    *  first, try to get from the 'user value' store
	    */
			Object o = this[key];

			if (o == null)
			{
				// if there isn't a value there, get it from the
				// defaults if we have them
				if (defaults != null)
				{
					o = defaults[key];
				}
			}

			return o;
		}

		/// <summary> Add a property to the configuration. If it already
		/// exists then the value stated here will be added
		/// to the configuration entry. For example, if
		/// *
		/// resource.loader = file
		/// *
		/// is already present in the configuration and you
		/// *
		/// addProperty("resource.loader", "classpath")
		/// *
		/// Then you will end up with a Vector like the
		/// following:
		/// *
		/// ["file", "classpath"]
		/// *
		/// </summary>
		/// <param name="key"></param>
		/// <param name="token"></param>
		public void AddProperty(String key, Object token)
		{
			Object o = this[key];

			/*
	    *  $$$ GMJ
	    *  FIXME : post 1.0 release, we need to not assume
	    *  that a scalar is a String - it can be an Object
	    *  so we should make a little vector-like class
	    *  say, Foo that wraps (not extends Vector),
	    *  so we can do things like
	    *  if ( !( o instanceof Foo) )
	    *  so we know it's our 'vector' container
	    *
	    *  This applies throughout
	    */

			if (o is String)
			{
				ArrayList v = new ArrayList(2);
				v.Add(o);
				v.Add(token);
				CollectionsUtil.PutElement(this, key, v);
			}
			else if (o is ArrayList)
			{
				((ArrayList) o).Add(token);
			}
			else
			{
				/*
		* This is the first time that we have seen
		* request to place an object in the 
		* configuration with the key 'key'. So
		* we just want to place it directly into
		* the configuration ... but we are going to
		* make a special exception for String objects
		* that contain "," characters. We will take
		* CSV lists and turn the list into a vector of
		* Strings before placing it in the configuration.
		* This is a concession for Properties and the
		* like that cannot parse multiple same key
		* values.
		*/
				if (token is String && ((String) token).IndexOf(PropertiesTokenizer.DELIMITER) > 0)
				{
					PropertiesTokenizer tokenizer = new PropertiesTokenizer((String) token);

					while(tokenizer.HasMoreTokens())
					{
						String s = tokenizer.NextToken();

						/*
			* we know this is a string, so make sure it
			* just goes in rather than risking vectorization
			* if it contains an escaped comma
			*/
						AddStringProperty(key, s);
					}
				}
				else
				{
					/*
		    * We want to keep track of the order the keys
		    * are parsed, or dynamically entered into
		    * the configuration. So when we see a key
		    * for the first time we will place it in
		    * an ArrayList so that if a client class needs
		    * to perform operations with configuration
		    * in a definite order it will be possible.
		    */
					AddPropertyDirect(key, token);
				}
			}
		}

		/// <summary>   Adds a key/value pair to the map.  This routine does
		/// no magic morphing.  It ensures the keyList is maintained
		/// *
		/// </summary>
		/// <param name="key">key to use for mapping
		/// </param>
		/// <param name="obj">object to store
		///
		/// </param>
		private void AddPropertyDirect(String key, Object obj)
		{
			/*
	    * safety check
	    */

			if (!ContainsKey(key))
			{
				keysAsListed.Add(key);
			}

			/*
	    * and the value
	    */
			CollectionsUtil.PutElement(this, key, obj);
		}

		/// <summary>  Sets a string property w/o checking for commas - used
		/// internally when a property has been broken up into
		/// strings that could contain escaped commas to prevent
		/// the inadvertent vectorization.
		///
		/// Thanks to Leon Messerschmidt for this one.
		///
		/// </summary>
		private void AddStringProperty(String key, String token)
		{
			Object o = this[key];

			/*
	    *  $$$ GMJ
	    *  FIXME : post 1.0 release, we need to not assume
	    *  that a scalar is a String - it can be an Object
	    *  so we should make a little vector-like class
	    *  say, Foo that wraps (not extends Vector),
	    *  so we can do things like
	    *  if ( !( o instanceof Foo) )
	    *  so we know it's our 'vector' container
	    *
	    *  This applies throughout
	    */

			/*
	    *  do the usual thing - if we have a value and 
	    *  it's scalar, make a vector, otherwise add
	    *  to the vector
	    */

			if (o is String)
			{
				ArrayList v = new ArrayList(2);
				v.Add(o);
				v.Add(token);
				CollectionsUtil.PutElement(this, key, v);
			}
			else if (o is ArrayList)
			{
				((ArrayList) o).Add(token);
			}
			else
			{
				AddPropertyDirect(key, token);
			}
		}

		/// <summary> Set a property, this will replace any previously
		/// set values. Set values is implicitly a call
		/// to clearProperty(key), addProperty(key,value).
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetProperty(String key, Object value)
		{
			ClearProperty(key);
			AddProperty(key, value);
		}

		/// <summary> Save the properties to the given outputStream.
		/// </summary>
		/// <param name="output">An OutputStream.
		/// </param>
		/// <param name="Header">A String.
		/// </param>
		/// <exception cref="IOException">
		/// </exception>
		public void Save(TextWriter output, String Header)
		{
			lock(this)
			{
				if (output != null)
				{
					TextWriter textWriter = output;
					if (Header != null)
					{
						textWriter.WriteLine(Header);
					}

					foreach(String key in Keys)
					{
						Object value = this[key];
						if (value == null)
						{
							continue;
						}

						if (value is String)
						{
							WriteKeyOutput(textWriter, key, (String) value);
						}
						else if (value is IEnumerable)
						{
							foreach(String currentElement in (IEnumerable) value)
								WriteKeyOutput(textWriter, key, currentElement);
						}

						textWriter.WriteLine();
						textWriter.Flush();
					}
				}
			}
		}

		private void WriteKeyOutput(TextWriter textWriter, String key, String value)
		{
			StringBuilder currentOutput = new StringBuilder();
			currentOutput.Append(key).Append("=").Append(value);
			textWriter.WriteLine(currentOutput.ToString());
		}

		/// <summary> Combines an existing Hashtable with this Hashtable.
		/// *
		/// Warning: It will overwrite previous entries without warning.
		/// *
		/// </summary>
		/// <param name="c">ExtendedProperties
		///
		/// </param>
		public void Combine(ExtendedProperties c)
		{
			foreach(String key in c.Keys)
			{
				Object o = c[key];
				// if the value is a String, escape it so that if there are delimiters that the value is not converted to a list
				if (o is String)
				{
					o = ((String) o).Replace(",", @"\,");
				}

				SetProperty(key, o);
			}
		}

		/// <summary> Clear a property in the configuration.
		/// *
		/// </summary>
		/// <param name="key">key to remove along with corresponding value.
		///
		/// </param>
		public void ClearProperty(String key)
		{
			if (ContainsKey(key))
			{
				/*
				* we also need to rebuild the keysAsListed or else
				* things get *very* confusing
				*/

				for(int i = 0; i < keysAsListed.Count; i++)
				{
					if (((String) keysAsListed[i]).Equals(key))
					{
						keysAsListed.RemoveAt(i);
						break;
					}
				}

				Remove(key);
			}
		}

		/// <summary> Get the list of the keys contained in the configuration
		/// repository.
		/// *
		/// </summary>
		/// <returns>An Iterator.
		///
		/// </returns>
		/// <summary> Get the list of the keys contained in the configuration
		/// repository that match the specified prefix.
		/// *
		/// </summary>
		/// <param name="prefix">The prefix to test against.
		/// </param>
		/// <returns>An Iterator of keys that match the prefix.
		///
		/// </returns>
		public IEnumerable GetKeys(String prefix)
		{
			ArrayList matchingKeys = new ArrayList();

			foreach(Object key in Keys)
			{
				if (key is String && ((String) key).StartsWith(prefix))
				{
					matchingKeys.Add(key);
				}
			}
			return matchingKeys;
		}

		/// <summary> Create an ExtendedProperties object that is a subset
		/// of this one. Take into account duplicate keys
		/// by using the setProperty() in ExtendedProperties.
		/// *
		/// </summary>
		/// <param name="prefix">prefix
		///
		/// </param>
		public ExtendedProperties Subset(String prefix)
		{
			ExtendedProperties c = new ExtendedProperties();
			bool validSubset = false;

			foreach(Object key in Keys)
			{
				if (key is String && ((String) key).StartsWith(prefix))
				{
					if (!validSubset)
						validSubset = true;

					String newKey;

					/*
					* Check to make sure that c.subset(prefix) doesn't
					* blow up when there is only a single property
					* with the key prefix. This is not a useful
					* subset but it is a valid subset.
					*/
					if (((String) key).Length == prefix.Length)
					{
						newKey = prefix;
					}
					else
					{
						newKey = ((String) key).Substring(prefix.Length + 1);
					}

					/*
						*  use addPropertyDirect() - this will plug the data as 
						*  is into the Map, but will also do the right thing
						*  re key accounting
						*/

					c.AddPropertyDirect(newKey, this[key]);
				}
			}

			if (validSubset)
			{
				return c;
			}
			else
			{
				return null;
			}
		}

		/// <summary> Display the configuration for debugging
		/// purposes.
		/// </summary>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach(String key in Keys)
			{
				Object value = this[key];
				sb.AppendFormat("{0} => {1}", key, ValueToString(value)).Append(Environment.NewLine);
			}
			return sb.ToString();
		}

		private String ValueToString(Object value)
		{
			if (value is ArrayList)
			{
				String s = "ArrayList :: ";
				foreach(Object o in (ArrayList) value)
				{
					if (!s.EndsWith(", "))
					{
						s += ", ";
					}
					s += string.Format("[{0}]", o);
				}
				return s;
			}
			else
			{
				return value.ToString();
			}
		}

		/// <summary> Get a string associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated string.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a String.
		///
		/// </exception>
		public String GetString(String key)
		{
			return GetString(key, null);
		}

		/// <summary> Get a string associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated string if key is found,
		/// default value otherwise.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a String.
		///
		/// </exception>
		public String GetString(String key, String defaultValue)
		{
			Object value = this[key];

			if (value is String)
			{
				return (String) value;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return defaultValue;
				}
				else
				{
					return defaults.GetString(key, defaultValue);
				}
			}
			else if (value is ArrayList)
			{
				return (String) ((ArrayList) value)[0];
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a String object", '\'', key));
			}
		}

		/// <summary> Get a list of properties associated with the given
		/// configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated properties if key is found.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a String/Vector.
		/// </exception>
		/// <exception cref="ArgumentException"> if one of the tokens is
		/// malformed (does not contain an equals sign).
		///
		/// </exception>
		public Hashtable GetProperties(String key)
		{
			//UPGRADE_TODO: Format of property file may need to be changed. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1089"'
			return GetProperties(key, new Hashtable());
		}

		/// <summary> Get a list of properties associated with the given
		/// configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultProps">Default property values.
		/// </param>
		/// <returns>The associated properties if key is found.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a String/Vector.
		/// </exception>
		/// <exception cref="ArgumentException"> if one of the tokens is
		/// malformed (does not contain an equals sign).
		///
		/// </exception>
		public Hashtable GetProperties(String key, Hashtable defaultProps)
		{
			/*
	    * Grab an array of the tokens for this key.
	    */
			String[] tokens = GetStringArray(key);

			/*
	    * Each token is of the form 'key=value'.
	    */
			Hashtable props = new Hashtable(defaultProps);
			for(int i = 0; i < tokens.Length; i++)
			{
				String token = tokens[i];
				int equalSign = token.IndexOf('=');
				if (equalSign > 0)
				{
					String pkey = token.Substring(0, (equalSign) - (0)).Trim();
					String pvalue = token.Substring(equalSign + 1).Trim();
					CollectionsUtil.PutElement(props, pkey, pvalue);
				}
				else
				{
					throw new ArgumentException(string.Format("{0}{1}' does not contain an equals sign", '\'', token));
				}
			}
			return props;
		}

		/// <summary> Get an array of strings associated with the given configuration
		/// key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated string array if key is found.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a String/Vector.
		///
		/// </exception>
		public String[] GetStringArray(String key)
		{
			Object value = this[key];

			// What's your vector, Victor?
			ArrayList vector;
			if (value is String)
			{
				vector = new ArrayList(1);
				vector.Add(value);
			}
			else if (value is ArrayList)
			{
				vector = (ArrayList) value;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return new String[0];
				}
				else
				{
					return defaults.GetStringArray(key);
				}
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a String/Vector object", '\'', key));
			}

			String[] tokens = new String[vector.Count];
			for(int i = 0; i < tokens.Length; i++)
			{
				tokens[i] = (String) vector[i];
			}

			return tokens;
		}

		/// <summary> Get a Vector of strings associated with the given configuration
		/// key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated Vector.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Vector.
		///
		/// </exception>
		public ArrayList GetVector(String key)
		{
			return GetVector(key, null);
		}

		/// <summary>
		/// Gets the string list.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public List<string> GetStringList(String key)
		{
			Object value = this[key];
			return (List<string>) value;
		}

		/// <summary> Get a Vector of strings associated with the given configuration
		/// key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated Vector.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Vector.
		///
		/// </exception>
		public ArrayList GetVector(String key, ArrayList defaultValue)
		{
			Object value = this[key];

			if (value is ArrayList)
			{
				return (ArrayList) value;
			}
			else if (value is String)
			{
				ArrayList v = new ArrayList(1);
				v.Add(value);
				CollectionsUtil.PutElement(this, key, v);
				return v;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return (defaultValue ?? new ArrayList());
				}
				else
				{
					return defaults.GetVector(key, defaultValue);
				}
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Vector object", '\'', key));
			}
		}

		/// <summary> Get a boolean associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated boolean.
		/// </returns>
		/// <exception cref="Exception"> is thrown if the key doesn't
		/// map to an existing object.
		/// </exception>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Boolean.
		///
		/// </exception>
		public bool GetBoolean(String key)
		{
			Boolean b = GetBoolean(key, DEFAULT_BOOLEAN);
			if ((Object) b == null)
			{
				throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
			}
			else
			{
				return b;
			}
		}

		/// <summary> Get a boolean associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated boolean if key is found and has valid
		/// format, default value otherwise.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Boolean.
		///
		/// </exception>
		public Boolean GetBoolean(String key, Boolean defaultValue)
		{
			Object value = this[key];

			if (value is Boolean)
			{
				return (Boolean) value;
			}
			else if (value is String)
			{
				String s = TestBoolean((String) value);
				Boolean b = s.ToUpper().Equals("TRUE");
				CollectionsUtil.PutElement(this, key, b);
				return b;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return defaultValue;
				}
				else
				{
					return defaults.GetBoolean(key, defaultValue);
				}
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Boolean object", '\'', key));
			}
		}

		/// <summary> Test whether the string represent by value maps to a boolean
		/// value or not. We will allow <code>true</code>, <code>on</code>,
		/// and <code>yes</code> for a <code>true</code> boolean value, and
		/// <code>false</code>, <code>off</code>, and <code>no</code> for
		/// <code>false</code> boolean values.  Case of value to test for
		/// boolean status is ignored.
		/// *
		/// </summary>
		/// <param name="value">The value to test for boolean state.
		/// </param>
		/// <returns><code>true</code> or <code>false</code> if the supplied
		/// text maps to a boolean value, or <code>null</code> otherwise.
		///
		/// </returns>
		public String TestBoolean(String value)
		{
			String s = value.ToLower();

			if (s.Equals("true") || s.Equals("on") || s.Equals("yes"))
			{
				return "true";
			}
			else if (s.Equals("false") || s.Equals("off") || s.Equals("no"))
			{
				return "false";
			}
			else
			{
				return null;
			}
		}

		/// <summary> Get a byte associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated byte if key is found and has valid
		/// format, <see cref="DEFAULT_BYTE"/> otherwise.
		/// </returns>
		/// <exception cref="Exception"> is thrown if the key doesn't
		/// map to an existing object.
		/// </exception>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Byte.
		/// </exception>
		public sbyte GetByte(String key)
		{
			if (ContainsKey(key))
			{
				Byte b = GetByte(key, DEFAULT_BYTE);
				return (sbyte) b;
			}
			else
			{
				throw new Exception(string.Format("{0}{1} doesn't map to an existing object", '\'', key));
			}
		}

		/// <summary> Get a byte associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated byte if key is found and has valid
		/// format, default value otherwise.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Byte.
		/// </exception>
		public sbyte GetByte(String key, sbyte defaultValue)
		{
			return GetByte(key, defaultValue);
		}

		/// <summary> Get a byte associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated byte if key is found and has valid
		/// format, default value otherwise.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Byte.
		/// </exception>
		public Byte GetByte(String key, Byte defaultValue)
		{
			Object value = this[key];

			if (value is Byte)
			{
				return (Byte) value;
			}
			else if (value is String)
			{
				Byte b = Byte.Parse((String) value);
				CollectionsUtil.PutElement(this, key, b);
				return b;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return defaultValue;
				}
				else
				{
					return defaults.GetByte(key, defaultValue);
				}
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Byte object", '\'', key));
			}
		}

		/// <summary> The purpose of this method is to get the configuration resource
		/// with the given name as an integer.
		/// *
		/// </summary>
		/// <param name="name">The resource name.
		/// </param>
		/// <returns>The value of the resource as an integer.
		///
		/// </returns>
		public Int32 GetInt(String name)
		{
			return GetInteger(name);
		}

		/// <summary> The purpose of this method is to get the configuration resource
		/// with the given name as an integer, or a default value.
		/// *
		/// </summary>
		/// <param name="name">The resource name
		/// </param>
		/// <param name="def">The default value of the resource.
		/// </param>
		/// <returns>The value of the resource as an integer.
		///
		/// </returns>
		public Int32 GetInt(String name, int def)
		{
			return GetInteger(name, def);
		}

		/// <summary> Get a int associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated int if key is found and has valid
		/// format, <see cref="DEFAULT_INT32"/> otherwise.
		/// </returns>
		/// <exception cref="Exception"> is thrown if the key doesn't
		/// map to an existing object.
		/// </exception>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Integer.
		/// </exception>
		public Int32 GetInteger(String key)
		{
			Int32 i = GetInteger(key, DEFAULT_INT32);
			if ((Object) i == null)
			{
				throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
			}
			else
			{
				return i;
			}
		}

		/// <summary> Get a int associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated int if key is found and has valid
		/// format, <see cref="DEFAULT_INT32"/> otherwise.
		/// </returns>
		/// <returns>The associated int if key is found and has valid
		/// format, default value otherwise.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Integer.
		/// </exception>
		public Int32 GetInteger(String key, Int32 defaultValue)
		{
			Object value = this[key];

			if (value is Int32)
			{
				return (Int32) value;
			}
			else if (value is String)
			{
				Int32 i = Int32.Parse((String) value);
				CollectionsUtil.PutElement(this, key, i);
				return i;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return defaultValue;
				}
				else
				{
					return defaults.GetInteger(key, defaultValue);
				}
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Integer object", '\'', key));
			}
		}

		/// <summary> Get a long associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated long if key is found and has valid
		/// format, <see cref="DEFAULT_INT64"/> otherwise.
		/// </returns>
		/// <exception cref="Exception"> is thrown if the key doesn't
		/// map to an existing object.
		/// </exception>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Long.
		/// </exception>
		public Int64 GetLong(String key)
		{
			Int64 l = GetLong(key, DEFAULT_INT64);
			if ((Object) l == null)
			{
				throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
			}
			else
			{
				return l;
			}
		}

		/// <summary> Get a long associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated long if key is found and has valid
		/// format, <see cref="DEFAULT_INT64"/> otherwise.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Long.
		/// </exception>
		public Int64 GetLong(String key, Int64 defaultValue)
		{
			Object value = this[key];

			if (value is Int64)
			{
				return (Int64) value;
			}
			else if (value is String)
			{
				Int64 l = Int64.Parse((String) value);
				CollectionsUtil.PutElement(this, key, l);
				return l;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return defaultValue;
				}
				else
				{
					return defaults.GetLong(key, defaultValue);
				}
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Long object", '\'', key));
			}
		}

		/// <summary> Get a float associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated float if key is found and has valid
		/// format, <see cref="DEFAULT_SINGLE"/> otherwise.
		/// </returns>
		/// <exception cref="Exception"> is thrown if the key doesn't
		/// map to an existing object.
		/// </exception>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Float.
		/// </exception>
		public float GetFloat(String key)
		{
			Single f = GetFloat(key, DEFAULT_SINGLE);
			if ((Object) f == null)
			{
				throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
			}
			else
			{
				return f;
			}
		}

		/// <summary> Get a float associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated float if key is found and has valid
		/// format, <see cref="DEFAULT_SINGLE"/> otherwise.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Float.
		/// </exception>
		public Single GetFloat(String key, Single defaultValue)
		{
			Object value = this[key];

			if (value is Single)
			{
				return (Single) value;
			}
			else if (value is String)
			{
				//UPGRADE_TODO: Format of parameters of constructor 'java.lang.Float.Float' are different in the equivalent in .NET. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1092"'
				Single f = Single.Parse((String) value);
				CollectionsUtil.PutElement(this, key, f);
				return f;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return defaultValue;
				}
				else
				{
					return defaults.GetFloat(key, defaultValue);
				}
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Float object", '\'', key));
			}
		}

		/// <summary> Get a double associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <returns>The associated double if key is found and has valid
		/// format, <see cref="DEFAULT_DOUBLE"/> otherwise.
		/// </returns>
		/// <exception cref="Exception"> is thrown if the key doesn't
		/// map to an existing object.
		/// </exception>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Double.
		/// </exception>
		public Double GetDouble(String key)
		{
			Double d = GetDouble(key, DEFAULT_DOUBLE);
			if ((Object) d == null)
			{
				throw new Exception(string.Format("{0}{1}' doesn't map to an existing object", '\'', key));
			}
			else
			{
				return d;
			}
		}

		/// <summary> Get a double associated with the given configuration key.
		/// *
		/// </summary>
		/// <param name="key">The configuration key.
		/// </param>
		/// <param name="defaultValue">The default value.
		/// </param>
		/// <returns>The associated double if key is found and has valid
		/// format, <see cref="DEFAULT_DOUBLE"/> otherwise.
		/// </returns>
		/// <exception cref="InvalidCastException"> is thrown if the key maps to an
		/// object that is not a Double.
		/// </exception>
		public Double GetDouble(String key, Double defaultValue)
		{
			Object value = this[key];

			if (value is Double)
			{
				return (Double) value;
			}
			else if (value is String)
			{
				//UPGRADE_TODO: Format of parameters of constructor 'java.lang.Double.Double' are different in the equivalent in .NET. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1092"'
				Double d = Double.Parse((String) value);
				CollectionsUtil.PutElement(this, key, d);
				return d;
			}
			else if (value == null)
			{
				if (defaults == null)
				{
					return defaultValue;
				}
				else
				{
					return defaults.GetDouble(key, defaultValue);
				}
			}
			else
			{
				throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a Double object", '\'', key));
			}
		}

		/// <summary>
		/// Convert a standard properties class into a configuration class.
		/// </summary>
		/// <param name="p">properties object to convert into a ExtendedProperties object.</param>
		/// <returns>ExtendedProperties configuration created from the properties object.</returns>
		public static ExtendedProperties ConvertProperties(ExtendedProperties p)
		{
			ExtendedProperties c = new ExtendedProperties();

			foreach(String key in p.Keys)
			{
				Object value = p.GetProperty(key);

				// if the value is a String, escape it so that if there are delimiters that the value is not converted to a list
				if (value is String)
					value = value.ToString().Replace(",", @"\,");
				c.SetProperty(key, value);
			}

			return c;
		}
	}
}