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

namespace Castle.MonoRail.Views.AspView.Internal
{
	using System;
	using System.IO;

	public class TabbedStringWriter : StringWriter
	{
		const char TAB = '\t';
		int indentation = 0;

		static readonly string[] tabs = new string[] {
			string.Empty,
			string.Empty + TAB,
			string.Empty + TAB+ TAB,
			string.Empty + TAB+ TAB+ TAB
		};

		public void Indent()
		{
			++indentation;
		}

		public void UnIndent()
		{
			--indentation;
		}

		private string Tabs
		{
			get { return tabs[indentation]; }
		}

		private string Tabify(string input)
		{
			return (Tabs + input.Replace(Environment.NewLine, Environment.NewLine + Tabs))
				.TrimEnd(TAB);
		}

		public void WriteLineWithNoIndentation(string value)
		{
			base.WriteLine(value);
		}

		public override void WriteLine(string value)
		{
			base.Write(Tabify(value));
			WriteLine();
		}
		public override void WriteLine(string format, object arg0)
		{
			base.Write(Tabify(format), arg0);
			WriteLine();
		}
		public override void WriteLine(string format, object arg0, object arg1)
		{
			base.Write(Tabify(format), arg0, arg1);
			WriteLine();
		}
		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			base.Write(Tabify(format), arg0, arg1, arg2);
			WriteLine();
		}
		public override void WriteLine(string format, params object[] arg)
		{
			base.Write(Tabify(format), arg);
			WriteLine();
		}
	}
}
