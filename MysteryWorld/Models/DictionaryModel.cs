using System.Collections.Generic;
using System.Linq;

namespace MysteryWorld.Models;

public static class DictionaryModel
{
    public static Dictionary<TK, TV> Copy<TK, TV>(this Dictionary<TK, TV> dict)
    {
        return dict.ToDictionary(entry => entry.Key,
            entry => entry.Value);
    }
}