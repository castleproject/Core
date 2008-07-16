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

namespace Castle.MonoRail.Views.AspView.Compiler.StatementProcessors
{
	using OutputMethodGenerators;

	public class StatementInfo
	{
		readonly string content;
		readonly IOutputMethodGenerator generator;

		public StatementInfo(IOutputMethodGenerator generator, string content)
		{
			this.content = content;
			this.generator = generator;
		}

		public string Content { get { return content; } }

		public IOutputMethodGenerator Generator { get { return generator; } }

		public string ToCode()
		{
			return generator.GenerateFrom(content);
		}
	}
}
