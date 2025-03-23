using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RandomHelpers
{
    public static int GetPositiveNegative(int value)
    {
        return value * (Random.Range(0, 2) == 1 ? 1 : -1);
    }

    public static bool GetRandomBool()
    {
        return Random.Range(0, 2) == 1;
    }

    public static bool GetRandomBool(float percent)
    {
        return Random.Range(0f, 100f) < percent;
    }
}

public static class IEnumeratorHelper {

    public delegate IEnumerator EventHandler(MonoBehaviour sender, EventArgs args);

    public static IEnumerator Occured
        (this EventHandler e, MonoBehaviour sender, EventArgs args = null)
    {
        if (e == null) yield break;
        var args_obj = args ?? EventArgs.Empty;
        foreach (var handler in e.GetInvocationList().Cast<EventHandler>())
            yield return handler(sender, args_obj);
    }
}
