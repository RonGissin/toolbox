using System.Runtime.CompilerServices;

namespace Safety
{
    public class Safety
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AssertNotNull<T>(T argument, [CallerArgumentExpression("argument")] string argumentName = null)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

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
