using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType { Wortel }


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public GameObject uiContainer; // <--- BARU: Wadah untuk semua UI Misi & Tas
    public Text missionText;
    public Text inventoryText;

    public Dictionary<ItemType, int> currentInventory = new Dictionary<ItemType, int>();

    private ItemType targetItem;
    private int targetAmount;
    private int collectedAmount;

    void Awake()
    {
        instance = this;
        currentInventory.Add(ItemType.Wortel, 0);
    }


    void Start()
    {
        GenerateNewMission();
        UpdateUI();
        
        // Default saat game mulai: Sembunyikan UI dulu (karena player belum masuk area)
        SetUIVisibility(false); 
    }

    // --- FUNGSI BARU UNTUK UI AREA ---
    public void SetUIVisibility(bool isVisible)
    {
        if (uiContainer != null)
        {
            uiContainer.SetActive(isVisible);
        }
    }
    // ---------------------------------

    public void GenerateNewMission()
    {
        targetItem = ItemType.Wortel;
        targetAmount = Random.Range(1, 6); // jumlah masih random
        collectedAmount = 0;
        UpdateUI();
    }


    public void AddItem(ItemType item)
    {
        currentInventory[item]++;
        UpdateUI();
    }

    public void DepositItems()
    {
        int amountCarried = currentInventory[targetItem];

        if (amountCarried > 0)
        {
            collectedAmount += amountCarried;
            currentInventory[targetItem] = 0;

            if (collectedAmount >= targetAmount)
            {
                Debug.Log("Misi Selesai!");
                GenerateNewMission();
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        missionText.text = $"Misi: Kumpulkan {targetAmount} {targetItem}\n" +
                           $"Terkumpul: {collectedAmount}/{targetAmount}";

        string invStr = "Tas:\n";
        foreach (var item in currentInventory)
        {
            if (item.Value > 0)
                invStr += $"{item.Key}: {item.Value}\n";
        }
        inventoryText.text = invStr;
    }
}