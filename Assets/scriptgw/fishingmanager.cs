using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FishRarity { Common, Rare, Epic, Secret }

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;

    [Header("References")]
    public GameObject realRodObject;   // Pancingan di tangan 
    public GameObject pickupRodObject; // Pancingan di tanah
    public Transform playerTransform;
    public Transform rodTipPoint;      // TITIK UJUNG KAIL

    [Header("Fish Prefabs (Visual)")]
    public GameObject fishCommonPrefab; 
    public GameObject fishRarePrefab;   
    public GameObject fishEpicPrefab;   
    public GameObject fishSecretPrefab; 

    [Header("Settings")]
    public Vector3 rodEquippedOffset = new Vector3(0.5f, 1.0f, 1.5f); 
    public GameObject bobberPrefab;   
    public float castDistance = 5f;
    public float waterHeight = 6.2f; // Pastikan ini sesuai tinggi air kamu
    public float throwSpeed = 10f; 

    [Header("UI References")]
    public GameObject fishingUIContainer; // Panel UI
    public Button actionButton;           // Tombol Cast (Visual)
    public Text actionButtonText;         // Teks Tombol
    public Text fishInventoryText;        // Teks Tas Ikan

    // STATE VARIABLES
    private bool hasRod = false;
    private bool isFishing = false;
    private bool isWaitingForBite = false;
    private bool isFishHooked = false;
    private GameObject currentBobber;

    private Dictionary<FishRarity, int> fishInventory = new Dictionary<FishRarity, int>();

    void Awake()
    {
        instance = this;
        foreach (FishRarity r in System.Enum.GetValues(typeof(FishRarity)))
            fishInventory.Add(r, 0);
    }

    void Start()
    {
        // Matikan UI dan Rod saat mulai
        if(fishingUIContainer != null) fishingUIContainer.SetActive(false);
        if(realRodObject != null) realRodObject.SetActive(false);
        
        // Setup tombol UI
        if(actionButton != null) 
        {
            actionButton.onClick.RemoveAllListeners(); 
            actionButton.onClick.AddListener(OnActionPress);
        }
        
        UpdateInventoryUI();
    }

    void Update()
    {
        // Jika sudah punya pancingan, tekan F untuk aksi
        if (IsRodEquipped() && Input.GetKeyDown(KeyCode.F))
        {
            OnActionPress();
        }
    }

    public void PickupRod()
    {
        hasRod = true;
        pickupRodObject.SetActive(false);

        realRodObject.SetActive(true);
        realRodObject.transform.SetParent(playerTransform);
        realRodObject.transform.localPosition = rodEquippedOffset;
        realRodObject.transform.localRotation = Quaternion.identity;

        fishingUIContainer.SetActive(true);
        ResetFishingState();

        // 🔥 PINDAH KE FPS CAMERA (HANYA INI!)
        CameraModeManager camManager = FindObjectOfType<CameraModeManager>();
        if (camManager != null)
        {
            camManager.SwitchToFirstPerson();
        }
    }

    // Fungsi Utama (Dipanggil oleh Tombol F atau Klik UI)
    void OnActionPress()
    {
        if (!hasRod) return; 

        // Logika Status Mancing
        if (!isFishing) 
        {
            CastRod(); // Lempar
        }
        else if (isFishHooked) 
        {
            ReelIn(); // Tarik (Dapat Ikan)
        }
        else if (isWaitingForBite)
        {
            // Kalau ditarik saat masih nunggu ("Wait..."), gagal/batal
            StopFishing(); 
            Debug.Log("Ditarik terlalu cepat!");
        }
    }

    void CastRod()
    {
        isFishing = true;
        if(actionButton != null) actionButton.interactable = false; // Disable visual tombol
        if(actionButtonText != null) actionButtonText.text = "...";

        // GUNAKAN POSISI ROD TIP
        Vector3 startPos = (rodTipPoint != null) ? rodTipPoint.position : realRodObject.transform.position;
        Vector3 targetPos = playerTransform.position + (playerTransform.forward * castDistance);
        targetPos.y = waterHeight; 

        currentBobber = Instantiate(bobberPrefab, startPos, Quaternion.identity);
        StartCoroutine(CastAnimation(startPos, targetPos));
    }

    IEnumerator CastAnimation(Vector3 start, Vector3 end)
    {
        float t = 0;
        float duration = Vector3.Distance(start, end) / throwSpeed;

        // Animasi pelampung melengkung jatuh ke air
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            Vector3 currentPos = Vector3.Lerp(start, end, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * 1.5f; 
            if(currentBobber != null) currentBobber.transform.position = currentPos;
            yield return null;
        }
        
        // --- BAGIAN INI YANG DIPERBAIKI ---
        if(currentBobber != null) 
        {
            currentBobber.transform.position = end; // Paksa posisi pas di air

            // Aktifkan script bobber agar mulai mengapung DI SINI, bukan di awal spawn
            FishingBobber bobberScript = currentBobber.GetComponent<FishingBobber>();
            if (bobberScript != null)
            {
                bobberScript.StartFloating(end.y);
            }
        }
        // ----------------------------------

        isWaitingForBite = true;
        if(actionButtonText != null) actionButtonText.text = "Wait..."; 
        StartCoroutine(WaitForFishRoutine());
    }

    IEnumerator WaitForFishRoutine()
    {
        float waitTime = Random.Range(2f, 5f);
        yield return new WaitForSeconds(waitTime);

        // Cek jika player membatalkan/stop fishing di tengah jalan
        if(!isFishing) yield break;

        isWaitingForBite = false;
        isFishHooked = true;
        
        if(actionButton != null) actionButton.interactable = true; 
        if(actionButtonText != null) actionButtonText.text = "PULL NOW! (Press F)"; 
    }

    void ReelIn()
    {
        if(actionButton != null) actionButton.interactable = false; 

        // --- TAMBAHAN PENTING: STOP LOGIKA MENGAPUNG ---
        if (currentBobber != null)
        {
            FishingBobber bobberScript = currentBobber.GetComponent<FishingBobber>();
            if (bobberScript != null)
            {
                bobberScript.StopFloating(); // Suruh bobber berhenti diam di air
            }
        }
        // -----------------------------------------------

        FishRarity caughtFish = CalculateCatch();
        fishInventory[caughtFish]++;
        UpdateInventoryUI();

        // Visual Ikan
        GameObject fishVisual = null;
        switch (caughtFish)
        {
            case FishRarity.Common: fishVisual = fishCommonPrefab; break;
            case FishRarity.Rare: fishVisual = fishRarePrefab; break;
            case FishRarity.Epic: fishVisual = fishEpicPrefab; break;
            case FishRarity.Secret: fishVisual = fishSecretPrefab; break;
        }

        if (fishVisual != null && currentBobber != null)
        {
            GameObject caughtObj = Instantiate(fishVisual, currentBobber.transform.position, Quaternion.identity);
            caughtObj.transform.SetParent(currentBobber.transform);
            
            // Atur rotasi ikan agar menghadap ke atas/samping sesuai keinginan
            caughtObj.transform.localRotation = Quaternion.Euler(0, 0, 90); 
            caughtObj.transform.localPosition = new Vector3(0, -0.5f, 0); 
        }

        StartCoroutine(ReelAnimation());
    }

    IEnumerator ReelAnimation()
    {
        if(currentBobber == null) yield break;
        
        // Tentukan target (ujung pancingan)
        Vector3 targetTip = (rodTipPoint != null) ? rodTipPoint.position : realRodObject.transform.position;

        // Gerakkan pelampung+ikan ke arah ujung pancingan
        while (Vector3.Distance(currentBobber.transform.position, targetTip) > 0.5f)
        {
            // Update posisi target terus menerus (jaga-jaga kalau player bergerak sedikit)
            targetTip = (rodTipPoint != null) ? rodTipPoint.position : realRodObject.transform.position;
            
            currentBobber.transform.position = Vector3.MoveTowards(currentBobber.transform.position, targetTip, throwSpeed * Time.deltaTime);
            yield return null;
        }

        // --- PERUBAHAN DI SINI ---
        // Begitu jarak sudah dekat (sampai), langsung reset.
        // Fungsi ResetFishingState() akan memanggil StopFishing(), 
        // yang mana StopFishing() berisi perintah Destroy(currentBobber).
        // Karena Ikan adalah anak (child) dari Bobber, ikan juga ikut terhapus otomatis.
        
        ResetFishingState(); 
    }

    FishRarity CalculateCatch()
    {
        float rand = Random.Range(0f, 100f);
        if (rand < 50f) return FishRarity.Common;    
        else if (rand < 80f) return FishRarity.Rare;  
        else if (rand < 95f) return FishRarity.Epic;  
        else return FishRarity.Secret;                
    }

    void StopFishing()
    {
        isFishing = false;
        isFishHooked = false;
        isWaitingForBite = false;
        if(actionButtonText != null) actionButtonText.text = "Cast (Press F)";
        if(actionButton != null) actionButton.interactable = true;
        if (currentBobber != null) Destroy(currentBobber);
    }

    void ResetFishingState() { StopFishing(); }

    void UpdateInventoryUI()
    {
        if (fishInventoryText == null) return;
        string text = "Fish Bag:\n";
        foreach(var fish in fishInventory)
            if(fish.Value > 0) text += $"{fish.Key}: {fish.Value}\n";
        fishInventoryText.text = text;
    }

    public void SetFishingAreaActive(bool isActive)
    {
        if (IsRodEquipped())
        {
            if (isActive)
            {
                realRodObject.transform.SetParent(playerTransform);
                realRodObject.transform.localPosition = rodEquippedOffset;
                realRodObject.transform.localRotation = Quaternion.identity;
                realRodObject.SetActive(true);
                fishingUIContainer.SetActive(true);
            }
            else
            {
                realRodObject.transform.SetParent(null);
                realRodObject.SetActive(false);
                fishingUIContainer.SetActive(false);
                if (isFishing) StopFishing();

                // 🔥 REAL ROD HILANG → BALIK KE TPS
                SwitchBackToTPS();
            }
        }
        else
        {
            if (pickupRodObject != null)
                pickupRodObject.SetActive(isActive);

            // 🔥 TIDAK ADA REAL ROD → TPS
            SwitchBackToTPS();
        }
    }

    void SwitchBackToTPS()
    {
        CameraModeManager camManager = FindObjectOfType<CameraModeManager>();
        if (camManager != null)
        {
            camManager.SwitchToThirdPerson();
        }
    }
    bool IsRodEquipped()
    {
        return realRodObject != null && realRodObject.activeSelf;
    }

}