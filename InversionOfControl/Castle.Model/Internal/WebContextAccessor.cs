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
// limitations under the License.using System.Collections;


namespace Castle.Model.Internal
{
    using System.Collections;
    using System.Web;
    
    class WebContextAccessor : IContextAccessor
    {
        const string WebContextAccessorKey = "WebContextAccessor.Key";

        public void Push()
        {
            Stack.Push(new Hashtable());
        }

        public void Pop()
        {
            Stack.Pop();
        }

        public Stack Stack
        {
            get
            {
                Stack stack = HttpContext.Current.Items[WebContextAccessorKey] as Stack;
                if (stack == null)
                {
                    stack = new Stack();
                    HttpContext.Current.Items[WebContextAccessorKey] = stack;
                }
                return stack;

            }
        }

        public IDictionary Items
        {
            get
            {
                IDictionary items = Stack.Peek() as IDictionary;
                if (items == null)
                {
                    items = new Hashtable();
                    HttpContext.Current.Items[WebContextAccessorKey] = items;
                }
                return items;
            }
        }
    }
}