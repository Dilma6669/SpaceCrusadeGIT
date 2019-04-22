using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static AnimationManager _instance;

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


    public static void SetAnimatorBool(Animator[] animators, string boolName, bool OnOff)
    {
        foreach (Animator anim in animators)
        {
            anim.SetBool(boolName, OnOff);
        }
    }

}
