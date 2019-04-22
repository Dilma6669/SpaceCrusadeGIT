using System.Collections.Generic;
using UnityEngine;

public class UnitBuilder : MonoBehaviour {

    ////////////////////////////////////////////////

    private static UnitBuilder _instance;

    ////////////////////////////////////////////////

    public List<GameObject> _unitPrefabs;

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

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public GameObject GetUnitModel(int choice)
    {
        return _unitPrefabs[choice];
    }
}
