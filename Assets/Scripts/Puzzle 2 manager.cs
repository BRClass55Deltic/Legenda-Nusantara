using UnityEngine;
using System.Collections;

public class ActivateButoIjo : MonoBehaviour
{
    public GameObject butoIjo;

    [Header("Camera Settings")]
    public GameObject cameraObject;      // ← DRAG CAMERA KE SINI
    public float rotationSpeed = 360f;    // derajat per detik
    public float lookBackDelay = 1f;

    private Transform cam;
    private bool triggered = false;

    void Start()
    {
        if (butoIjo != null)
            butoIjo.SetActive(false);

        if (cameraObject != null)
            cam = cameraObject.transform;
        else
            Debug.LogError("Camera Object belum di-assign!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;

            if (butoIjo != null)
            {
                butoIjo.SetActive(true);
                Debug.Log("SPAWNED");
            }

            if (cam != null)
                StartCoroutine(RotateCameraRoutine());
        }
    }

    IEnumerator RotateCameraRoutine()
    {
        Quaternion startRot = cam.rotation;
        Quaternion lookBackRot = startRot * Quaternion.Euler(0f, 180f, 0f);

        // === ROTASI KE BELAKANG ===
        while (Quaternion.Angle(cam.rotation, lookBackRot) > 0.5f)
        {
            cam.rotation = Quaternion.RotateTowards(
                cam.rotation,
                lookBackRot,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        yield return new WaitForSeconds(lookBackDelay);

        // === KEMBALI KE ARAH AWAL ===
        while (Quaternion.Angle(cam.rotation, startRot) > 0.5f)
        {
            cam.rotation = Quaternion.RotateTowards(
                cam.rotation,
                startRot,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
}
