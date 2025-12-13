using UnityEngine;

public class CameraModeManager : MonoBehaviour
{
    public enum CameraMode { ThirdPerson, FirstPerson }
    public CameraMode currentMode = CameraMode.ThirdPerson;

    [Header("Camera Objects")]
    public GameObject thirdPersonCamera;
    public GameObject firstPersonCamera;

    // Movement scripts (auto find)
    MonoBehaviour thirdPersonMovement;
    MonoBehaviour firstPersonMovement;

    // Camera controller scripts (auto find)
    MonoBehaviour thirdPersonCameraScript;
    MonoBehaviour firstPersonCameraScript;

    void Awake()
    {
        // === AUTO CARI MOVEMENT DI PLAYER (PARENT & CHILD) ===
        thirdPersonMovement = GetComponentInChildren<Movement>(true);
        firstPersonMovement = GetComponentInChildren<FirstPersonMovement>(true);

        // === AUTO CARI CAMERA SCRIPT ===
        if (thirdPersonCamera != null)
            thirdPersonCameraScript =
                thirdPersonCamera.GetComponent<GenshinCameraCon>();

        if (firstPersonCamera != null)
            firstPersonCameraScript =
                firstPersonCamera.GetComponent<FirstPersonCamera>();
    }


    void Start()
    {
        SetMode(currentMode);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            currentMode = currentMode == CameraMode.ThirdPerson
                ? CameraMode.FirstPerson
                : CameraMode.ThirdPerson;

            SetMode(currentMode);
        }
    }

    void SetMode(CameraMode mode)
    {
        bool isFP = mode == CameraMode.FirstPerson;

        // === CAMERA OBJECT ===
        if (thirdPersonCamera != null)
            thirdPersonCamera.SetActive(!isFP);

        if (firstPersonCamera != null)
            firstPersonCamera.SetActive(isFP);

        // === MOVEMENT SCRIPT ===
        if (thirdPersonMovement != null)
            thirdPersonMovement.enabled = !isFP;

        if (firstPersonMovement != null)
            firstPersonMovement.enabled = isFP;

        // === CAMERA CONTROLLER SCRIPT ===
        if (thirdPersonCameraScript != null)
            thirdPersonCameraScript.enabled = !isFP;

        if (firstPersonCameraScript != null)
            firstPersonCameraScript.enabled = isFP;

        // === CURSOR ===
        Cursor.lockState = isFP ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isFP;
    }

    public void SwitchToFirstPerson()
    {
        currentMode = CameraMode.FirstPerson;
        SetMode(currentMode);
    }

    public void SwitchToThirdPerson()
    {
        currentMode = CameraMode.ThirdPerson;
        SetMode(currentMode);
    }
}
