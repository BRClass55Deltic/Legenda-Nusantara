using UnityEngine;

public class ActivateButoIjo : MonoBehaviour
{
    public GameObject butoIjo;

    [Header("Camera Control")]
    public GenshinCameraCon genshinCamera;
    public float rotationSpeed = 360f;
    public float lookBackDelay = 1f;

    bool triggered = false;

    void Start()
    {
        if (butoIjo != null)
            butoIjo.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;

            if (butoIjo != null)
                butoIjo.SetActive(true);

            if (genshinCamera != null)
            {
                StartCoroutine(
                    genshinCamera.RotateYTemporary(
                        180f,
                        rotationSpeed,
                        lookBackDelay
                    )
                );
            }
        }
    }
}
