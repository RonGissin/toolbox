using System.Runtime.CompilerServices;

namespace ToolBox.Safety
{
    public static class Safe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>(T argument, [CallerArgumentExpression("argument")] string argumentName = null)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return argument;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ThrowIfNullOrEmpty(string argument, [CallerArgumentExpression("argument")] string argumentName = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException($"{argumentName} cannot be null or empty.");
            }

            return argument;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ThrowIfNullOrEmptyEnumerable<T>(IEnumerable<T> argument, [CallerArgumentExpression("argument")] string argumentName = null)
            where T : class
        {
            if (argument == null || !argument.Any())
            {
                throw new ArgumentNullException($"{argumentName} cannot be null or empty enumerable.");
            }

            return argument;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ThrowIfEmptyGuid(Guid argument, [CallerArgumentExpression("argument")] string argumentName = null)
        {
            if (argument == Guid.Empty)
            {
                throw new ArgumentException($"{argumentName} cannot be the empty guid.");
            }

            return argument;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfAboveUpperBound<T>(T argument, T upperBound, [CallerArgumentExpression("argument")] string argumentName = null)
            where T : IComparable<T>
        {
            if (argument.CompareTo(upperBound) > 0)
            {
                throw new ArgumentException($"{argumentName} must be less than or equal to the upper bound {upperBound}");
            }

            return argument;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfBelowLowerBound<T>(T argument, T lowerBound, [CallerArgumentExpression("argument")] string argumentName = null)
            where T : IComparable<T>
        {
            if (argument.CompareTo(lowerBound) < 0)
            {
                throw new ArgumentException($"{argumentName} must be bigger than or equal to the lower bound {lowerBound}");
            }

            return argument;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNotBetween<T>(T argument, T lowerBound, T upperBound, [CallerArgumentExpression("argument")] string argumentName = null)
            where T : IComparable<T>
        {
            ThrowIfBelowLowerBound(argument, lowerBound);
            ThrowIfAboveUpperBound(argument, upperBound);

            return argument;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf(bool shouldThrow, string message)
        {
            if (shouldThrow)
            {
                throw new ArgumentException(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf(bool shouldThrow, string paramName, string message)
        {
            if (shouldThrow)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertIsAssignableFrom<T>(Type type, [CallerArgumentExpression("type")] string paramName = null)
            where T : class
        {
            if (type != null
                && !(type.IsSubclassOf(typeof(T)) || type.IsAssignableFrom(typeof(T))))
            {
                throw new ArgumentException($"The type {type.Name} must derive from {nameof(T)}", paramName);
            }
        }
    }
}
