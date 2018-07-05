using Framework;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public delegate void OnChallengeStartedListener();
public delegate void OnChallengeTurnTakenListener();
public delegate void OnChallengeWonListener();
public delegate void OnChallengeLostListener();

public class ChallengeManager : Singleton<ChallengeManager> {

    public OnChallengeStartedListener OnChallengeStartedEvent;
    public OnChallengeTurnTakenListener OnChallengeTurnTakenEvent;
    public OnChallengeWonListener OnChallengeWonEvent;
    public OnChallengeLostListener OnChallengeLostEvent;

    string _challengeId = null;

    public bool IsChallengeActive { get; set; }
    public string OPlayerName { get; set; }
    public string OPlayerId { get; set; }
    public string XPlayerName { get; set; }
    public string XPlayerId { get; set; }
    /// <summary>
    /// The player who should be making the next move
    /// </summary>
    public string CurrentPlayerId { get; set; }

    /// <summary>
    /// The slot type of the local player
    /// </summary>
    public SlotState CurrentPlayerSlotState
    {
        get
        {
            if(IsChallengeActive)
            {
                if(CurrentPlayerId == OPlayerId)
                {
                    return SlotState.O;
                }
                else
                {
                    return SlotState.X;
                }
            }
            else
            {
                return SlotState.Empty;
            }
        }
    }

    public int MarkX = 0;
    public int MarkY = 0;
    // Use this for initialization
    void Start () {
        ChallengeStartedMessage.Listener += OnChallengeStarted;
        ChallengeTurnTakenMessage.Listener += OnChallengeTurnTaken;
        ChallengeWonMessage.Listener += OnChallengeWon;
        ChallengeLostMessage.Listener += OnChallengeLost;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    SlotState SlotStateFromString(string slotName)
    {
        if(slotName == SlotState.O.ToString())
        {
            return SlotState.O;
        }
        else if(slotName == SlotState.X.ToString())
        {
            return SlotState.X;
        }
        else if(slotName == SlotState.Empty.ToString())
        {
            return SlotState.Empty;
        }
        else
        {
            Debug.LogError("Unknown slot name string");
            return SlotState.Empty;
        }
    }

    /// <summary>
    /// The slot type of the local player
    /// </summary>
    public SlotState LocalSlotType
    {
        get; private set;
    }

    /// <summary>
    /// Called when the game is ready to start. Can be used to setup the initial game state
    /// </summary>
    /// <param name="message"></param>
    void OnChallengeStarted(ChallengeStartedMessage message)
    {
        IsChallengeActive = true;
        _challengeId = message.Challenge.ChallengeId;
        OPlayerName = message.Challenge.Challenger.Name;
        OPlayerId = message.Challenge.Challenger.Id;
        XPlayerName = message.Challenge.Challenged.First().Name;
        XPlayerId = message.Challenge.Challenged.First().Id;
        CurrentPlayerId = message.Challenge.NextPlayer == OPlayerId ? OPlayerId: XPlayerId;
       
       
        string chosenPlayerId = message.Challenge.ScriptData.GetString("chosenPlayerId"); //a player is randomly chosen to make the first move
        string chosenPlayerBlock = message.Challenge.ScriptData.GetString("chosenBlockType"); //the block type (X,O) of the chosen player is randomly set

        if (chosenPlayerId == UIManager.Instance.LocalPlayerId)
        {
            LocalSlotType = SlotStateFromString(chosenPlayerBlock);
        }
        else
        {
            SlotState chosenBlockState = SlotStateFromString(chosenPlayerBlock);
            LocalSlotType = chosenBlockState == SlotState.O ? SlotState.X : SlotState.O;
        }
        OnChallengeStartedEvent.Invoke();

    }

    /// <summary>
    /// Called when a player has made their move
    /// </summary>
    /// <param name="message"></param>
    void OnChallengeTurnTaken(ChallengeTurnTakenMessage message)
    {
        CurrentPlayerId = message.Challenge.NextPlayer == OPlayerId ? OPlayerId : XPlayerId;
        MarkX = (int)message.Challenge.ScriptData.GetInt("X");
        MarkY = (int)message.Challenge.ScriptData.GetInt("Y");
        OnChallengeTurnTakenEvent.Invoke();
    }

    /// <summary>
    /// Invoked when the game is over and a player wins.
    /// </summary>
    /// <param name="message"></param>
    void OnChallengeWon(ChallengeWonMessage message)
    {
        IsChallengeActive = false;
        OnChallengeWonEvent.Invoke();
    }

    /// <summary>
    /// Invoked when the game is over and this player loses
    /// </summary>
    /// <param name="message"></param>
    void OnChallengeLost(ChallengeLostMessage message)
    {
        IsChallengeActive = false;
        OnChallengeLostEvent.Invoke();
    }

    /// <summary>
    /// Sends the move event to the server
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Move(int x, int y)
    {
        LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetChallengeInstanceId(_challengeId);
        request.SetEventKey("Move");
        request.SetEventAttribute("X", x);
        request.SetEventAttribute("Y", y);
        request.SetEventAttribute("LastPlayerId", UIManager.Instance.LocalPlayerId);
        request.Send(OnMoveSuccess, OnMoveError);

    }

    void OnMoveSuccess(LogChallengeEventResponse response)
    {

    }

    void OnMoveError(LogChallengeEventResponse response)
    {
        Debug.LogError(response.Errors.JSON.ToString());
    }
}
