// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security;

[assembly: AssemblyProduct(
	#if DOTNET45 // order is important
	"Castle Core for .NET Framework 4.5"
	#elif DOTNET40
	"Castle Core for .NET Framework 4.0"
	#elif DOTNET35
	"Castle Core for .NET Framework 3.5"
	#else
	"Castle Core for .NET Core 1.0"
	#endif
)]

[assembly: AssemblyCompany("Castle Project")]
[assembly: AssemblyCopyright("Copyright (c) 2004-$CurrentYear$ Castle Project - http://www.castleproject.org")]

[assembly: AssemblyVersion("0.0.0")]
[assembly: AssemblyFileVersion("0.0.0")]
[assembly: AssemblyInformationalVersion("0.0.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#endif

[assembly: ComVisible(false)]

[assembly: CLSCompliant(true)]

#if FEATURE_SECURITY_PERMISSIONS && DOTNET40
[assembly: SecurityRules(SecurityRuleSet.Level2)]
#endif

#if FEATURE_SECURITY_PERMISSIONS && FEATURE_APTCA
[assembly: AllowPartiallyTrustedCallers]
#endif