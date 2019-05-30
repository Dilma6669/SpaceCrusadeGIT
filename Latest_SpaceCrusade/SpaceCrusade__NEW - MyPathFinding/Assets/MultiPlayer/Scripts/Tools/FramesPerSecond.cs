using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour
{
    Text fpstext;

    //FPS stuff
    int frameCount = 0;
    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.

    void Awake()
    {
        fpstext = transform.Find("FPSNum").GetComponent<Text>();
    }

    private void Update()
    {
        CalculateFPS();
        fpstext.text = GetFPS();
    }

    private void CalculateFPS()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0f / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }
    }

    private string GetFPS()
    {
        return fps.ToString();
    }
}

