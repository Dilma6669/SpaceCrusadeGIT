using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static UIManager _instance;

    ////////////////////////////////////////////////

    static Text playerIDText;
    static Text playerNameText;
    static Text totalPlayerText;
    static Text seedNumText;

    ////////////////////////////////////////////////

    public delegate void ChangeLayerEvent(int change);
	public static event ChangeLayerEvent OnChangeLayerClick;

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

        playerIDText = transform.FindDeepChild("PlayerNum").GetComponent<Text>();
        playerNameText = transform.FindDeepChild("PlayerName").GetComponent<Text>();
        totalPlayerText = transform.FindDeepChild("TotalPlayersNum").GetComponent<Text>();
        seedNumText = transform.FindDeepChild("SeedNum").GetComponent<Text>();
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    // The players personal GUI
    public static void SetUpPlayersGUI(int playerID)
    {
        GameManager._UIManager.GetComponent<Canvas>().enabled = true;

        playerIDText.text = playerID.ToString();
        playerNameText.text = PlayerManager.PlayerName;

        SyncedVars _syncedVars = GameObject.Find("SyncedVars").GetComponent<SyncedVars>(); // needs to be here, function runs before awake
        seedNumText.text = _syncedVars.GlobalSeed.ToString();
    }

	public static void UpdateTotalPlayersGUI(int total) {

        totalPlayerText.text = total.ToString();
	}

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void ChangeLayer(bool UpDown)
    {
        if (UpDown)
        {
            if (OnChangeLayerClick != null)
                OnChangeLayerClick(1);
        }
        else
        {
            if (OnChangeLayerClick != null)
                OnChangeLayerClick(-1);
        }
    }
}
