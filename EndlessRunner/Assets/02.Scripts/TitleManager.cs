using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject ControlKeyMenu;

    public void OnStartButtonEnter()
    {
        // Scene 이름을 string 타입의 파라미터로 입력받아 씬을 로드한다
        SceneManager.LoadScene("GameScene");
    }
    public void OnCloseButtonEnter()
    {

        ControlKeyMenu.SetActive(false);
    }

    public void OnControlKeyButtonEnter()
    {
        ControlKeyMenu.SetActive(true);
    }

    // 에디터 쪽 환경에서의 종료와
    // 빌드 환경 쪽에서의 종료를 상황에 따라 처리
    public void OnExitButtonEnter()
    {

        #if UNITY_EDITOR // 유니티 에디터 쪽에서의 작업
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif 

        // 버튼 누를시 게임종료
        //Application.Quit();
    }
}
