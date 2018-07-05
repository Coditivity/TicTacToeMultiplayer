
using Framework;
using UnityEngine;

public enum SlotState
{
    Empty,
    X,
    O
}

public class Slot : MonoBehaviour {

    public SlotState slotState = SlotState.Empty;

    Vector3 position = Vector3.zero;
    int row = 0;
    int col = 0;
    
    
    GameObject prefabX = null;
    GameObject prefabO = null;

    ObjectInteraction _oi = null;

    GameObject _blueBlock = null;
    GameObject _greenBlock = null;

    MeshRenderer _blueMesh = null;
    MeshRenderer _greenMesh = null;


    public void Init(Vector3 position, int row, int col, GameObject prefabX, GameObject prefabO, GameObject prefabEmpty
        , GameObject prefabGreen)
    {
        _blueBlock = gameObject;
        _greenBlock = Instantiate(prefabGreen);
        _greenBlock.name = "" + row + " " + col;

        _blueMesh = _blueBlock.GetComponent<MeshRenderer>();
        _greenMesh = _greenBlock.GetComponent<MeshRenderer>();
        _greenMesh.enabled = false;

        this.position = position;
        this.row = row;
        this.col = col;
        this.prefabO = prefabO;
        this.prefabX = prefabX;
        transform.position = position;
        _greenBlock.transform.position = transform.position;

        gameObject.name = "slot " + row + ", " + col;
        _oi = GetComponent<ObjectInteraction>();
        _oi.OnMouseUpEvent += ProcessMouseClick;
        _oi.OnMouseOverEvent += ProcessMouseOver;
    }

    void OnObjectHover()
    {
        _greenMesh.enabled = true;
        _blueMesh.enabled = false;
    }

    void OnObjectNotHover()
    {
        _greenMesh.enabled = false;
        _blueMesh.enabled = true;
    }

    private void OnMouseExit()
    {
        OnObjectNotHover();
    }

    void ProcessMouseOver(bool bObjectInFront)
    {
        if(bObjectInFront)
        {
            OnObjectNotHover();
        }
        else
        {
            OnObjectHover();
        }
    }

    void ProcessMouseClick()
    {
        if (slotState != SlotState.Empty)
        {
            return;
        }
        if (ChallengeManager.Instance.CurrentPlayerId != UIManager.Instance.LocalPlayerId)
        {
            return;
        }
        Mark(ChallengeManager.Instance.LocalSlotType);
        ChallengeManager.Instance.Move(row, col);
    }
    SlotState _firstSlotState = SlotState.O;

    void Mark()
    {
        
        if (_lastSlotState == SlotState.Empty)
        {
            Mark(_firstSlotState);
        }
        else if(_lastSlotState == SlotState.O)
        {
            Mark(SlotState.X);
        }
        else if (_lastSlotState == SlotState.X)
        {
            Mark(SlotState.O);
        }
        
    }

    static SlotState _lastSlotState = SlotState.Empty;
    public void Mark(SlotState state)
    {
        if(slotState != SlotState.Empty)
        {
            return;
        }
        if(_lastSlotState == state) //can't mark if already played
        {
            return;
        }
        slotState = state;
        _lastSlotState = state;
        gameObject.SetActive(false);
        _greenBlock.gameObject.SetActive(false);
        if (slotState == SlotState.O)
        {   
            GameObject obj = Instantiate(prefabO);
            obj.transform.position = position;
        }
        else if(slotState == SlotState.X)
        {
            GameObject obj = Instantiate(prefabX);
            obj.transform.position = position;
        }
        else
        {
            Debug.LogError("invalid slot value");
        }
    }

}
