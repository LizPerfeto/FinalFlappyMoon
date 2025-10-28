#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<GameObject> availableSkinPrefabs = new List<GameObject>();

    public GameObject Player;

    [SerializeField] private GameObject GameOverCanvas;

    private GameObject selectedSkinPrefab;
    private SkinSprites selectedSkinSprites;
    private SpriteRenderer playerSpriteRenderer;

    private const string PrefKeySelected = "SelectedSkinIndex";

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }

        playerSpriteRenderer = Player != null ? Player.GetComponent<SpriteRenderer>() ?? Player.GetComponentInChildren<SpriteRenderer>() : null;

        if (availableSkinPrefabs == null || availableSkinPrefabs.Count == 0)
        {
            var loaded = Resources.LoadAll<GameObject>("Skins");
            if (loaded != null && loaded.Length > 0)
                availableSkinPrefabs = new List<GameObject>(loaded);
        }

        int savedIndex = PlayerPrefs.GetInt(PrefKeySelected, -1);

        if (savedIndex >= 0 && savedIndex < availableSkinPrefabs.Count)
        {
            SelectSkinPrefab(availableSkinPrefabs[savedIndex]);
        }
        else if (availableSkinPrefabs != null && availableSkinPrefabs.Count > 0)
        {
            SelectSkinPrefab(availableSkinPrefabs[0]);
        }

        if (playerSpriteRenderer != null)
        {
            int vida = (HealthManager.instance != null) ? HealthManager.instance.Vida : 3;
            ApplySpriteByVida(vida);
        }
        else
        {
            StartCoroutine(WaitAndApplySprite());
        }

        ShellCoinManager.instance?.CaptureSnapshot();
    }

    private System.Collections.IEnumerator WaitAndApplySprite()
    {
        float timeout = 2f;
        float t = 0f;
        while (t < timeout)
        {
            if (Player == null) Player = GameObject.FindWithTag("Player");
            if (Player != null)
                playerSpriteRenderer = Player.GetComponent<SpriteRenderer>() ?? Player.GetComponentInChildren<SpriteRenderer>();
            if (playerSpriteRenderer != null) break;
            t += Time.deltaTime;
            yield return null;
        }

        if (playerSpriteRenderer != null)
        {
            int vida = (HealthManager.instance != null) ? HealthManager.instance.Vida : 3;
            ApplySpriteByVida(vida);
        }
    }

    private void SelectSkinPrefab(GameObject prefab)
    {
        selectedSkinPrefab = prefab;
        if (selectedSkinPrefab != null)
        {
            selectedSkinSprites = selectedSkinPrefab.GetComponent<SkinSprites>() ?? selectedSkinPrefab.GetComponentInChildren<SkinSprites>();
        }
        else
        {
            selectedSkinSprites = null;
        }
    }

    public void ApplySpriteByVida(int vida)
    {
        if (playerSpriteRenderer == null)
        {
            if (Player != null)
                playerSpriteRenderer = Player.GetComponent<SpriteRenderer>() ?? Player.GetComponentInChildren<SpriteRenderer>();
            if (playerSpriteRenderer == null)
            {
                return;
            }
        }

        if (selectedSkinSprites != null)
        {
            var s = selectedSkinSprites.GetSpriteForVida(vida);
            if (s != null)
            {
                playerSpriteRenderer.sprite = s;
                return;
            }
        }

        if (selectedSkinPrefab != null)
        {
            var prefabSr = selectedSkinPrefab.GetComponent<SpriteRenderer>() ?? selectedSkinPrefab.GetComponentInChildren<SpriteRenderer>();
            if (prefabSr != null && prefabSr.sprite != null)
            {
                playerSpriteRenderer.sprite = prefabSr.sprite;
                return;
            }
        }

        var anim = Player.GetComponent<Animator>();
        if (anim != null) anim.enabled = false;
    }

    public void GameOver()
    {
        if (GameOverCanvas != null) GameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReturnToMenuFromGameOver()
    {
        Time.timeScale = 1f;
        if (GameOverCanvas != null) GameOverCanvas.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    public void ReturnToMenuDiscardProgress()
    {
        Time.timeScale = 1f;
        if (GameOverCanvas != null) GameOverCanvas.SetActive(false);
        ShellCoinManager.instance?.RevertToSnapshot();
        SceneManager.LoadScene("Menu");
    }

    public void ExitGameFromGameOver()
    {
        Time.timeScale = 1f;
        if (GameOverCanvas != null) GameOverCanvas.SetActive(false);

        if (ShellCoinManager.instance != null)
            ShellCoinManager.instance.SaveNow();
        PlayerPrefs.Save();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RestartGame()
    {
        if (HealthManager.instance != null) HealthManager.instance.ResetLives();

        Time.timeScale = 1f;

        if (GameOverCanvas != null) GameOverCanvas.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}