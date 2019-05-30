using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{

    public void LoadSinglePlayerScene(int scene)
    {
        switch (scene)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                break;

        }
    }


    public void LoadMultiPlayerScene(int scene)
    {
        switch (scene)
        {
            case 0:
                SceneManager.LoadScene("Assets/Shared/Scenes/MultiPlayerScene_0.unity");
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                break;

        }
    }


}
