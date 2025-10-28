using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static Score instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int _score;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
           
        }
    }

    private void Start()
    {
        scoreText.text = _score.ToString();
        highScoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        UpdateHighScore();
    }

    private void UpdateHighScore()
    {
        if(_score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", _score);
            highScoreText.text = _score.ToString();
        }
    }

    public void UpdateScore()
    {
        _score++;
        scoreText.text = _score.ToString();
        UpdateHighScore();
    }
}
