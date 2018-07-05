using UnityEngine;

public class Board : MonoBehaviour {

    Slot[,] _slots = new Slot[3, 3];
    [SerializeField] float _slotSize = 2.5f;
    [SerializeField] GameObject _prefabX;
    [SerializeField] GameObject _prefabO;
    [SerializeField] GameObject _prefabSlot;
    [SerializeField] GameObject _prefabHighlighted;


    // Use this for initialization
    void Start () {

        ChallengeManager.Instance.OnChallengeTurnTakenEvent += OnChallengeTurnTaken;
        ChallengeManager.Instance.OnChallengeStartedEvent += OnChallengeStarted;
        Vector3 startingPos = Vector3.zero + Vector3.left * (_slotSize) + Vector3.down * (_slotSize);
		for(int i=0; i<3;i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector3 pos = startingPos + new Vector3(i * _slotSize, j * _slotSize, 0);

                _slots[i, j] = Instantiate(_prefabSlot).GetComponent<Slot>();
                _slots[i, j].Init(pos, i, j, _prefabX, _prefabO, _prefabSlot, _prefabHighlighted);
            }
        }
	}
	
    void OnChallengeStarted()
    {
        if (ChallengeManager.Instance.CurrentPlayerId == UIManager.Instance.LocalPlayerId)
        {
            UIManager.Instance.SetTurnMessage("Your Turn");
        }
        else
        {
            UIManager.Instance.SetTurnMessage("Opponent's Turn");
        }
        if (ChallengeManager.Instance.LocalSlotType == SlotState.O)
        {
            UIManager.Instance.SetSlotInfo(_prefabO);
        }
        else
        {
            UIManager.Instance.SetSlotInfo(_prefabX);
        }
    }

    /// <summary>
    /// A player has made a move
    /// </summary>
    void OnChallengeTurnTaken()
    {

        if(ChallengeManager.Instance.CurrentPlayerId == UIManager.Instance.LocalPlayerId)
        {
            UIManager.Instance.SetTurnMessage("Your Turn");
        }
        else
        {
            UIManager.Instance.SetTurnMessage("Opponent's Turn");
        }
        int x = ChallengeManager.Instance.MarkX;
        int y = ChallengeManager.Instance.MarkY;
        if(ChallengeManager.Instance.CurrentPlayerId == UIManager.Instance.LocalPlayerId) //if the last move was not made by this player
        {
            SlotState opponentSlotState = ChallengeManager.Instance.LocalSlotType == SlotState.O ? SlotState.X : SlotState.O;
            _slots[x, y].Mark(opponentSlotState);
            CheckForGameWin();
        }

    }


	// Update is called once per frame
	void Update () {
        if(_bGameWon)
        {
            return;
        }
        CheckForGameWin();
	}


    void CheckForGameWin()
    {
        SlotState lastSlotState = SlotState.Empty;
        int count = 1;
        int hCount = 1;
        for (int i = 0; i < 3; i++)
        {
            
            for (int j = 0; j < 3; j++)
            {
                if (_slots[i, j].slotState == SlotState.Empty)
                {
                    count = 1;
                    break;
                }
               // Debug.LogError("checking>>" + i + " " + j);
                if (j == 0)
                {
                    count = 1;
                    lastSlotState = _slots[i, j].slotState;
                }
                else
                {
                    if (_slots[i, j].slotState == lastSlotState)
                    {
                        count++;
                        if (count == 3)
                        {
                            OnGameWin(lastSlotState);
                        }
                    }
                    else
                    {
                        count = 1;
                        break;
                    }
                }
            }
            for (int j = 0; j < 3; j++)
            {
                if (_slots[j, i].slotState == SlotState.Empty)
                {
                    hCount = 1;
                    break;
                }
               // Debug.LogError("checking>>" + j + " " + i);

                if (j == 0)
                {
                    lastSlotState = _slots[j, i].slotState;
                    hCount = 1;
                }
                else
                {
                    if (_slots[j, i].slotState == lastSlotState)
                    {
                        hCount++;
                        if (hCount == 3)
                        {
                            OnGameWin(lastSlotState);
                        }
                    }
                    else
                    {
                        hCount = 1;
                        break;
                    }
                }
            }
        }
        

        lastSlotState = _slots[0, 0].slotState;
        if (lastSlotState != SlotState.Empty)
        {
            if (_slots[1, 1].slotState == lastSlotState)
            {
                if (_slots[2, 2].slotState == lastSlotState)
                {
                    OnGameWin(lastSlotState);
                }
            }
        }
        lastSlotState = _slots[2, 0].slotState;
        if (lastSlotState != SlotState.Empty)
        {
            if (_slots[1, 1].slotState == lastSlotState)
            {
                if (_slots[0, 2].slotState == lastSlotState)
                {
                    OnGameWin(lastSlotState);
                }
            }
        }
        int markedCount = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if(_slots[i, j].slotState!=SlotState.Empty)
                {
                    markedCount++;
                }
                else
                {
                    break;
                }
            }
        }
        if(markedCount == 9)
        {
            OnGameWin(SlotState.Empty);
        }
    }

    bool _bGameWon = false;
    void OnGameWin(SlotState winningSlotType)
    {
        if(_bGameWon) //if already won, return
        {
            return; 
        }
        _bGameWon = true;
        string message = "";
        if(winningSlotType == SlotState.Empty)
        {
            message = "Draw";
        }
        else if(winningSlotType == ChallengeManager.Instance.LocalSlotType)
        {
            message = "Victory!!!";
        }
        else 
        {
            message = "Defeat!";
        }
        
        UIManager.Instance.SetEndMessage(true, message);
    }
}
