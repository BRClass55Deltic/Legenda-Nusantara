using UnityEngine;

public class ActivateButoIjo : MonoBehaviour
{
    public GameObject butoIjo;

    void Start()
    {
        // Pastikan Buto Ijo awalnya mati
        if (butoIjo != null)
            butoIjo.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (butoIjo != null)
            {
                butoIjo.SetActive(true);
            }
        }
    }
}
