using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Builder_Grid : MonoBehaviour
{
    public GameObject GridContainer;

    public List<List<List<GameObject>>> yObjects;

    public List<int[]> dataArrayList;

    public TextAsset TextFile;

    void Start()
    {
        PutGridIntoLists();

        GetGridInteriorData();

        SaveDataArrayIntoDoc();

        print("FINISHED!!!");

    }

    //////////////////////////////////////////////

    private void PutGridIntoLists()
    {
        yObjects = new List<List<List<GameObject>>>();
        foreach (Transform yChild in GridContainer.transform)
        {
            List<List<GameObject>> zObjects = new List<List<GameObject>>();
            foreach (Transform zChild in yChild.gameObject.transform)
            {
                List<GameObject> xObjects = new List<GameObject>();
                foreach (Transform xChild in zChild.gameObject.transform)
                {
                    xObjects.Add(xChild.gameObject);
                }
                zObjects.Add(xObjects);
            }
            yObjects.Add(zObjects);
        }
    }

    //////////////////////////////////////////////

    private void GetGridInteriorData()
    {
        dataArrayList = new List<int[]>();

        foreach (List<List<GameObject>> yChild in yObjects)
        {
            foreach (List<GameObject> zChild in yChild)
            {
                foreach (GameObject xChild in zChild)
                {
                    CubeData cubeData = GetCubeData(xChild);

                    int[] dataArray = ConvertCubeDataIntoIntArray(cubeData);

                    dataArrayList.Add(dataArray);
                }
            }
        }
    }

    //////////////////////////////////////////////

    private CubeData GetCubeData(GameObject xChild)
    {
        CubeData data;

        if (xChild.transform.childCount == 0)
        {
            return data = new CubeData()
            {
                styleType = CubeObjectStyles.Default,
                health = 100,
                objectType = CubeObjectTypes.Empty,
                rotation = Vector3.zero
            };
        }
        else
        {
            GameObject cubeObject = xChild.transform.GetChild(0).gameObject;

            return data = new CubeData()
            {
                styleType = cubeObject.GetComponent<CubeObjectScript>().cubeStyle,
                health = 100,
                objectType = cubeObject.GetComponent<CubeObjectScript>().cubeType,
                rotation = (Vector3)cubeObject.transform.rotation.eulerAngles
            };
        }
    }

    //////////////////////////////////////////////

    private int[] ConvertCubeDataIntoIntArray(CubeData cubeData)
    {
        int[] dataArray = new int[6];

        dataArray[0] = (int)cubeData.styleType;
        dataArray[1] = (int)cubeData.health;
        dataArray[2] = (int)cubeData.objectType;
        dataArray[3] = (int)cubeData.rotation.x;
        dataArray[4] = (int)cubeData.rotation.y;
        dataArray[5] = (int)cubeData.rotation.z;

    return dataArray;
    }

    //////////////////////////////////////////////

    private void SaveDataArrayIntoDoc()
    {
        string path = "Assets/Builders/Data/testDoc.txt";

        File.WriteAllText(path, "");

        if (File.Exists(path))
        {
            foreach (int[] dataArray in dataArrayList)
            {
                File.AppendAllText(path, dataArray[0] + " " + dataArray[1] + " " + dataArray[2] + " " + dataArray[3] + " " + dataArray[4] + " " + dataArray[5] + "\n");
            }
        }
    }

    //////////////////////////////////////////////

    private int[,] LoadDataFromDoc()
    {
        int width = 27;

        string[] words;
        int[,] array = new int[width, width];

        if (TextFile != null)
        {
            // Add each line of the text file to
            // the array using a space
            // as the delimiter
            words = (TextFile.text.Split(' '));

            int row = -1, column = -1;
            for (int i = 0; i < words.Length; i++)
            {
                row = (int)(i / width);
                column = (int)(i % width);
                array[row, column] = int.Parse(words[i]);
            }
        }
        return array;
    }

}