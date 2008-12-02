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

namespace Castle.Facilities.WcfIntegration.Internal
{
	using System.Collections.Generic;
	using System.ServiceModel;
	using Castle.MicroKernel;

	public class WcfBurden : IWcfBurden
	{
		private readonly List<object> instances = new List<object>();

		public void Add(object instance)
		{
			instances.Add(instance);
		}

		public void Release(IKernel kernel)
		{
			foreach (object instance in instances)
			{
				kernel.ReleaseComponent(instance);
			}
			instances.Clear();
		}
	}

	public class WcfBurdenExtension<T> : WcfBurden, IExtension<T>
		where T : IExtensibleObject<T>
	{
		public void Attach(T owner)
		{
		}

		public void Detach(T owner)
		{
		}
	}
}
