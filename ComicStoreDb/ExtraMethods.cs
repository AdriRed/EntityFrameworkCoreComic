using System;

namespace ComicStoreDb
{
    internal static class Methods
    {
        public static int[] ToInt(this string[] target)
        {
            int[] result = new int[target.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Int32.Parse(target[i]);
            }
            return result;
        }

        /// <summary>
        /// Converts an int array to DateTime.
        /// </summary>
        /// <param name="target">This array must be in that format: {Day, Month, Year}</param>
        /// <returns></returns>
        public static DateTime ToDate(this int[] target)
        {
            return new DateTime(target[2], target[1], target[0]);
        }

        public static string[] AddWhitespaces(this string[] target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                for (int j = 0; j < target[i].Length - 1; j++)
                {
                    if (Char.IsLower(target[i][j]) && Char.IsUpper(target[i][j + 1]))
                    {
                        target[i] = target[i].Substring(0, j + 1) + " " + target[i].Substring(j + 1);
                    }
                }
            }

            return target;
        }
    }
}