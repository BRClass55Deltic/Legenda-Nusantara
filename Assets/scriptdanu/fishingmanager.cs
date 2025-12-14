using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FishRarity { Common, Rare, Epic, Secret }

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;

    [Header("References")]
    public GameObject realRodObject;
    public GameObject pickupRodObject;
    public Transform playerTransform;
    public Transform rodTipPoint;

    [Header("Fish Prefabs")]
    public GameObject fishCommonPrefab;
    public GameObject fishRarePrefab;
    public GameObject fishEpicPrefab;
    public GameObject fishSecretPrefab;

    [Header("Settings")]
    public Vector3 rodEquippedOffset = new Vector3(0.5f, 1.0f, 1.5f);
    public GameObject bobberPrefab;
    public float castDistance = 5f;
    public float waterHeight = 6.2f;
    public float throwSpeed = 10f;

    [Header("UI")]
    public GameObject fishingUIContainer;
    public Button castButton;
    public Button reelButton;
    public Text fishInventoryText;

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
        fishingUIContainer.SetActive(false);
        realRodObject.SetActive(false);

        castButton.onClick.AddListener(OnActionPress);
        reelButton.onClick.AddListener(OnActionPress);

        ShowCastButton();
        UpdateInventoryUI();
    }

    void Update()
    {
        if (IsRodEquipped() && Input.GetKeyDown(KeyCode.F))
            OnActionPress();
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

        CameraModeManager camManager = FindObjectOfType<CameraModeManager>();
        if (camManager != null)
            camManager.SwitchToFirstPerson();
    }

    void OnActionPress()
    {
        if (!hasRod) return;

        if (!isFishing)
            CastRod();
        else if (isFishHooked)
            ReelIn();
        else if (isWaitingForBite)
            StopFishing();
    }

    void CastRod()
    {
        isFishing = true;
        ShowCastDisabled();

        Vector3 start = rodTipPoint.position;
        Vector3 end = playerTransform.position + playerTransform.forward * castDistance;
        end.y = waterHeight;

        currentBobber = Instantiate(bobberPrefab, start, Quaternion.identity);
        StartCoroutine(CastAnimation(start, end));
    }

    IEnumerator CastAnimation(Vector3 start, Vector3 end)
    {
        float t = 0;
        float dur = Vector3.Distance(start, end) / throwSpeed;

        while (t < 1)
        {
            t += Time.deltaTime / dur;
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * 1.5f;
            currentBobber.transform.position = pos;
            yield return null;
        }

        currentBobber.transform.position = end;
        currentBobber.GetComponent<FishingBobber>()?.StartFloating(end.y);

        isWaitingForBite = true;
        StartCoroutine(WaitForFishRoutine());
    }

    IEnumerator WaitForFishRoutine()
    {
        yield return new WaitForSeconds(Random.Range(2f, 5f));

        if (!isFishing) yield break;

        isWaitingForBite = false;
        isFishHooked = true;
        ShowReelButton();
    }

    void ReelIn()
    {
        ShowCastDisabled();

        currentBobber.GetComponent<FishingBobber>()?.StopFloating();

        FishRarity fish = CalculateCatch();
        fishInventory[fish]++;
        UpdateInventoryUI();

        GameObject prefab =
            fish == FishRarity.Common ? fishCommonPrefab :
            fish == FishRarity.Rare ? fishRarePrefab :
            fish == FishRarity.Epic ? fishEpicPrefab :
            fishSecretPrefab;

        if (prefab)
        {
            GameObject f = Instantiate(prefab, currentBobber.transform);
            f.transform.localPosition = new Vector3(0, -0.5f, 0);
            f.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }

        StartCoroutine(ReelAnimation());
    }

    IEnumerator ReelAnimation()
    {
        while (Vector3.Distance(currentBobber.transform.position, rodTipPoint.position) > 0.5f)
        {
            currentBobber.transform.position =
                Vector3.MoveTowards(currentBobber.transform.position, rodTipPoint.position, throwSpeed * Time.deltaTime);
            yield return null;
        }

        ResetFishingState();
    }

    FishRarity CalculateCatch()
    {
        float r = Random.Range(0f, 100f);
        if (r < 50) return FishRarity.Common;
        if (r < 80) return FishRarity.Rare;
        if (r < 95) return FishRarity.Epic;
        return FishRarity.Secret;
    }

    void StopFishing()
    {
        isFishing = false;
        isWaitingForBite = false;
        isFishHooked = false;

        ShowCastButton();

        if (currentBobber) Destroy(currentBobber);
    }

    void ResetFishingState() => StopFishing();

    void ShowCastButton()
    {
        castButton.gameObject.SetActive(true);
        castButton.interactable = true;
        reelButton.gameObject.SetActive(false);
    }

    void ShowReelButton()
    {
        castButton.gameObject.SetActive(false);
        reelButton.gameObject.SetActive(true);
        reelButton.interactable = true;
    }

    void ShowCastDisabled()
    {
        castButton.gameObject.SetActive(true);
        castButton.interactable = false;
        reelButton.gameObject.SetActive(false);
    }

    void UpdateInventoryUI()
    {
        string s = "Fish Bag:\n";
        foreach (var f in fishInventory)
            if (f.Value > 0) s += $"{f.Key}: {f.Value}\n";
        fishInventoryText.text = s;
    }

    // ❌ TIDAK DIUBAH
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
                SwitchBackToTPS();
            }
        }
        else
        {
            if (pickupRodObject != null)
                pickupRodObject.SetActive(isActive);

            SwitchBackToTPS();
        }
    }

    void SwitchBackToTPS()
    {
        CameraModeManager camManager = FindObjectOfType<CameraModeManager>();
        if (camManager != null)
            camManager.SwitchToThirdPerson();
    }

    bool IsRodEquipped()
    {
        return realRodObject != null && realRodObject.activeSelf;
    }
}
