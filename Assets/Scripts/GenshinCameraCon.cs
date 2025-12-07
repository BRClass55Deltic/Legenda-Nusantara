using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenshinCameraCon : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float sensitivity = 0.2f;
    public float minY = -20f;
    public float maxY = 60f;

    public Vector3 editorOffset;
    public RectTransform touchArea; // panel kamera

    float rotX;
    float rotY;

    bool dragging = false; // apakah sedang drag pada area kamera

    void Start()
    {
        Vector3 a = transform.rotation.eulerAngles;
        rotX = a.y;
        rotY = a.x;

        editorOffset = transform.position - target.position;
    }

    void LateUpdate()
    {
        HandleMouseInput();
        HandleTouchInput();

        // APPLY CAMERA
        Quaternion rot = Quaternion.Euler(rotY, rotX, 0);
        Vector3 cameraBackOffset = rot * new Vector3(0, 0, -distance);

        transform.position = target.position + editorOffset + cameraBackOffset;
        transform.rotation = rot;
    }

    // =============================
    //         MOUSE LOGIC
    // =============================
    void HandleMouseInput()
    {
        // Start drag jika klik di dalam panel
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, Input.mousePosition))
                dragging = true;
        }

        // stop drag
        if (Input.GetMouseButtonUp(0))
            dragging = false;

        // rotate camera hanya jika sedang drag di panel
        if (dragging && Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotX += mouseX * sensitivity * 10f;
            rotY -= mouseY * sensitivity * 10f;
            rotY = Mathf.Clamp(rotY, minY, maxY);
        }
    }

    // =============================
    //         MOBILE LOGIC
    // =============================
    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, t.position))
                dragging = true;
        }

        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            dragging = false;
        }

        if (dragging && t.phase == TouchPhase.Moved)
        {
            rotX += t.deltaPosition.x * sensitivity;
            rotY -= t.deltaPosition.y * sensitivity;
            rotY = Mathf.Clamp(rotY, minY, maxY);
        }
    }
}