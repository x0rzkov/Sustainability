﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private CanvasColors c;
    public Color ownedColor;
    public GameObject openStorePromt;
    public GameObject activeContracts;
    public GameObject deliverContract;
    private List<GameObject> openMenu = new List<GameObject>();
    private int itemSelected = 0;
    private int storeNumber;
    private Store store;
    public PreviewModel previewModel;
    public GameObject[] storeFrames;
    private int reFuelPrice;

    public delegate void RefuelShip();
    public static event RefuelShip OnRefuelShip;

    public delegate void BuyItemEvent(GameObject item, bool isShipStore);
    public static event BuyItemEvent OnItemBuy;

    private void Start()
    {
        InitListners();
        ContractManager.Instance.InitNewContracts();
        
    }

    private void InitListners() {
        Ship.OnEnterCity += EnterCity;
        Ship.OnExitCity += ExitCity;
    }

    private void EnterCity(Store s) {
        Ship.Instance.gameObject.GetComponent<PreviewModel>().shipPreviews[itemSelected].SetActive(true);
        store = s; 
        openStorePromt.SetActive(true);
        c = this.gameObject.GetComponent<CanvasColors>();
        
        storeNumber = store.storeNumber;
        c.clerk.sprite = store.clerk;
        c.clerkGlow.sprite = store.clerkGlow;
        c.storeSymbol.sprite = store.storeSymbol;
        c.storeSlogan.text = store.storeSlogan;
        c.storeName.text = store.storeName;
        c.unitFuelPrice.text = "Per Unit: " + store.fuelCostPerUnit.ToString() + ",-";

        for (int i = 0; i < c.textColor.Length; i++) {
            c.textColor[i].color = store.storeColor;
        }
        for (int i = 0; i < c.imageColor.Length; i++) {
            c.imageColor[i].color = store.storeColor;
        }
        UpdateStore();
    }

    private void UpdateStore() {
        previewModel = PreviewModel.Instance;
        float cost = Ship.Instance.maxFuel - Ship.Instance.currentFuel;
        reFuelPrice = Mathf.RoundToInt((store.fuelCostPerUnit * cost));
        c.fuelCost.text = "Total: " + reFuelPrice.ToString()+",-";
        for (int i = 0; i < storeFrames.Length; i++) {
            storeFrames[i].SetActive(false);
        }

        for (int i = 0; i < previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].buyableParts.Count; i++) {
            storeFrames[i].SetActive(true);
            if (previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].bought[i]) {
                storeFrames[i].GetComponent<Image>().color = ownedColor;
                storeFrames[i].GetComponent<Button>().enabled = false;
            }
            else {
                storeFrames[i].GetComponent<Button>().enabled = true;
            }
        }

        int z = 0;
        foreach (int x in previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].price) {
            if (!previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].bought[z]) {
                c.storePrices[z].text = x.ToString() + ",-";
            }
            else {
                c.storePrices[z].text = "Owned";
            }
            z++;
        }
    }

    private void ExitCity() {
        ContractManager.Instance.InitNewContracts();
        openStorePromt.SetActive(false);
        foreach (GameObject x in openMenu) {
            x.SetActive(false);
        }
        openMenu.Clear();
    }

    public void ExitMenu(GameObject gameObjectToExit) {
        gameObjectToExit.SetActive(false);
        if (openMenu.Contains(gameObjectToExit)) {
            openMenu.Remove(gameObjectToExit);
        }
    }

    public void OpenMenu(GameObject gameObjectToOpen) {
        gameObjectToOpen.SetActive(true);
        openMenu.Add(gameObjectToOpen);
        ContractManager.Instance.UpdateUIPositionsBigMenu();
    }

    public void FuelShip() {
        if (CreditSystem.Instance.credits > CreditSystem.Instance.fuelCost) {
            CreditSystem.Instance.credits -= reFuelPrice;
            OnRefuelShip?.Invoke();
            UpdateStore();
        }
    }

    public void UpdateUIPositionsSmallMenu() {
        int i = 0;
        foreach (Contract a in ContractManager.Instance.currentContracts) {
            a.selfInActiveContractScreen.transform.position = new Vector3(a.selfInActiveContractScreen.transform.position.x, 700, a.selfInActiveContractScreen.transform.position.z);
            a.selfInActiveContractScreen.transform.Translate(new Vector3(0, -((i++) * 90), 0));
            a.selfProgressUI.collectedPeople.text = a.colectedPersons.ToString();
        }
    }

    public void SelectStoreItem(int itemNumber) {
        itemSelected = itemNumber;
        if (store.isShipStore) {
            foreach(GameObject x in Ship.Instance.gameObject.GetComponent<PreviewModel>().shipPreviews) {
                x.SetActive(false);
            }
            Ship.Instance.gameObject.GetComponent<PreviewModel>().shipPreviews[itemSelected].SetActive(true);
        }
        else {
            //if selevted object is on, disable it, else turn it on.
            if (!previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].buyableParts[itemSelected].activeSelf) {
                storeFrames[itemSelected].GetComponent<Image>().color = new Color(255, 0, 0);
                previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].buyableParts[itemSelected].SetActive(true);
            }
            else {
                storeFrames[itemSelected].GetComponent<Image>().color = store.storeColor;
                previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].buyableParts[itemSelected].SetActive(false);
            }

            //disable all other objects.
            for (int i = 0; i < PreviewModel.Instance.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].buyableParts.Count; i++) {
                if (i != itemSelected && !previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].bought[i]) {
                    storeFrames[i].GetComponent<Image>().color = store.storeColor;
                    previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].buyableParts[i].SetActive(false);
                }
            }
        }
    }

    public void BuyItem() {
        if (!previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].bought[itemSelected]) {
            if (store.isShipStore) {
                Ship.Instance.currentShipMesh.SetActive(false);
                OnItemBuy(previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].buyableParts[itemSelected], true);
                previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].bought[itemSelected] = true;
            }
            else {
                OnItemBuy(previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].buyableParts[itemSelected], false);
                previewModel.shipPreviews[Ship.Instance.currentShip].GetComponent<ShipParts>().stores.stores[store.storeNumber].bought[itemSelected] = true;
            }
        }
        UpdateStore();
    }
}