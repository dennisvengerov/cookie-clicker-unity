using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;   // background music
    public AudioSource sfxSource;     // for button sounds

    [Header("Click Sound Clips (small → large)")]
    public AudioClip[] clickClips;    // assign in Inspector

    int currentClipIndex = -1;        // -1 = SFX disabled
    bool sfxEnabled => currentClipIndex >= 0;

    GameManager gameManager;
    StoreUpgrade storeUpgrade;        // optional if you want keyboard for upgrades too

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Only background music at the start.
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }

        gameManager   = FindObjectOfType<GameManager>();
        storeUpgrade  = FindObjectOfType<StoreUpgrade>();
    }

    void Update()
    {
        // ENTER → click cookie
        if (Input.GetKeyDown(KeyCode.Return) && gameManager != null)
        {
            gameManager.ClickAction();
            PlayCookieClick();
        }

        // (Optional) another key for upgrades:
        // if (Input.GetKeyDown(KeyCode.UpArrow) && storeUpgrade != null) {
        //     storeUpgrade.ClickAction();
        //     PlayUpgradeClick();
        // }

        // RIGHT ARROW → cycle through click SFX clips (enables SFX)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CycleClickClip();
        }
    }

    void CycleClickClip()
    {
        if (clickClips == null || clickClips.Length == 0) return;

        currentClipIndex++;
        if (currentClipIndex >= clickClips.Length)
            currentClipIndex = 0;

        // You don’t *have* to assign to sfxSource.clip, but it’s nice for debugging
        sfxSource.clip = clickClips[currentClipIndex];
        Debug.Log($"SFX enabled. Now using clip #{currentClipIndex} : {sfxSource.clip.name}");
    }

    public void PlayCookieClick()
    {
        if (!sfxEnabled || sfxSource == null || clickClips == null || clickClips.Length == 0)
        {
            Debug.Log($"PlayCookieClick BLOCKED: sfxEnabled={sfxEnabled}, " +
                    $"sfxSourceNull={sfxSource == null}, " +
                    $"clipsLen={(clickClips == null ? -1 : clickClips.Length)}");
            return;
        }

        var clip = clickClips[currentClipIndex];
        Debug.Log($"PlayCookieClick playing clip: {clip.name}");
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUpgradeClick()
    {
        if (!sfxEnabled || sfxSource == null || clickClips.Length == 0) return;

        var clip = clickClips[currentClipIndex];
        sfxSource.PlayOneShot(clip);
    }
}

