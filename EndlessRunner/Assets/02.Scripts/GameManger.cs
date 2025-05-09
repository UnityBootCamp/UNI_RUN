using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ΩÃ±€≈Ê
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


    static GameManager _instance;
    public DeadManager DeadManager;
    public TileManager TileManager;
    public ScoreManager ScoreManager;


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

