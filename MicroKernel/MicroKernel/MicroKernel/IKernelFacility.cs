// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel
{
    /// <summary>
    /// The facility role is to increment the kernel functionality 
    /// by interception the kernel working - throught events - and 
    /// acting accordingly.
    /// <para>For instance, a facility can expose your components as 
    /// managed components (managed extensions) by intercept 
    /// the components registration events. Other facility can intercept
    /// the model construction and change the logger or configuration.
    /// </para>
    /// </summary>
    public interface IKernelFacility
    {
        /// <summary>
        /// Init the facility given it a chance to subscribe
        /// to kernel events.
        /// </summary>
        /// <param name="kernel">Kernel instance</param>
        void Init(IKernel kernel);

        /// <summary>
        /// Gives a chance to the facility to unsubscribe from the
        /// events and do its proper clean up.
        /// </summary>
        void Terminate(IKernel kernel);
    }
}
