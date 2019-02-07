﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverContracts : MonoBehaviour
{
    public void OnButtonPress()
    {
        foreach(Contract c in ContractManager.Instance.currentContracts)
        {
            if(c.colectedPersons == c.personsToCollect)
            {
                CreditSystem.Instance.credits += c.contractReward;
                for(int i = 0; i < ContractManager.Instance.portUI.portrets.Length; i++)
                {
                    ContractManager.Instance.portUI.portrets[i].sprite = ContractManager.Instance.portrets[3];
                }
                Destroy(c);
            }
        }
    }
}