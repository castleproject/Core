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
	using Castle.Facilities.WcfIntegration.Behaviors;
	using Castle.MicroKernel;

	public class WcfBurden : IWcfBurden
	{
		private readonly IKernel kernel;
		private readonly List<object> instances = new List<object>();

		public WcfBurden(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public void Add(object instance)
		{
			instances.Add(instance);
		}

		public void CleanUp()
		{
			foreach (object instance in instances)
			{
				if (instance is IWcfCleanUp)
				{
					((IWcfCleanUp)instance).CleanUp();
				}
				else
				{
					kernel.ReleaseComponent(instance);
				}
			}
			instances.Clear();
		}
	}

	public class WcfBurdenExtension<T> : AbstractExtension<T>, IWcfCleanUp
		where T : class, IExtensibleObject<T>
	{
		private readonly IWcfBurden burden;

		public WcfBurdenExtension(IWcfBurden burden)
		{
			this.burden = burden;
		}

		public IWcfBurden Burden
		{
			get { return burden; }
		}

		public void CleanUp()
		{
			burden.CleanUp();
		}
	}
}
