using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public DeadManager DeadManager;
    public TileManager TileManager;
    public ScoreManager ScoreManager;

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameManager();
            }

            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}

