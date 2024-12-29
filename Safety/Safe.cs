using System.Runtime.CompilerServices;

namespace ToolBox.Safety
{
    public static class Safe
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the argument is null.
        /// </summary>
        /// <typeparam name="T">The type of the argument to validate.</typeparam>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="argumentName">The name of the argument to validate (optional)</param>
        /// <returns>The validated argument.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument is null.</exception>
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

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the argument is null or empty.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="argumentName">The name of the argument to validate (optional)</param>
        /// <returns>The validated argument.</returns>
        /// <exception cref="ArgumentException">Thrown if the argument is null or empty.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ThrowIfNullOrEmpty(string argument, [CallerArgumentExpression("argument")] string argumentName = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentException($"{argumentName} cannot be null or empty.");
            }

            return argument;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the argument is null or an empty enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable's items.</typeparam>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="argumentName">The name of the argument to validate (optional)</param>
        /// <returns>c</returns>
        /// <exception cref="ArgumentNullException">Thrown if the argument is null or an empty enumerable.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ThrowIfNullOrEmptyEnumerable<T>(IEnumerable<T> argument, [CallerArgumentExpression("argument")] string argumentName = null)
            where T : class
        {
            if (argument == null || !argument.Any())
            {
                throw new ArgumentException($"{argumentName} cannot be null or empty enumerable.");
            }

            return argument;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the argument is the empty guid.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="argumentName">The name of the argument to validate (optional)</param>
        /// <returns>The name of the argument to validate (optional)</returns>
        /// <exception cref="ArgumentException">Thrown if the argument is the empty guid.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ThrowIfEmptyGuid(Guid argument, [CallerArgumentExpression("argument")] string argumentName = null)
        {
            if (argument == Guid.Empty)
            {
                throw new ArgumentException($"{argumentName} cannot be the empty guid.");
            }

            return argument;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the argument is above the provided upper bound.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="argumentName">The name of the argument to validate (optional)</param>
        /// <returns>The name of the argument to validate (optional)</returns>
        /// <exception cref="ArgumentException">Thrown if the argument is above the upper bound.</exception>
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

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the argument is below the provided lower bound.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="argumentName">The name of the argument to validate (optional)</param>
        /// <returns>The name of the argument to validate (optional)</returns>
        /// <exception cref="ArgumentException">Thrown if the argument is below the lower bound.</exception>
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

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the argument is not between the provided bounds.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="argumentName">The name of the argument to validate (optional)</param>
        /// <returns>The name of the argument to validate (optional)</returns>
        /// <exception cref="ArgumentException">Thrown if the argument is not between the provided bounds.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNotBetween<T>(T argument, T lowerBound, T upperBound, [CallerArgumentExpression("argument")] string argumentName = null)
            where T : IComparable<T>
        {
            ThrowIfBelowLowerBound(argument, lowerBound);
            ThrowIfAboveUpperBound(argument, upperBound);

            return argument;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the provided condition is true.
        /// </summary>
        /// <param name="shouldThrow">A boolean indicating whether to throw the exception.</param>
        /// <param name="message">The message with which to throw the exception.</param>
        /// <param name="paramName">The param name with which to enrich the exception (optional).</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="shouldThrow"/> evaluates to true.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf(bool shouldThrow, string message, string paramName = null)
        {
            if (shouldThrow)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the provided <paramref name="type"/> is not assignable from <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <param name="paramName">The name of the type parameter to validate.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="type"/> is not assignable from <typeparamref name="T"/>.</exception>
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
