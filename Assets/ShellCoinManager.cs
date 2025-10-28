using UnityEngine;
using System;

public class ShellCoinManager : MonoBehaviour
{
    private static ShellCoinManager _instance;
    private static bool isQuitting = false;

    public static ShellCoinManager instance
    {
        get
        {
            if (_instance != null) return _instance;

            var found = UnityEngine.Object.FindObjectsOfType<ShellCoinManager>();
            if (found != null && found.Length > 0)
            {
                _instance = found[0];
                return _instance;
            }

            if (!Application.isPlaying || isQuitting) return null;

            var go = new GameObject("ShellCoinManager");
            _instance = go.AddComponent<ShellCoinManager>();
            return _instance;
        }
        private set => _instance = value;
    }

    public int Coins { get; private set; }

    public event Action<int> OnCoinsChanged;

    private const string PrefsKey = "ShellCoins";

    private int sessionSnapshot = -1;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Load()
    {
        Coins = PlayerPrefs.GetInt(PrefsKey, 0);
        OnCoinsChanged?.Invoke(Coins);
    }

    private void Save()
    {
        PlayerPrefs.SetInt(PrefsKey, Coins);
        PlayerPrefs.Save();
    }

    public void SaveNow()
    {
        Save();
    }

    public void CaptureSnapshot()
    {
        sessionSnapshot = Coins;
    }

    public void RevertToSnapshot()
    {
        if (sessionSnapshot < 0)
        {
            return;
        }

        Coins = sessionSnapshot;
        sessionSnapshot = -1;
        OnCoinsChanged?.Invoke(Coins);
        Save();
    }

    public void ClearSnapshot()
    {
        sessionSnapshot = -1;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
        Save();
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (Coins >= amount)
        {
            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            Save();
            return true;
        }
        return false;
    }

    public void ResetCoins()
    {
        Coins = 0;
        OnCoinsChanged?.Invoke(Coins);
        Save();
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
        Save();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
