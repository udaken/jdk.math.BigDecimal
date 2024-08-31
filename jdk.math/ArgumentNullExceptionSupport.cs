using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace jdk.math {
    internal static class ArgumentNullExceptionSupport {
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
            if (argument is null) {
                Throw(paramName);
            }

            [DoesNotReturn]
            static void Throw(string? name) {
                throw new ArgumentNullException(name);
            }
        }
    }
}
#if !NETCOREAPP3_0_OR_GREATER
namespace System.Runtime.CompilerServices {
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute {
        public CallerArgumentExpressionAttribute(string parameterName) {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}
#endif