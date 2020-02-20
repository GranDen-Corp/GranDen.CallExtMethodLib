using System;
using System.Linq;
using System.Reflection;

namespace GranDen.CallExtMethodLib
{
    /// <summary>
    /// Invocation handler of extension method.
    /// </summary>
    public class ExtMethodInvoker
    {
        /// <summary>
        /// Assembly that specified in constructor parameter.
        /// </summary>
        public Assembly ExtensionLibAssembly { get; }

        /// <summary>
        /// Create an invoker for calling extension method.
        /// </summary>
        /// <param name="partialAssemblyName"></param>
        public ExtMethodInvoker(string partialAssemblyName)
        {
            ExtensionLibAssembly = partialAssemblyName.GetLoadedAssembly();
            if (ExtensionLibAssembly == null)
            {
                throw new TypeLoadException($"Cannot find assembly that has partial name {{{partialAssemblyName}}}");
            }
        }

        /// <summary>
        /// Invoke an Extension method that is void.
        /// </summary>
        /// <param name="extMethodInfo"></param>
        /// <param name="extMethodParams"></param>
        public void Invoke(IExtMethodInfo extMethodInfo, params object[] extMethodParams)
        {
            if (string.IsNullOrWhiteSpace(extMethodInfo.MethodName))
            {
                throw new ArgumentException($"{nameof(extMethodInfo.MethodName)} is empty");
            }
            if (extMethodParams.Length < 1)
            {
                throw new TargetParameterCountException($"Invoke Extension method {extMethodInfo.MethodName}() must provide at least the extended type parameter!");
            }

            if (extMethodInfo.ExtendedType == null)
            {
                Invoke(extMethodInfo.MethodName, extMethodParams);
                return;
            }

            var targetExtMethods = ExtensionLibAssembly.GetExtensionMethods(extMethodInfo);
            var methodInfos = targetExtMethods as MethodInfo[] ?? targetExtMethods.ToArray();
            if (!methodInfos.Any())
            {
                throw new MissingMethodException(extMethodInfo.MethodName);
            }

            methodInfos.First().Invoke(null, extMethodParams);
        }

        /// <summary>
        /// Invoke an Extension method that is void, and its 1st parameter is Class or Struct type object.
        /// </summary>
        /// <param name="methodName">The extension method name.</param>
        /// <param name="extMethodParams">Parameters that needs to invoke the extension method.</param>
        public void Invoke(string methodName, params object[] extMethodParams)
        {
            if (extMethodParams.Length < 1)
            {
                throw new TargetParameterCountException($"Invoke Extension method {methodName}() must provide at least the extended type parameter!");
            }

            var targetExtMethods = ExtensionLibAssembly.GetExtensionMethods(extMethodParams.First().GetType(), methodName);
            var methodInfos = targetExtMethods as MethodInfo[] ?? targetExtMethods.ToArray();
            if (!methodInfos.Any())
            {
                throw new MissingMethodException(methodName);
            }

            Exception notMatchRunEx = null;
            foreach (var methodInfo in methodInfos.Where(x => x.GetParameters().Length == extMethodParams.Length))
            {
                try
                {
                    methodInfo.Invoke(null, extMethodParams);
                    notMatchRunEx = null;
                    break;
                }
                catch (ArgumentException ex)
                {
                    notMatchRunEx = ex;
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }

                    throw;
                }
            }

            if (notMatchRunEx != null)
            {
                throw notMatchRunEx;
            }
        }

        /// <summary>
        /// Invoke an Extension method that has return type.
        /// </summary>
        /// <typeparam name="TReturn">The return type of extension method.</typeparam>
        /// <param name="extMethodInfo">The <see cref="IExtMethodInfo"/> object contains target extension method information.</param>
        /// <param name="extMethodParams">Parameters that needs to invoke the extension method.</param>
        /// <returns></returns>
        public TReturn Invoke<TReturn>(IExtMethodInfo extMethodInfo, params object[] extMethodParams)
        {
            if (string.IsNullOrWhiteSpace(extMethodInfo.MethodName))
            {
                throw new ArgumentException($"{nameof(extMethodInfo.MethodName)} is empty");
            }
            if (extMethodParams.Length < 1)
            {
                throw new TargetParameterCountException($"Invoke Extension method {extMethodInfo.MethodName}() must provide at least the extended type parameter!");
            }

            if (extMethodInfo.ExtendedType == null)
            {
                return Invoke<TReturn>(extMethodInfo.MethodName, extMethodParams);
            }

            var targetExtMethods = ExtensionLibAssembly.GetExtensionMethods(extMethodInfo);
            var methodInfos = targetExtMethods as MethodInfo[] ?? targetExtMethods.ToArray();
            if (!methodInfos.Any())
            {
                throw new MissingMethodException(extMethodInfo.MethodName);
            }

            object invokeResult = null;

            Exception notMatchRunEx = null;
            foreach (var methodInfo in methodInfos.Where(x => x.GetParameters().Length == extMethodParams.Length))
            {
                try
                {
                    invokeResult = methodInfo.Invoke(null, extMethodParams);
                    notMatchRunEx = null;
                    break;
                }
                catch (ArgumentException ex)
                {
                    notMatchRunEx = ex;
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }

                    throw;
                }
            }

            if (notMatchRunEx != null)
            {
                throw notMatchRunEx;
            }

            if (invokeResult == null)
            {
                return default;
            }

            return (TReturn)invokeResult;
        }

        /// <summary>
        /// Invoke an Extension method that has return type, and its 1st parameter is Class or Struct type object.
        /// </summary>
        /// <typeparam name="TReturn">The return type of extension method.</typeparam>
        /// <param name="methodName">The extension method name.</param>
        /// <param name="extMethodParams">Parameters that needs to invoke the extension method.</param>
        /// <returns></returns>
        public TReturn Invoke<TReturn>(string methodName, params object[] extMethodParams)
        {
            if (extMethodParams.Length < 1)
            {
                throw new TargetParameterCountException($"Invoke Extension method {methodName}() must provid at least the extended type parameter!");
            }

            var targetExtMethod = ExtensionLibAssembly.GetExtensionMethods(extMethodParams.First().GetType(), methodName).FirstOrDefault();

            if (targetExtMethod == null)
            {
                throw new MissingMethodException(methodName);
            }
            object invokeResult;
            try
            {
                invokeResult = targetExtMethod.Invoke(null, extMethodParams);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }

            if (invokeResult == null)
            {
                return default;
            }

            return (TReturn)invokeResult;
        }
    }
}
