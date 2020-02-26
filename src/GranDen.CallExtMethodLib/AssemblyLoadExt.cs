using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GranDen.CallExtMethodLib
{
    /// <summary>
    /// Library for loading assembly and probing extension methods.
    /// </summary>
    public static class AssemblyLoadExt
    {
        /// <summary>
        /// Load Assembly by given partial name
        /// </summary>
        /// <param name="partialName">The assembly name without version &amp; public token part.</param>
        /// <returns></returns>
        public static Assembly GetLoadedAssembly(this string partialName)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var target = loadedAssemblies.FirstOrDefault(_ => _.GetName().Name.Equals(partialName));
            if (target != null) { return target; }

            var mainAssembly = Assembly.GetEntryAssembly();
            if (mainAssembly == null) { return null;}

            var assemblyName = mainAssembly.GetReferencedAssemblies().FirstOrDefault(_ => _.Name.Equals(partialName));

            return assemblyName == null ? null : Assembly.Load(assemblyName);
        }

        /// <summary>
        /// Find extension method from assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the extension method implementation.</param>
        /// <param name="extMethodInfo">The object that implements <see cref="IExtMethodInfo"/>.</param>
        /// <returns>The candidates of <see cref="MemberInfo"/> objects.</returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, IExtMethodInfo extMethodInfo)
        {
            return GetExtensionMethods(assembly, extMethodInfo.ExtendedType, extMethodInfo.MethodName);
        }

        /// <summary>
        /// Find extension method from assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the extension method implementation</param>
        /// <param name="extendedType">The type of the first parameter in Extension method declaration.</param>
        /// <param name="extensionMethodName">Extension method name.</param>
        /// <returns>The candidates of <see cref="MemberInfo"/> objects.</returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, Type extendedType, string extensionMethodName)
        {
            var extMethodInfos =
                from type in assembly.GetTypes()
                where type.IsSealed && !type.IsGenericType && !type.IsNested
                from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where method.IsDefined(typeof(ExtensionAttribute), false)
                where method.Name == extensionMethodName
                where method.GetParameters()[0].ParameterType == extendedType
                select method;

            return extMethodInfos;
        }
    }
}
