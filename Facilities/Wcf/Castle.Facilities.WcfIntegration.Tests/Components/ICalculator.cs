// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration.Tests
{
	using System.ServiceModel;
	using System.ServiceModel.Web;

	[ServiceContract]
	public interface ICalculator
	{
		[OperationContract]
		[WebGet(UriTemplate = "add?x={x}&y={y}")]
		long Add(long x, long y);

		[OperationContract]
		[WebGet(BodyStyle = WebMessageBodyStyle.Wrapped)]
		long Subtract(long x, long y);

		[OperationContract]
		[WebInvoke(UriTemplate = "mult?x={x}&y={y}")]
		long Multiply(long x, long y);

		[OperationContract]
		[WebInvoke(BodyStyle=WebMessageBodyStyle.Wrapped)]
		long Divide(long x, long y);
	}
}
