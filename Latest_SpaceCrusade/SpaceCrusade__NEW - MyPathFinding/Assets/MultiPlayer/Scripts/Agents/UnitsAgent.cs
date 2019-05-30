using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class UnitsAgent : NetworkBehaviour
{
    public List<UnitScript> _unitObjects = new List<UnitScript>();

    public void AddUnitToUnitAgent(UnitScript unit)
    {
        _unitObjects.Add(unit);
    }
}
