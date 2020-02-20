using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GranDen.CallExtMethodLib
{
    /// <summary>
    /// 
    /// </summary>
    public static class CreateDelegateHelper
    {
        /// <summary>
        /// Create a simple Action&lt;T&gt; Lambda Delegate that only assign input variable's properties.
        /// </summary>
        /// <param name="inputType">Input variable type</param>
        /// <param name="inputVariableName">input variable name</param>
        /// <param name="assignDictionary">The PropertyName : PropertyValue key-value dictionary use for assign action input variable.</param>
        /// <returns>The Action&lt;T&gt; delegate.</returns>
        public static Delegate CreateAssignValueAction(Type inputType, string inputVariableName, IDictionary<string, object> assignDictionary)
        {
            var pe = Expression.Parameter(inputType, inputVariableName);

            var assignExpressions = new List<BinaryExpression>();
            foreach (var keyValuePair in assignDictionary)
            {
                var memberExpression = Expression.Property(pe, keyValuePair.Key);
                var constant = Expression.Constant(keyValuePair.Value);
                var assign = Expression.Assign(memberExpression, constant);
                assignExpressions.Add(assign);
            }
            var body = Expression.Block(assignExpressions);

            var retType = typeof(Action<>).MakeGenericType(inputType);
            var lambdaExpression = Expression.Lambda(retType, body, pe);

            return lambdaExpression.Compile();
        }
    }
}
