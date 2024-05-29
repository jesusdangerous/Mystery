using System;
using System.Collections.Generic;
using System.Linq;

namespace MysteryWorld.Models;

public static class ListModel
{
    public static void ForEachReversed<T>(this List<T> list, Action<T> action)
    {
        for (var i = list.Count - 1; i >= 0; i--)
            action(list[i]);
    }

    public static void ForEachReversed<T>(this ICollection<T> list, Action<T> action)
    {
        list.ToList().ForEachReversed(action);
    }
}