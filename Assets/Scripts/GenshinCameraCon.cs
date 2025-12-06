using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenshinCameraCon : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float sensitivity = 0.2f;
    public float minY = -20f;
    public float maxY = 60f;

    public Vector3 editorOffset; // offset dari posisi editor

    float rotX;
    float rotY;

    void Start()
    {
        // Ambil rotasi kamera dari editor
        Vector3 a = transform.rotation.eulerAngles;
        rotX = a.y;
        rotY = a.x;

        // Ambil offset awal dari posisi kamera saat di editor
        editorOffset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // =============================
        //   MOUSE INPUT (EDITOR / PC)
        // =============================
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotX += mouseX * sensitivity * 10f;
            rotY -= mouseY * sensitivity * 10f;
            rotY = Mathf.Clamp(rotY, minY, maxY);
        }

        // =============================
        //      TOUCH INPUT (MOBILE)
        // =============================
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                rotX += t.deltaPosition.x * sensitivity;
                rotY -= t.deltaPosition.y * sensitivity;
                rotY = Mathf.Clamp(rotY, minY, maxY);
            }
        }

        // APPLY CAMERA
        Quaternion rot = Quaternion.Euler(rotY, rotX, 0);

        // offset belakang kamera seperti genshin
        Vector3 cameraBackOffset = rot * new Vector3(0, 0, -distance);

        // posisi akhir = posisi target + offset editor + offset rotasi
        transform.position = target.position + editorOffset + cameraBackOffset;
        transform.rotation = rot;
    }
}