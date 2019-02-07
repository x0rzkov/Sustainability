﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterContractMenu : MonoBehaviour
{
    public GameObject uiButton;
    public GameObject contractsUi;
    public GameObject activeContracts;
    private bool canOpen;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenContracts();
        }
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("City")) {
            canOpen = true;
            uiButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("City")) {
            uiButton.SetActive(false);
            canOpen = false;
        }
    }

    public void OpenContracts() {
        if (canOpen) {
            if (uiButton.activeSelf)
            {
                UpdateUIPositions();
                uiButton.SetActive(false);
                contractsUi.SetActive(true);
                activeContracts.SetActive(false);
            }
            else
            {
                uiButton.SetActive(true);
                contractsUi.SetActive(false);
                activeContracts.SetActive(false);
            }
        }
        else
        {
            if (activeContracts.activeSelf)
            {
                activeContracts.SetActive(false);
            }
            else
            {
                activeContracts.SetActive(true);
            }
        }
    }

    private void UpdateUIPositions()
    {
        int i = 0;
        if (canOpen)
        {
            foreach (Contract g in ContractManager.Instance.existingContracts)
            {
                g.selfInAvailableContractScreen.transform.position = new Vector3(g.selfInAvailableContractScreen.transform.position.x, 700, g.selfInAvailableContractScreen.transform.position.z);
                g.selfInAvailableContractScreen.transform.Translate(new Vector3(0, -((i++) * 90), 0));
            }
            i = 0;
            foreach (Contract a in ContractManager.Instance.currentContracts)
            {
                a.selfInAvailableContractScreen.transform.position = new Vector3(a.selfInAvailableContractScreen.transform.position.x, 700, a.selfInAvailableContractScreen.transform.position.z);
                a.selfInAvailableContractScreen.transform.Translate(new Vector3(0, -((i++) * 90), 0));
            }
        }
        else
        {
            i = 0;
            foreach (Contract a in ContractManager.Instance.currentContracts)
            {
                a.selfInActiveContractScreen.transform.position = new Vector3(a.selfInActiveContractScreen.transform.position.x, 700, a.selfInActiveContractScreen.transform.position.z);
                a.selfInActiveContractScreen.transform.Translate(new Vector3(0, -((i++) * 90), 0));
            }
        }
    }
}
