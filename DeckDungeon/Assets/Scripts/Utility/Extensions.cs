using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = UnityEngine.Object;

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static T GetRandomElement<T>(this IList<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}

public static class DDCardBaseExtensions
{

}

public static class DDEnemyBasextensions
{
    public static DDEnemyBase Clone(this DDEnemyBase scriptableObject)
    {
        if (!scriptableObject)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(DDEnemyBase)} object.");
            return (DDEnemyBase)ScriptableObject.CreateInstance(typeof(DDEnemyBase));
        }

        DDEnemyBase instance = Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name;
        return instance;
    }
}

public static class EnumExtensions
{
    public static int GetLayer(this ETargetType enumValue)
    {
        return 1 << (int)enumValue;
    }
}