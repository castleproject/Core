// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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


#if (!SILVERLIGHT && !MONO && !NETCORE)

namespace Castle.DynamicProxy.Tests
{
	using System.Reflection;

	using ADODB;

	using Xunit;

	public class ComInterfacesTests : BasePEVerifyTestCase
	{
		[Fact]
		public void Can_proxy_ADO_RecorsSet()
		{
			generator.CreateInterfaceProxyWithoutTarget<Recordset>();
		}

		[Fact]
		public void Can_proxy_existing_com_object()
		{
			var command = new Command();
			var parameter = ExtractActualComParameterObject(command.CreateParameter("foo"));
			generator.CreateInterfaceProxyWithTargetInterface(parameter);
		}

		private Parameter ExtractActualComParameterObject(Parameter parameter)
		{
			var type = parameter.GetType();
			var method = type.GetMethod("GetParm", BindingFlags.Instance | BindingFlags.NonPublic);
			var actualParameter = method.Invoke(parameter, null);
			return (Parameter)actualParameter;
		}
	}
}

#endif