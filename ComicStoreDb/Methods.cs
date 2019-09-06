using System;
using System.Collections.Generic;
using System.Text;

namespace ComicStoreDb
{
    static class Methods
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

    }
}
