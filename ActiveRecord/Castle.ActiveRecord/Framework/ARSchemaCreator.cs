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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;
	using System.Diagnostics;
	using System.IO;

	using NHibernate.Cfg;
	using NHibernate.Connection;
	using NHibernate.Dialect;


	/// <summary>
	/// Used to execute a script file to create/update/drop 
	/// a database schema. Inspired on NHibernate SchemaExport class.
	/// </summary>
	public class ARSchemaCreator
	{
		private readonly IDictionary connectionProperties;
		private readonly Dialect dialect;

		public ARSchemaCreator( Configuration config )
		{
			this.connectionProperties = config.Properties;
			this.dialect = Dialect.GetDialect( connectionProperties );
		}

		public void Execute( String scriptFileName )
		{
			if (scriptFileName == null)
			{
				throw new ArgumentNullException("scriptFileName");
			}

			scriptFileName = Normalize(scriptFileName);

			if (!File.Exists(scriptFileName))
			{
				throw new ArgumentException("File name could not be found: " + scriptFileName, "scriptFileName");
			}

			using(StreamReader reader = new StreamReader(File.OpenRead(scriptFileName)))
			{
				String content = reader.ReadToEnd();

				ExecuteScript(content);
			}
		}

		private void ExecuteScript(String sqlScript)
		{
			IConnectionProvider connectionProvider = 
				ConnectionProviderFactory.NewConnectionProvider( CreateConnectionProperties() );

			String[] parts = sqlScript.Split(';');
			
			if (parts.Length == 1)
			{
				parts = SplitString(sqlScript, "GO");
			}

			try
			{
				using(IDbConnection connection = connectionProvider.GetConnection())
				{
					using(IDbCommand statement = connection.CreateCommand())
					{
						foreach(String part in parts)
						{
							try
							{
								statement.CommandText = part;
								statement.CommandType = CommandType.Text;
								statement.ExecuteNonQuery();
							}
							catch(Exception ex)
							{
								// Ignored, but we output it

								Debug.WriteLine(String.Format("SQL: {0} \r\nthrew {1}. Ignoring", part, ex.Message));
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not execute schema script", ex);
			}
		}

		private string[] SplitString(string sqlScript, string split)
		{
			ArrayList parts = new ArrayList();

			int searchFrom = -1;
			int lastIndex = 0;

			for(;;)
			{
				searchFrom = sqlScript.IndexOf(split, searchFrom + 1);

				if (searchFrom != -1)
				{
					parts.Add( sqlScript.Substring(lastIndex, searchFrom - lastIndex) );

					lastIndex = searchFrom + split.Length;
				}
				else 
				{
					if (searchFrom == -1 && lastIndex != 0)
					{
						parts.Add( sqlScript.Substring(lastIndex) );
					}
					break;
				}
			}

			return (String[]) parts.ToArray( typeof(String) );
		}

		private IDictionary CreateConnectionProperties()
		{
			IDictionary props = new HybridDictionary();
	
			foreach(DictionaryEntry entry in dialect.DefaultProperties)
			{
				props[entry.Key] = entry.Value;
			}
	
			if(connectionProperties != null)
			{
				foreach(DictionaryEntry entry in connectionProperties)
				{
					props[entry.Key] = entry.Value;
				}
			}

			return props;
		}

		private string Normalize(string fileName)
		{
			if (!Path.IsPathRooted(fileName))
			{
				return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, fileName );
			}
			else
			{
				return fileName;
			}
		}
	}
}
