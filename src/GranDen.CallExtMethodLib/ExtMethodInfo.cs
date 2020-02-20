using System;

namespace GranDen.CallExtMethodLib
{
    /// <summary>
    /// Extension method information interface.
    /// </summary>
    public interface IExtMethodInfo
    {
        /// <summary>
        /// Extension method name.
        /// </summary>
        string MethodName { get; }
        /// <summary>
        /// The type of 1st parameter of extension method.
        /// </summary>
        Type ExtendedType { get; }
    }

    /// <summary>
    /// The default implementation of <see cref="IExtMethodInfo"/>.
    /// </summary>
    public class ExtMethodInfo : IExtMethodInfo
    {
        /// <inheritdoc/>
        public string MethodName { get; set; }
        /// <inheritdoc/>
        public Type ExtendedType { get; set; } = null;
    }
}
