using UnityEngine;

public class FishingAreaZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FishingManager.instance.SetFishingAreaActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FishingManager.instance.SetFishingAreaActive(false);
        }
    }
}