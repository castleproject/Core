namespace org.apache.velocity.runtime.resource.loader
{
    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2001-2002 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */
    using System;
    using Resource = org.apache.velocity.runtime.resource.Resource;
    using ResourceNotFoundException = org.apache.velocity.exception.ResourceNotFoundException;

    /// <summary> This is a simple template file loader that loads templates
    /// from a DataSource instead of plain files.
    /// *
    /// It can be configured with a datasource name, a table name,
    /// id column (name), content column (the template body) and a
    /// datetime column (for last modification info).
    /// <br>
    /// <br>
    /// Example configuration snippet for velocity.properties:
    /// <br>
    /// <br>
    /// resource.loader = file, ds <br>
    /// <br>
    /// ds.resource.loader.public.name = DataSource <br>
    /// ds.resource.loader.description = Velocity DataSource Resource Loader <br>
    /// ds.resource.loader.class = org.apache.velocity.runtime.resource.loader.DataSourceResourceLoader <br>
    /// ds.resource.loader.resource.datasource = java:comp/env/jdbc/Velocity <br>
    /// ds.resource.loader.resource.table = tb_velocity_template <br>
    /// ds.resource.loader.resource.keycolumn = id_template <br>
    /// ds.resource.loader.resource.templatecolumn = template_definition <br>
    /// ds.resource.loader.resource.timestampcolumn = template_timestamp <br>
    /// ds.resource.loader.cache = false <br>
    /// ds.resource.loader.modificationCheckInterval = 60 <br>
    /// <br>
    /// Example WEB-INF/web.xml: <br>
    /// <br>
    /// <resource-ref> <br>
    /// <description>Velocity template DataSource</description> <br>
    /// <res-ref-name>jdbc/Velocity</res-ref-name> <br>
    /// <res-type>javax.sql.DataSource</res-type> <br>
    /// <res-auth>Container</res-auth> <br>
    /// </resource-ref> <br>
    /// <br>
    /// <br>
    /// and Tomcat 4 server.xml file: <br>
    /// [...] <br>
    /// <Context path="/exampleVelocity" docBase="exampleVelocity" debug="0"> <br>
    /// [...] <br>
    /// <ResourceParams name="jdbc/Velocity"> <br>
    /// <parameter> <br>
    /// <name>driverClassName</name> <br>
    /// <value>org.hsql.jdbcDriver</value> <br>
    /// </parameter> <br>
    /// <parameter> <br>
    /// <name>driverName</name> <br>
    /// <value>jdbc:HypersonicSQL:database</value> <br>
    /// </parameter> <br>
    /// <parameter> <br>
    /// <name>user</name> <br>
    /// <value>database_username</value> <br>
    /// </parameter> <br>
    /// <parameter> <br>
    /// <name>password</name> <br>
    /// <value>database_password</value> <br>
    /// </parameter> <br>
    /// </ResourceParams> <br>
    /// [...] <br>
    /// </Context> <br>
    /// [...] <br>
    /// <br>
    /// Example sql script:<br>
    /// CREATE TABLE tb_velocity_template ( <br>
    /// id_template varchar (40) NOT NULL , <br>
    /// template_definition text (16) NOT NULL , <br>
    /// template_timestamp datetime NOT NULL  <br>
    /// ) <br>
    /// *
    /// </summary>
    /// <author> <a href="mailto:david.kinnvall@alertir.com">David Kinnvall</a>
    /// </author>
    /// <author> <a href="mailto:paulo.gaspar@krankikom.de">Paulo Gaspar</a>
    /// </author>
    /// <author> <a href="mailto:lachiewicz@plusnet.pl">Sylwester Lachiewicz</a>
    /// </author>
    /// <version> $Id: DataSourceResourceLoader.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public class DataSourceResourceLoader:ResourceLoader {
	private System.String dataSourceName;
	private System.String tableName;
	private System.String keyColumn;
	private System.String templateColumn;
	private System.String timestampColumn;
	private InitialContext ctx;
	private DataSource dataSource;

	public virtual void  init(ExtendedProperties configuration) {
	    dataSourceName = configuration.getString("resource.datasource");
	    tableName = configuration.getString("resource.table");
	    keyColumn = configuration.getString("resource.keycolumn");
	    templateColumn = configuration.getString("resource.templatecolumn");
	    timestampColumn = configuration.getString("resource.timestampcolumn");

	    System.Diagnostics.Process.info("Resources Loaded From: " + dataSourceName + "/" + tableName);
	    System.Diagnostics.Process.info("Resource Loader using columns: " + keyColumn + ", " + templateColumn + " and " + timestampColumn);
	    System.Diagnostics.Process.info("Resource Loader Initalized.");
	}

	public override bool isSourceModified(Resource resource) {
	    return (resource.LastModified != readLastModified(resource, "checking timestamp"));
	}

	public override long getLastModified(Resource resource) {
	    return readLastModified(resource, "getting timestamp");
	}

	/// <summary> Get an InputStream so that the Runtime can build a
	/// template with it.
	/// *
	/// </summary>
	/// <param name="name">name of template
	/// </param>
	/// <returns>InputStream containing template
	///
	/// </returns>
	//UPGRADE_NOTE: Synchronized keyword was removed from method 'getResourceStream'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
	public override System.IO.Stream getResourceStream(System.String name) {
	    lock(this) {
		if (name == null || name.Length == 0) {
		    throw new ResourceNotFoundException("Need to specify a template name!");
		}

		try {
		    System.Data.OleDb.OleDbConnection conn = openDbConnection();

		    try {
			//UPGRADE_TODO: The equivalent of java.sql.ResultSet must call Close() before performing any other operation with the associated connection. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1129"'
			System.Data.OleDb.OleDbDataReader rs = readData(conn, templateColumn, name);

			try {
			    if (rs.Read()) {
				//UPGRADE_ISSUE: Method 'java.sql.ResultSet.getAsciiStream' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javasqlResultSetgetAsciiStream_javalangString"'
				return new System.IO.BufferedStream(rs.getAsciiStream(templateColumn));
			    } else {
				System.String msg = "DataSourceResourceLoader Error: cannot find resource " + name;
				System.Diagnostics.Process.error(msg);

				throw new ResourceNotFoundException(msg);
			    }
			} finally {
			    rs.Close();
			}
		    } finally {
			closeDbConnection(conn);
		    }
		} catch (System.Exception e) {
		    System.String msg = "DataSourceResourceLoader Error: database problem trying to load resource " + name + ": " + e.ToString();

		    System.Diagnostics.Process.error(msg);

		    throw new ResourceNotFoundException(msg);

		}

	    }
	}

	/// <summary>  Fetches the last modification time of the resource
	/// *
	/// </summary>
	/// <param name="resource">Resource object we are finding timestamp of
	/// </param>
	/// <param name="i_operation">string for logging, indicating caller's intention
	/// *
	/// </param>
	/// <returns>timestamp as long
	///
	/// </returns>
	private long readLastModified(Resource resource, System.String i_operation) {
	    /*
	    *  get the template name from the resource
	    */

	    System.String name = resource.Name;
	    try {
		System.Data.OleDb.OleDbConnection conn = openDbConnection();

		try {
		    //UPGRADE_TODO: The equivalent of java.sql.ResultSet must call Close() before performing any other operation with the associated connection. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1129"'
		    System.Data.OleDb.OleDbDataReader rs = readData(conn, timestampColumn, name);
		    try {
			if (rs.Read()) {
			    return (System.Convert.ToDateTime(rs[(timestampColumn)]).Ticks - 621355968000000000) / 10000;
			} else {
			    System.Diagnostics.Process.error("DataSourceResourceLoader Error: while " + i_operation + " could not find resource " + name);
			}
		    } finally {
			rs.Close();
		    }
		} finally {
		    closeDbConnection(conn);
		}
	    } catch (System.Exception e) {
		System.Diagnostics.Process.error("DataSourceResourceLoader Error: error while " + i_operation + " when trying to load resource " + name + ": " + e.ToString());
	    }
	    return 0;
	}

	/// <summary>   gets connection to the datasource specified through the configuration
	/// parameters.
	/// *
	/// </summary>
	/// <returns>connection
	///
	/// </returns>
	private System.Data.OleDb.OleDbConnection openDbConnection() {
	    if (ctx == null) {
		ctx = new InitialContext();
	    }

	    if (dataSource == null) {
		dataSource = (DataSource) ctx.lookup(dataSourceName);
	    }

	    return dataSource.Connection;
	}

	/// <summary>  Closes connection to the datasource
	/// </summary>
	private void  closeDbConnection(System.Data.OleDb.OleDbConnection conn) {
	    try {
		conn.Close();
	    } catch (System.Exception e) {
		System.Diagnostics.Process.info("DataSourceResourceLoader Quirk: problem when closing connection: " + e.ToString());
	    }
	}

	/// <summary>  Reads the data from the datasource.  It simply does the following query :
	/// <br>
	/// SELECT <i>columnNames</i> FROM <i>tableName</i> WHERE <i>keyColumn</i>
	/// = '<i>templateName</i>'
	/// <br>
	/// where <i>keyColumn</i> is a class member set in init()
	/// *
	/// </summary>
	/// <param name="conn">connection to datasource
	/// </param>
	/// <param name="columnNames">columns to fetch from datasource
	/// </param>
	/// <param name="templateName">name of template to fetch
	/// </param>
	/// <returns>result set from query
	///
	/// </returns>
	//UPGRADE_TODO: The equivalent of java.sql.ResultSet must call Close() before performing any other operation with the associated connection. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1129"'
	private System.Data.OleDb.OleDbDataReader readData(System.Data.OleDb.OleDbConnection conn, System.String columnNames, System.String templateName) {
	    System.Data.OleDb.OleDbCommand stmt = SupportClass.TransactionManager.manager.CreateStatement(conn);

	    System.String sql = "SELECT " + columnNames + " FROM " + tableName + " WHERE " + keyColumn + " = '" + templateName + "'";

	    System.Data.OleDb.OleDbCommand temp_OleDbCommand;
	    temp_OleDbCommand = stmt;
	    temp_OleDbCommand.CommandText = sql;
	    return temp_OleDbCommand.ExecuteReader();
	}
    }
}
