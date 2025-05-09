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

    
    // 게임오버시에 결산창 세팅
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

    // 재시작버튼
    public void OnReplayButtonEnter()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 타이틀버튼
    public void OnTitleButtonEnter()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene("TitleScene");
    }

}