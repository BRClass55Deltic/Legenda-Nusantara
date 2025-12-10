using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 2.5f; // Jarak bisa mengambil barang
    public LayerMask interactableLayer;   // Layer khusus barang/peti

    void Update()
    {
        // --- TOMBOL E (AMBIL BARANG) ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryCollectItem();
        }

        // --- TOMBOL F (BUKA PETI/SETOR) ---
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryOpenChest();
        }
    }

    void TryCollectItem()
    {
        // Cari object di sekitar player dalam radius interactionRange
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);
        
        foreach (var hit in hitColliders)
        {
            CollectableItem item = hit.GetComponent<CollectableItem>();
            if (item != null)
            {
                item.Interact();
                return; // Ambil satu barang saja per klik
            }
        }
    }

    void TryOpenChest()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);

        foreach (var hit in hitColliders)
        {
            Chest chest = hit.GetComponent<Chest>();
            if (chest != null)
            {
                chest.Interact();
                return;
            }
        }
    }

    // Untuk visualisasi jarak di Scene View (Gizmos)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}