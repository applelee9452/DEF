using System;
using System.Collections.Generic;

namespace DEF;

public static class ListExtensions
{
    // Shuffle algorithm as seen on page 32 in the book "Algorithms" (4th edition) by Robert Sedgewick
    // <param name="source">Collection to shuffle.</param>
    // <typeparam name="T">The generic type parameter of the collection.</typeparam>
    // <returns>The shuffled collection as IEnumerable.</returns>
    public static List<T> ShuffleRef<T>(this List<T> source, Random rd)
    {
        var array = source;

        var n = array.Count;
        for (var i = 0; i < n; i++)
        {
            var r = i + rd.Next(0, n - i);// Exchange a[i] with random element in a[i..n-1]                

            (array[r], array[i]) = (array[i], array[r]);
        }

        return array;
    }
}