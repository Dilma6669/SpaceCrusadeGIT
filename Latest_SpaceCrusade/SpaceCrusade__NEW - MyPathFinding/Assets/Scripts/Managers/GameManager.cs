using UnityEngine;

public class GameManager : MonoBehaviour {

    ////////////////////////////////////////////////

    private static GameManager _instance;

    ////////////////////////////////////////////////

    // these are just references to the in-scene game objects
    public static GameObject _WorldManager;
    public static GameObject _PlayerManager;
    public static GameObject _LocationManager;
    public static GameObject _MovementManager;
    public static GameObject _CombatManager;
    public static GameObject _CameraManager;
    public static GameObject _UIManager;
    public static GameObject _NetworkManager;
    public static GameObject _UnitsManager;
    public static GameObject _LayerManager;

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        _WorldManager       = GameObject.Find("WorldManager");
        _PlayerManager      = GameObject.Find("PlayerManager");
        _LocationManager    = GameObject.Find("LocationManager");
        _MovementManager    = GameObject.Find("MovementManager");
        _CombatManager      = GameObject.Find("CombatManager");
        _CameraManager      = GameObject.Find("CameraManager");
        _UIManager          = GameObject.Find("UIManager");
        _NetworkManager     = GameObject.Find("NetworkManager");
        _UnitsManager       = GameObject.Find("UnitsManager");
        _LayerManager       = GameObject.Find("LayerManager");
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public static void StartGame(Vector3 worldStartLoc)
    {
        Debug.Log("fuck StartGame");
        UnitsManager.LoadPlayersUnits(worldStartLoc);
    }

}

