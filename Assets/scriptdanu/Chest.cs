using UnityEngine;

public class Chest : MonoBehaviour
{
    public void Interact()
    {
        // Panggil fungsi deposit di GameManager
        GameManager.instance.DepositItems();
    }
}