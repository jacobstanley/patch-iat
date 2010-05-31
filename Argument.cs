using System;

namespace PatchIat
{
    internal static class Argument
    {
        public static string NotEmpty(this string argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (argument.Length == 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, "argument cannot be empty");
            }

            return argument;
        }

        public static T NotNull<T>(this T argument, string argumentName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return argument;
        }
    }
}