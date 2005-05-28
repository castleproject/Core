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

namespace Castle.Rook.Compiler.Services.Default
{
	using System.IO;
	using System.Text;

	public class DebugErrorReport : ErrorReport
	{
		private readonly TextWriter outWriter;
		private readonly TextWriter errorWriter;
		private readonly StringBuilder sbout;
		private readonly StringBuilder sberror;

		public DebugErrorReport()
		{
			sbout = new StringBuilder();
			outWriter = new StringWriter(sbout);

			sberror = new StringBuilder();
			errorWriter = new StringWriter(sberror);
		}

		protected override TextWriter OutWriter
		{
			get { return outWriter; }
		}

		protected override TextWriter ErrorWriter
		{
			get { return errorWriter; }
		}

		public StringBuilder OutSBuilder
		{
			get { return sbout; }
		}

		public StringBuilder ErrorSBuilder
		{
			get { return sberror; }
		}
	}
}
