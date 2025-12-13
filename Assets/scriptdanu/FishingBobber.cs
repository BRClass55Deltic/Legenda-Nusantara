using UnityEngine;

public class FishingBobber : MonoBehaviour
{
    private float startY;
    public float floatStrength = 0.1f;
    public float frequency = 2f;
    private bool isFloating = false; 

    public void StartFloating(float waterLevel)
    {
        startY = waterLevel;
        isFloating = true;
    }

    // --- TAMBAHAN BARU: FUNGSI STOP ---
    public void StopFloating()
    {
        isFloating = false; // Mematikan paksaan posisi Y
    }
    // ----------------------------------

    void Update()
    {
        if (!isFloating) return;

        float newY = startY + Mathf.Sin(Time.time * frequency) * floatStrength;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}