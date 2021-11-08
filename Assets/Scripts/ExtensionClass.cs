using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static class ExtensionClass
{
    public static void ForEach<T>(this IEnumerable<T> sequence,Action<T> action)
    {
        foreach (var item in sequence)
        {
            action(item);
        }
    }


    public static IEnumerable<T> NonNull<T>(this IEnumerable<T> sequence)
    {
        return sequence.Where(element => element != null);
    }

    public static IEnumerable<U> ConvertType<U>(this IEnumerable sequence) where U:class
    {
        List<U> ans = new List<U>();

        foreach (var element in sequence)
        {
            ans.Add(element as U);
        }
        return ans;
    }

    public static IEnumerable<U> ConvertAll<T,U>(this IEnumerable<T> sequence,Func<T,U> func) 
    {
        List<U> ans = new List<U>();
        foreach (var item in sequence)
        {
            ans.Add(func(item));
        }
        return ans;
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> sequence)
    {
        return sequence.OrderBy(i => Guid.NewGuid()).ToList();
    }

}
