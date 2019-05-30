using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    ////////////////////////////////////////////////

    private static WorldManager _instance;

    ////////////////////////////////////////////////

    // these are just references to the in-scene game objects
    public static GameObject _GridContainer;
    public static GameObject _WorldContainer;
    public static GameObject _NetworkContainer;

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
        _GridContainer = transform.Find("GridContainer").gameObject;
        _WorldContainer = transform.Find("WorldContainer").gameObject;
        _NetworkContainer = transform.Find("NetworkContainer").gameObject;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public static void BuildWorldForClient()
    {
        SyncedVars _syncedVars = GameObject.Find("SyncedVars").GetComponent<SyncedVars>(); // needs to be here, function runs before awake
        if (_syncedVars == null) { Debug.LogError("We got a problem here"); }

        int GlobalSeed = _syncedVars.GlobalSeed;
        Random.InitState(GlobalSeed);

        // Get the World Nodes
        WorldBuilder.BuildWorldNodes();

        LayerManager.MakeAllNodeLayersVisible();

        Debug.Log("FINSIHED Building World!!!!!!!!");
    }



}
