using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _currentScoreText;
    [SerializeField] TextMeshProUGUI _highScoreText;
    [SerializeField] TextMeshProUGUI _runDistanceText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }


    public void SetScoreText(float score, float distance)
    {
        gameObject.SetActive(true);
        _currentScoreText.text = "Score : " + ((int)score).ToString();

        if (score > PlayerPrefs.GetInt("HIGH_SCORE"))
        {
            PlayerPrefs.SetInt("HIGH_SCORE", (int)score);
        }
        _highScoreText.text = "High Score : "  + PlayerPrefs.GetInt("HIGH_SCORE").ToString();
        _runDistanceText.text = "Distance : " + (int)distance + "M";

    }

    public void OnReplayButtonEnter()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnTitleButtonEnter()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene("TitleScene");
    }

}