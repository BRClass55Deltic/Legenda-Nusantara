using System.Collections;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public ItemType itemType; // Tentukan di Inspector (Wortel/Jahe/dll)
    private Collider col;
    private Renderer rend;

    void Start()
    {
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
    }

    public void Interact()
    {
        // Tambahkan ke inventory
        GameManager.instance.AddItem(itemType);
        
        // Mulai proses respawn
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        // Sembunyikan object dan matikan collider
        rend.enabled = false;
        col.enabled = false;

        // Tunggu 7 detik
        yield return new WaitForSeconds(7f);

        // Munculkan kembali
        rend.enabled = true;
        col.enabled = true;
    }
}