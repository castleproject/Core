// Copyright 2003-2004 The Apache Software Foundation
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
// limitations under the License.namespace Apache.Avalon.Framework.Test{	using System;	using System.Text;	using System.IO;	using System.Collections;	using Apache.Avalon.Framework;	using NUnit.Framework;
	[TestFixture]	public class ConsoleLoggerTestCase	{		StringWriter _writer;		StringBuilder _buffer;		TextWriter _consoleWriter;		public ConsoleLoggerTestCase()		{			_consoleWriter = Console.Out;		}		[SetUp]		public void Init()		{			_buffer = new StringBuilder();			_writer = new StringWriter(_buffer);			System.Console.SetOut(_writer);		}		[Test]		public void TestCommonUse()		{			ConsoleLogger logger = new ConsoleLogger();			logger.Debug("Simple message");			String content = _buffer.ToString();			Assertion.AssertEquals("[Debug] '' Simple message\r\n", content);		}		[Test]		public void TestFormat()		{			ConsoleLogger logger = new ConsoleLogger();			logger.Debug("The object {0} is not implementing {1}", this, typeof(ILogger));			String content = _buffer.ToString();			Assertion.AssertEquals("[Debug] '' The object Apache.Avalon.Framework.Test.ConsoleLoggerTestCase " + 				"is not implementing Apache.Avalon.Framework.ILogger\r\n", content);		}	}}