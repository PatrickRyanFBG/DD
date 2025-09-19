using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDParticleLibrary : ScriptableObject
{
    [SerializeField] private List<DDParticleBase> particles;

    private Dictionary<Type, DDParticleBase> particleDict = null;

    public T GetParticle<T>() where T : DDParticleBase
    {
        if (particleDict == null)
        {
            particleDict = new Dictionary<Type, DDParticleBase>();

            foreach (var p in particles)
            {
                particleDict.Add(p.GetType(), p);
            }
        }
        
        if(particleDict.TryGetValue(typeof(T), out var part))
        {
            T spawnedParticle = Instantiate(part) as T;
            return spawnedParticle;
        }
        
        return null;
    }
}
