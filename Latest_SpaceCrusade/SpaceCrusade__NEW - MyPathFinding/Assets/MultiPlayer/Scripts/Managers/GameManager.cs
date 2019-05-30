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
        _WorldManager       = transform.Find("WorldManager").gameObject;
        _PlayerManager      = transform.Find("PlayerManager").gameObject;
        _LocationManager    = transform.Find("LocationManager").gameObject;
        _MovementManager    = transform.Find("MovementManager").gameObject;
        _CombatManager      = transform.Find("CombatManager").gameObject;
        _CameraManager      = transform.Find("CameraManager").gameObject;
        _UIManager          = transform.Find("UIManager").gameObject;
        _NetworkManager     = transform.Find("NetworkManager").gameObject;
        _UnitsManager       = transform.Find("UnitsManager").gameObject;
        _LayerManager       = transform.Find("LayerManager").gameObject;

        if (_WorldManager == null)      { Debug.LogError("We got a problem here"); }
        if (_PlayerManager == null)     { Debug.LogError("We got a problem here"); }
        if (_LocationManager == null)   { Debug.LogError("We got a problem here"); }
        if (_MovementManager == null)   { Debug.LogError("We got a problem here"); }
        if (_CombatManager == null)     { Debug.LogError("We got a problem here"); }
        if (_CameraManager == null)     { Debug.LogError("We got a problem here"); }
        if (_UIManager == null)         { Debug.LogError("We got a problem here"); }
        if (_NetworkManager == null)    { Debug.LogError("We got a problem here"); }
        if (_UnitsManager == null)      { Debug.LogError("We got a problem here"); }
        if (_LayerManager == null)      { Debug.LogError("We got a problem here"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public static void StartGame(Vector3 worldStartLoc)
    {
        Debug.Log(" ::::::::::::::::: START GAME :::::::::::::::::: ");
        UnitsManager.LoadPlayersUnits(worldStartLoc);
    }

}

