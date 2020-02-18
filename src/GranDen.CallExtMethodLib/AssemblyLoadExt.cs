using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GranDen.CallExtMethodLib
{
    public static class AssemblyLoadExt
    {
        /// <summary>
        /// Load Assembly by given partial name
        /// </summary>
        /// <param name="partialName"></param>
        /// <returns></returns>
        public static Assembly GetLoadedAssembly(this string partialName)
        {
            var loadedAssembly =
                AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(c => c.FullName.Contains(partialName));

            if (loadedAssembly != null) { return loadedAssembly; }

            var assemblyName = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                .FirstOrDefault(c => c.FullName.Contains(partialName));

            return assemblyName == null ? null : Assembly.Load(assemblyName);
        }

        /// <summary>
        /// Find extension method from assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="extendedType">The type of the first parameter in Extension method declaration.</param>
        /// <param name="extensionMethodName"></param>
        /// <returns></returns>
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