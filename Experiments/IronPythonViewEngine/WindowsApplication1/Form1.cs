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

namespace WindowsApplication1
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;
	using IronPython.Hosting;

	public partial class Form1 : Form
	{
		private PythonEngine eng;
		private EngineModule mod1;
		private CompiledCode scriptCompiledCode;

		public Form1()
		{
			EngineOptions options = new EngineOptions();
			eng = new PythonEngine(options);
			eng.Import("site");
			System.IO.FileStream fs = new System.IO.FileStream("scripting-log.txt", System.IO.FileMode.Create);
			eng.SetStandardOutput(fs);
			eng.SetStandardError(fs);

			mod1 = eng.CreateModule("mr", false);

			InitializeComponent();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			output.Text = String.Empty;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			StringWriter writer = new StringWriter();
			Dictionary<string,object> locals = new Dictionary<string, object>();
			locals.Add("output", writer);
			
			try
			{
				CompiledCode functionsCode = eng.Compile(functions.Text);
				scriptCompiledCode = eng.Compile(script.Text);

				functionsCode.Execute(mod1);
				scriptCompiledCode.Execute(mod1, locals);
			}
			catch(Exception ex)
			{
				output.Text += Environment.NewLine + ex.Message + 
				               Environment.NewLine + Environment.NewLine + 
				               ex.Message +
				               Environment.NewLine + Environment.NewLine;
			}

			output.Text += writer.GetStringBuilder().ToString();
			output.Text += Environment.NewLine;
			output.Text += Environment.NewLine;
			
			eng.Shutdown();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			StringWriter writer = new StringWriter();
			Dictionary<string, object> locals = new Dictionary<string, object>();
			locals.Add("output", writer);

			try
			{
				CompiledCode functionsCode = eng.Compile(functions.Text);
				functionsCode.Execute(mod1);
				
				scriptCompiledCode.Execute(mod1, locals);
			}
			catch (Exception ex)
			{
				output.Text += Environment.NewLine + ex.Message +
							   Environment.NewLine + Environment.NewLine +
							   ex.Message +
							   Environment.NewLine + Environment.NewLine;
			}

			output.Text += writer.GetStringBuilder().ToString();
			output.Text += Environment.NewLine;
			output.Text += Environment.NewLine;
		}
	}
}