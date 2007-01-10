#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

namespace Castle.Igloo.LifestyleManager
{
    /// <summary>
    /// Represents a component candidate for context eviction
    /// </summary>
    public class Candidate
    {
        private string _name = string.Empty;
        private object _component = null;

        public string Name
        {
            get { return _name; }
        }

        public object Component
        {
            get { return _component; }
        }

        public Candidate(string name, object component)
        {
            _name = name;
            _component = component;
        }
    }
}
