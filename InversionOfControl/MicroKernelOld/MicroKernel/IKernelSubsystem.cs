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
	/// Subsystems are used by the kernel to 
	/// expose information or implement some logic which may
	/// increment the kernel functionality.
	/// <para>
	/// Tipical subsystems would be a configuration subsystem,
	/// a logger subsystem and even a LookupCriteria subsystem.
	/// </para>
	/// </summary>
	/// <remarks>
	/// There are specific points in the default kernel implementation
	/// which requests an specific subsystem. The subsystems keys may be 
	/// found in <see cref="KernelConstants"/>.
	/// </remarks>
	public interface IKernelSubsystem
	{
		void Init(IKernel kernel);
	}
}