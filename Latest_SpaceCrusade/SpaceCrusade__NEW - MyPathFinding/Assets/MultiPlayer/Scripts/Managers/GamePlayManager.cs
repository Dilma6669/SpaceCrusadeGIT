using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour {

    GameManager _gameManager;

    [HideInInspector]
    public LocationManager _locationManager;
    [HideInInspector]
    public MovementManager _movementManager;
    [HideInInspector]
    public CombatManager _combatManager;

	void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }


        _locationManager = GetComponentInChildren<LocationManager>();
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _movementManager = GetComponentInChildren<MovementManager> ();
		if(_movementManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_combatManager = GetComponentInChildren<CombatManager> ();
		if(_combatManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}


}
