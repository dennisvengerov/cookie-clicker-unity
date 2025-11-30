using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem; // Uses New Input System

public class CreamStressTest : MonoBehaviour
{
    [Header("Assets")]
    public Sprite creamSprite;
    public RectTransform uiParent; 

    [Header("Settings")]
    public int currentCount = 10; 
    public float spawnInterval = 0.02f; // Initial delay
    
    [Header("Ramp Up Settings")]
    public float countMultiplier = 1.1f; // Increase count by 10%
    public float intervalDecay = 0.935f;   // Decrease delay by 10% (makes it faster)

    [Header("Visuals")]
    public Vector2 padding = new Vector2(40f, 40f);
    public Vector2 scaleRange = new Vector2(0.6f, 0.9f); 

    private Rect parentRect;

    void Start()
    {
        if (uiParent == null)
        {
            Debug.LogError("❌ Assign uiParent in the Inspector!");
            return;
        }

        parentRect = uiParent.rect;
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        // Check for Enter Key (New Input System)
        if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            RampUp();
        }
    }

    void RampUp()
    {
        StopAllCoroutines();

        // 1. Clear Screen
        foreach (Transform child in uiParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Increase Count (Exponential Growth)
        int previousCount = currentCount;
        currentCount = Mathf.CeilToInt(currentCount * countMultiplier);
        if (currentCount == previousCount) currentCount++;

        // 3. Decrease Delay (Exponential Speedup)
        spawnInterval = spawnInterval * intervalDecay;

        Debug.Log($"🚀 Ramping Up! Count: {currentCount} | Delay: {spawnInterval:F4}s");

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < currentCount; i++)
        {
            SpawnOne();
            
            // Only wait if the interval is noticeable
            if (spawnInterval > 0.0001f)
                yield return new WaitForSecondsRealtime(spawnInterval);
        }
    }

    void SpawnOne()
    {
        GameObject go = new GameObject("Cream", typeof(RectTransform), typeof(Image));
        
        go.transform.SetParent(uiParent, false);
        go.transform.SetAsLastSibling(); 

        Image img = go.GetComponent<Image>();
        img.sprite = creamSprite;
        img.raycastTarget = false; 
        img.SetNativeSize();

        // Position
        RectTransform rt = (RectTransform)go.transform;
        float x = Random.Range(parentRect.xMin + padding.x, parentRect.xMax - padding.x);
        float y = Random.Range(parentRect.yMin + padding.y, parentRect.yMax - padding.y);
        rt.anchoredPosition = new Vector2(x, y);

        // Scale Logic (0.01f multiplier included)
        float s = Random.Range(scaleRange.x, scaleRange.y) * 0.01f; 
        rt.localScale = new Vector3(s, s, 1f);
        
        // Random Rotation
        rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-12f, 12f));
    }
}