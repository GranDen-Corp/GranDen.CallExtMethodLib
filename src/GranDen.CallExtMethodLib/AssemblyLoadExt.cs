using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

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
        /// <param name="assemblyPartialName">The assembly name without version &amp; public token part.</param>
        /// <returns></returns>
        public static Assembly GetLoadedAssembly(this string assemblyPartialName)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var target = loadedAssemblies.FirstOrDefault(_ => _.GetName().Name.Equals(assemblyPartialName));
            if (target != null) { return target; }

            foreach (var assembly in loadedAssemblies)
            {
                var refAssembly = assembly.GetReferencedAssemblies().FirstOrDefault(_ => _.Name.Equals(assemblyPartialName));
                if (refAssembly != null)
                {
                    var ret = Assembly.Load(refAssembly);
                    return ret;
                }
            }

            var mainAssembly = Assembly.GetEntryAssembly();
            if (mainAssembly == null) { return null; }

            var assemblyName = mainAssembly.GetReferencedAssemblies().FirstOrDefault(_ => _.Name.Equals(assemblyPartialName));
            if (assemblyName != null)
            {
                return Assembly.Load(assemblyName);
            }

            //Try to probe assembly from current project's dep.json file records
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var runtimeLibrary in dependencies)
            {
                if(runtimeLibrary.Name.Equals(assemblyPartialName))
                {
                    return Assembly.Load(new AssemblyName(runtimeLibrary.Name));
                }

                if (runtimeLibrary.RuntimeAssemblyGroups.Any(runtimeAssemblyGroup => 
                    runtimeAssemblyGroup.RuntimeFiles.Any(runtimeFile =>
                        runtimeFile.Path.EndsWith($"{assemblyPartialName}.dll"))))
                {
                    return Assembly.Load(new AssemblyName(assemblyPartialName));
                }
            }

            //Force manually load assembly from current exeucting directory
            return ForceLoadAssembly(assemblyPartialName);
        }

        

        private static Assembly ForceLoadAssembly(string assemblyPartialName)
        {
            var executeDirectoryInfo = GetExecutingDirectory();
            var candidateAssemblyPath = new[] {
                $"{executeDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{assemblyPartialName}.dll",
                $"{executeDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{assemblyPartialName}.exe",
                $"{executeDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{assemblyPartialName}"
            };
            Assembly loadedAssembly = null;
            Exception loadException = null;
            foreach (var assemblyPath in candidateAssemblyPath)
            {
                try
                {
                    loadedAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                    loadException = null;
                    break;
                }
                catch (Exception ex)
                {
                    loadException = ex;
                } 
            }

            if(loadException != null) { throw loadException; }
            return loadedAssembly;            
        }

        private static DirectoryInfo GetExecutingDirectory()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory;
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
