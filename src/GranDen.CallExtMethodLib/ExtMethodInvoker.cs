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
        private readonly Assembly _extensionLibAssembly;

        public ExtMethodInvoker(string partialAssemblyName)
        {
            _extensionLibAssembly = partialAssemblyName.GetLoadedAssembly();
            if (_extensionLibAssembly == null)
            {
                throw new TypeLoadException($"Cannot find assembly that has partial name {{{partialAssemblyName}}}");
            }
        }

        /// <summary>
        /// Invoke an Extension method that is void.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="extMethodParams"></param>
        public void Invoke(string methodName, params object[] extMethodParams)
        {
            if (extMethodParams.Length < 1)
            {
                throw new TargetParameterCountException($"Invoke Extension method {methodName}() must provid at least the extended type parameter!");
            }

            var targetExtMethod = _extensionLibAssembly.GetExtensionMethods(extMethodParams.First().GetType(), methodName).FirstOrDefault();

            if (targetExtMethod == null)
            {
                throw new MissingMethodException(methodName);
            }

            targetExtMethod.Invoke(null, extMethodParams);
        }

        /// <summary>
        /// Invoke an Extension method that has return type.
        /// </summary>
        /// <typeparam name="TReturn">The return type of extension method.</typeparam>
        /// <param name="methodName"></param>
        /// <param name="extMethodParams"></param>
        /// <returns></returns>
        public TReturn Invoke<TReturn>(string methodName, params object[] extMethodParams)
        {
            if (extMethodParams.Length < 1)
            {
                throw new TargetParameterCountException($"Invoke Extension method {methodName}() must provid at least the extended type parameter!");
            }

            var targetExtMethod = _extensionLibAssembly.GetExtensionMethods(extMethodParams.First().GetType(), methodName).FirstOrDefault();

            if (targetExtMethod == null)
            {
                throw new MissingMethodException(methodName);
            }

            var invokeResult = targetExtMethod.Invoke(null, extMethodParams);

            if(invokeResult == null)
            {
                return default;
            }

            return (TReturn)invokeResult;
        }
    }
}
