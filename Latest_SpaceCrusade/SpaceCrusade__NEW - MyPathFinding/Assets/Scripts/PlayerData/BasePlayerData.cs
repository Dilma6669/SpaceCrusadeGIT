using System.Collections.Generic;
using UnityEngine;

public class BasePlayerData
{
    public string name;
    public List<int[,]> smallShipFloorsPART1 = new List<int[,]>();
    public List<int[,]> smallShipFloorsPART2 = new List<int[,]>();
    public List<int[,]> smallShipVentsPART1 = new List<int[,]>();
    public List<int[,]> smallShipVentsPART2 = new List<int[,]>();

    public int numUnits;

    public List<UnitData> allUnitData = new List<UnitData>(){};
}

