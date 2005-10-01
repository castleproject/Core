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
namespace Castle.MonoRail.Views.Brail

import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps

class BrailPreProcessor(AbstractCompilerStep):
    override def Run():
        new = []
        for input in self.Parameters.Input:
            using reader=input.Open():
                code = Booify(reader.ReadToEnd())
                #print join("${i+1}:${line}" for i as int, line in enumerate(System.IO.StringReader(code)), "\n")
                new.Add(StringInput(input.Name, code))
        self.Parameters.Input.Clear()
        for input in new:
            self.Parameters.Input.Add(input)                
        
    static def Booify(code as string):
        buffer = System.IO.StringWriter()
        output = def(code as string):
            return if len(code) == 0
            buffer.Write('output """')
            buffer.Write(code)
            buffer.WriteLine('"""')
        
        lastIndex = 0
        index = code.IndexOf("<%")
        while index > -1:
			
            output(code[lastIndex:index])
            lastIndex = code.IndexOf("%>", index + 2)
            raise 'expected %>' if lastIndex < 0
            buffer.Write(code[index+2:lastIndex])
            lastIndex += 2
            index = code.IndexOf("<%", lastIndex)   
        output(code[lastIndex:])
        return buffer.ToString()
