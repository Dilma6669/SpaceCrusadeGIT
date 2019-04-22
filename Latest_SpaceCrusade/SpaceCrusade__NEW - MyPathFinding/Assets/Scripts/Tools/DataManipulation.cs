using System.Collections.Generic;
using UnityEngine;

public class DataManipulation : MonoBehaviour {

    public static List<Vector3> GetLocVectorsFromCubeScript(List<CubeLocationScript> cubesScripts)
    {
        List<Vector3> vects = new List<Vector3>();
        foreach (CubeLocationScript cube in cubesScripts)
        {
            Vector3 vect = cube.CubeStaticLocVector;
            vects.Add(vect);
        }
        return vects;
    }


    public static int[] ConvertVectorsIntoIntArray(List<Vector3> vects)
    {
        int[] intArray = new int[vects.Count * 3];
        int index = 0;
        foreach(Vector3 vect in vects)
        {
            intArray[index] = (int)vect.x;
            index += 1;
            intArray[index] = (int)vect.y;
            index += 1;
            intArray[index] = (int)vect.z;
            index += 1;
        }
        return intArray;
    }

    public static List<Vector3> ConvertIntArrayIntoVectors(int[] intArray)
    {
        List<Vector3> vects = new List<Vector3>();

        int index = 0;
        for(int i = 0; i < intArray.Length/3; i++)
        {
            Vector3 vect = new Vector3();

            vect.x = intArray[index];
            index += 1;
            vect.y = intArray[index];
            index += 1;
            vect.z = intArray[index];
            index += 1;

            vects.Add(vect);
        }
        return vects;
    }
}
