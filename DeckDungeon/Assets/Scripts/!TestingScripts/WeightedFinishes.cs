using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedFinishes : MonoBehaviour
{
    [SerializeField] private float cycles = 100;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dictionary<DDPlayerCardFinish, float> counts = new();

            for (int i = 0; i < cycles; i++)
            {
                DDPlayerCardFinish finish =
                    DDGlobalManager.Instance.CardFinishLibrary.GetRandomFinishByImpact(EPlayerCardFinishImpact
                        .Positive);

                if (counts.ContainsKey(finish))
                {
                    counts[finish]++;
                }
                else
                {
                    counts.Add(finish, 1);
                }
            }

            string output = "";

            float totalWeight =
                DDGlobalManager.Instance.CardFinishLibrary.GetTotalWeightsByImpact(EPlayerCardFinishImpact.Positive);
            
            foreach (KeyValuePair<DDPlayerCardFinish, float> kvp in counts)
            {
                output += kvp.Key.PlayerCardFinish + ": " + kvp.Value + " - " + (kvp.Key.Weight / totalWeight) + "/" + (kvp.Value / cycles) + "\n";
            }
            
            Debug.Log(output);
        }
    }
}
