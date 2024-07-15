using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDPlayerLaneArmorUI : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI amountText;

    public void SetAmount(int amount)
    {
        if(amount <= 0)
        {
            if(gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if(!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            amountText.text = amount.ToString();
        }
    }
}
