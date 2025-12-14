using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalCollectable : MonoBehaviour
{
    public CrystalManager crystalManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            crystalManager.CollectCrystal();
            Destroy(gameObject);
        }
    }
}