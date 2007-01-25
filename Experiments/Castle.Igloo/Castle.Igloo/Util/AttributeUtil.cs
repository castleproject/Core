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

using System;
using System.Reflection;
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Util
{
    /// <summary>
    ///  Utility methods that simplifies access to attribute.
    /// </summary>
    public sealed class AttributeUtil
    {
        /// <summary>
        /// Gets the <see cref="SkipNavigationAttribute"/> on class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The attribute</returns>
        public static SkipNavigationAttribute GetSkipNavigationAttribute(Type type)
        {
            SkipNavigationAttribute attribute = Attribute.GetCustomAttribute(type, typeof(SkipNavigationAttribute)) as SkipNavigationAttribute;
            if (attribute==null)
            {
                attribute = Attribute.GetCustomAttribute(type, typeof(SkipNavigationAttribute), true) as SkipNavigationAttribute;
            }
            return attribute;
        }

        /// <summary>
        /// Gets the <see cref="SkipNavigationAttribute"/> on method.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <returns>The attribute</returns>
        public static SkipNavigationAttribute GetSkipNavigationAttribute(MemberInfo memberInfo)
        {
            SkipNavigationAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(SkipNavigationAttribute)) as SkipNavigationAttribute;
            if (attribute == null)
            {
                attribute = Attribute.GetCustomAttribute(memberInfo, typeof(SkipNavigationAttribute), true) as SkipNavigationAttribute;
            }
            return attribute;
        }

        /// <summary>
        /// Determines whether the specified properties type has the <see cref="InjectAttribute"/>.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type has <see cref="InjectAttribute"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasInjectAttribute(MemberInfo memberInfo)
        {
            InjectAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(InjectAttribute)) as InjectAttribute;
            if (attribute == null)
            {
                attribute = Attribute.GetCustomAttribute(memberInfo, typeof(InjectAttribute), true) as InjectAttribute;
            }
            return (attribute != null);
        }
        
        /// <summary>
        /// Gets the <see cref="InjectAttribute"/> on properties.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <returns>The attribute</returns>
        public static InjectAttribute GetInjectAttribute(MemberInfo memberInfo)
        {
            InjectAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(InjectAttribute)) as InjectAttribute;
            if (attribute == null)
            {
                attribute = Attribute.GetCustomAttribute(memberInfo, typeof(InjectAttribute), true) as InjectAttribute;
            }
            return attribute;
        }

        /// <summary>
        /// Determines whether the specified properties type has the <see cref="OutjectAttribute"/>.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type has <see cref="OutjectAttribute"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasOutjectAttribute(MemberInfo memberInfo)
        {
            OutjectAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(OutjectAttribute)) as OutjectAttribute;
            if (attribute == null)
            {
                attribute = Attribute.GetCustomAttribute(memberInfo, typeof(OutjectAttribute), true) as OutjectAttribute;
            }
            return (attribute != null);
        }

        /// <summary>
        /// Gets the <see cref="OutjectAttribute"/> on properties.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <returns>The attribute</returns>
        public static OutjectAttribute GetOutjectAttribute(MemberInfo memberInfo)
        {
            OutjectAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(OutjectAttribute)) as OutjectAttribute;
            if (attribute == null)
            {
                attribute = Attribute.GetCustomAttribute(memberInfo, typeof(OutjectAttribute), true) as OutjectAttribute;
            }
            return attribute;
        }
        
        /// <summary>
        /// Determines whether the specified class type has the <see cref="ScopeAttribute"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified typz has <see cref="ScopeAttribute"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasScopeAttribute(Type type)
        {
            ScopeAttribute attribute = Attribute.GetCustomAttribute(type, typeof(ScopeAttribute)) as ScopeAttribute;
            if (attribute == null)
            {
                attribute = Attribute.GetCustomAttribute(type, typeof(ScopeAttribute), true) as ScopeAttribute;
            }
            return (attribute!=null);
        }
        
        /// <summary>
        /// Gets the <see cref="ScopeAttribute"/> on class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ScopeAttribute GetScopeAttribute(Type type)
        {
            ScopeAttribute attribute = Attribute.GetCustomAttribute(type, typeof(ScopeAttribute)) as ScopeAttribute;
            if (attribute == null)
            {
                attribute = Attribute.GetCustomAttribute(type, typeof(ScopeAttribute), true) as ScopeAttribute;
            }
            return attribute;
        }
    }
}
