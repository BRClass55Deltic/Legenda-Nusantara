using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    public int totalCrystal = 3;
    private int currentCrystal = 0;

    [Header("UI Crystal")]
    public GameObject[] crystalUI; // size = 3

    [Header("Win Trigger")]
    public GameObject winTrigger; // collider kabur

    void Start()
    {
        // Hide semua UI di awal
        foreach (GameObject ui in crystalUI)
        {
            ui.SetActive(false);
        }

        // Matikan win trigger di awal
        if (winTrigger != null)
            winTrigger.SetActive(false);
    }

    public void CollectCrystal()
    {
        currentCrystal++;

        // Tampilkan UI sesuai urutan
        if (currentCrystal - 1 < crystalUI.Length)
        {
            crystalUI[currentCrystal - 1].SetActive(true);
        }

        // Kalau sudah 3 kristal
        if (currentCrystal >= totalCrystal)
        {
            Debug.Log("Semua kristal terkumpul!");
            ActivateWinCondition();
        }
    }

    void ActivateWinCondition()
    {
        if (winTrigger != null)
            winTrigger.SetActive(true);
    }
}