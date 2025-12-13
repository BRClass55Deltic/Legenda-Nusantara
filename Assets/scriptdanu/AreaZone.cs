using UnityEngine;

public class AreaZone : MonoBehaviour
{
    // Fungsi bawaan Unity saat ada object masuk ke area Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Pastikan yang masuk adalah Player
        if (other.CompareTag("Player"))
        {
            GameManager.instance.SetUIVisibility(true); // Munculkan UI
            Debug.Log("Masuk Area Bahan");
        }
    }

    // Fungsi bawaan Unity saat ada object keluar dari area Trigger
    private void OnTriggerExit(Collider other)
    {
        // Pastikan yang keluar adalah Player
        if (other.CompareTag("Player"))
        {
            GameManager.instance.SetUIVisibility(false); // Sembunyikan UI
            Debug.Log("Keluar Area Bahan");
        }
    }
}