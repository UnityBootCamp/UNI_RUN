using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject ControlKeyMenu;

    public void OnStartButtonEnter()
    {
        // Scene �̸��� string Ÿ���� �Ķ���ͷ� �Է¹޾� ���� �ε��Ѵ�
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

    // ������ �� ȯ�濡���� �����
    // ���� ȯ�� �ʿ����� ���Ḧ ��Ȳ�� ���� ó��
    public void OnExitButtonEnter()
    {

        #if UNITY_EDITOR // ����Ƽ ������ �ʿ����� �۾�
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif 

        // ��ư ������ ��������
        //Application.Quit();
    }
}
