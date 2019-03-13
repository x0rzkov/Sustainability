﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    public bool canDrop;
    private bool once = false;
    private static Ship instance = null;
    public static Ship Instance
    {
        get
        {
            if (instance == null)
            {
                // This is where the magic happens.
                instance = FindObjectOfType(typeof(Ship)) as Ship;
            }

            // If it is still null, create a new instance
            if (instance == null)
            {
                GameObject i = new GameObject("Ship");
                i.AddComponent(typeof(Ship));
                instance = i.GetComponent<Ship>();
            }
            return instance;
        }
    }

    public List<Contract> currentContracts = new List<Contract>();
    public int currentPersonsOnShip;
    public GameObject currentShipMesh;
    public CharacterMovement characterMovement;
    public GameObject[] shipMeshes;

    //upgradables
    public int maxPersonsOnShip;
    public float currentFuel;
    public float baseFuel;
    public float maxFuel;
    public Slider uiSlider;
    public Slider storeUiSlider;
    public int currentShip;
    public Store currentStore;
    public delegate void EnterCity(Store store);
    public static event EnterCity OnEnterCity;

    public delegate void ExitCity();
    public static event ExitCity OnExitCity;

    [Header("Upgadables")]
    public List<GameObject> upgrades = new List<GameObject>();
    public List<FuelUpgrade> fuel = new List<FuelUpgrade>();


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("City")) {
            OnEnterCity?.Invoke(other.GetComponent<Store>());
        }
        //If gameObject has a personClass on it.
        if (other.gameObject.GetComponent<Person>() != null && !once)
        {
            once = true;
            Person p = other.gameObject.GetComponent<Person>();
            foreach (Contract c in currentContracts)
            {
                if(c.contractNumber == p.contract.contractNumber && currentPersonsOnShip < maxPersonsOnShip)
                {
                    //Person is a part of the contract.
                    c.colectedPersons++;
                    ContractManager.Instance.portUI.portrets[currentPersonsOnShip].sprite = p.portret;
                    currentPersonsOnShip++;
                    ContractManager.Instance.portretManager++;
                    //Contract is done if all persons are collected
                    if (c.personsToCollect == c.colectedPersons)
                    {
                        canDrop = true;
                        c.done = true;
                    }
                    ContractManager.Instance.passangers.Add(p);
                    p.gameObject.SetActive(false);
                }
            }
        }
    }

    private void UpdateMaxFuel() {
        maxFuel = baseFuel;
        foreach(FuelUpgrade f in fuel) {
            maxFuel += f.fuelUpgrade;
        }
        uiSlider.maxValue = maxFuel;
        storeUiSlider.maxValue = maxFuel;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("City")) {
            OnExitCity?.Invoke();
        }
        once = false;
    }

    private void Start() {
        InitListners();
        maxFuel = baseFuel;

        uiSlider.maxValue = maxFuel;
        storeUiSlider.maxValue = maxFuel;
    }

    private void InitListners() {
        ButtonManager.OnRefuelShip += Refuel;
        ButtonManager.OnItemBuy += StoreBuy;
    }

    private void Update() {
        uiSlider.value = currentFuel;
        storeUiSlider.value = currentFuel;
    }

    private void Refuel() {
        currentFuel = maxFuel;
    }

    private void StoreBuy(GameObject newObject, bool isShipStore) {
        if (!isShipStore) {
            GameObject temp = Instantiate(newObject, currentShipMesh.transform);
            temp.layer = 0;
            upgrades.Add(temp);

            //If the item has a fuelUpgrade
            if (temp.GetComponent<FuelUpgrade>() != null) {
                fuel.Add(temp.GetComponent<FuelUpgrade>());
                UpdateMaxFuel();
            }
        }
        else {
            currentShipMesh = Instantiate(newObject, this.transform);
            currentShipMesh.transform.localPosition = new Vector3(0, -.5f, 0);
            characterMovement.meshObject = currentShipMesh;
            currentShip = currentShipMesh.GetComponent<ShipParts>().shipType;
            currentShipMesh.layer = 0;
            foreach (Transform trans in currentShipMesh.GetComponentsInChildren<Transform>(true)) {
                trans.gameObject.layer = 0;
            }
            currentShipMesh.SetActive(true);
            currentShipMesh.transform.localScale = new Vector3(20, 20, 20);
            fuel.Clear();
        }
    }
}
