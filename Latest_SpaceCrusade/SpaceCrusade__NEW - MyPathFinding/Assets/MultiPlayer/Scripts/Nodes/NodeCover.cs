using UnityEngine;

public class NodeCover : MonoBehaviour {

    public BaseNode parentNode;
    GameObject selector;

    bool _active = true;

    void Awake()
    {
        selector = transform.Find("Select").gameObject;
        selector.SetActive(false);
    }

    void OnMouseDown()
    {
        _active = parentNode.ActivateMapPiece(parentNode, _active, gameObject); 
    }

    void OnMouseOver()
    {
        selector.SetActive(true);
    }

    void OnMouseExit()
    {
        selector.SetActive(false);
    }
}
