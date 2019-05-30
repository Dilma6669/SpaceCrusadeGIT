using UnityEngine;

public class CubeLocation_SERVER : ScriptableObject
{
    // All Checks combined into two bools
    bool _isHumanMoveable;
    bool _isAlienMoveable;


    public bool IS_HUMAN_MOVABLE
    {
        get { return _isHumanMoveable; }
        set { _isHumanMoveable = value; }
    }
    public bool IS_ALIEN_MOVABLE
    {
        get { return _isAlienMoveable; }
        set { _isAlienMoveable = value; }
    }


    void Awake()
    {
        IS_HUMAN_MOVABLE = false;
        IS_ALIEN_MOVABLE = false;
    }


    // data for the server
    public void SetCubeData(bool[] data)
    {
        IS_HUMAN_MOVABLE = data[0];
        IS_ALIEN_MOVABLE = data[1];
    }

}
