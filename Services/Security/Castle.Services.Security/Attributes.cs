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

namespace Castle.Services.Security
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class SecureAttribute : System.Attribute 
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class DenyAttribute : PermissionAttribute 
    {
        public DenyAttribute(String role) : base(role){}
        public DenyAttribute(String[] roles) : base(roles){}
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class AllowAttribute : PermissionAttribute 
    {
        public AllowAttribute(String role) : base(role){}
        public AllowAttribute(String[] roles) : base(roles){}
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public abstract class PermissionAttribute : System.Attribute
    {
        public PermissionAttribute(String role) : this(new String[] {role}){}
        public PermissionAttribute(params String[] roles)
        {
            this._roles = roles;
        }
        private String[] _roles;

        public String[] Roles
        {
            get { return this._roles; }
        }
    }
}