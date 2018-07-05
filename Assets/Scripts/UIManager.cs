using Framework;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    [SerializeField]
    InputField inputFieldUserName = null;
    [SerializeField]
    InputField inputFieldPassword = null;
    [SerializeField]
    Text _textError = null;
    [SerializeField] GameObject _panelBlocker;
    [SerializeField] GameObject _panelLogin;

    [SerializeField] GameObject _panelEndMessage;
    [SerializeField] Text _textEndMessage;
    // Use this for initialization
    void Start() {
        MatchNotFoundMessage.Listener += OnMatchNotFound;
        ChallengeStartedMessage.Listener += OnChallengeStarted;

    }

    // Update is called once per frame
    void Update() {

    }


    public void OnClickRegister()
    {
        BlockInput();
        RegistrationRequest request = new RegistrationRequest();
        request.SetUserName(inputFieldUserName.text);
        request.SetDisplayName(inputFieldUserName.text);
        request.SetPassword(inputFieldPassword.text);
        request.Send(OnRegistrationSuccess, OnRegistrationError);
    }

    public void OnClickLogin()
    {
        BlockInput();
        AuthenticationRequest request = new AuthenticationRequest();
        request.SetUserName(inputFieldUserName.text);
        request.SetPassword(inputFieldPassword.text);
        request.Send(OnLoginSuccess, OnLoginError);
    }

    public GameObject panelFindMatch;
    public Text textMatchFindMessage;
    public void OnClickFindMatch()
    {
        MatchmakingRequest request = new MatchmakingRequest();
        request.SetMatchShortCode("defMatch");
        request.SetSkill(0);
        request.Send(OnMatchMakingSuccess, OnMatchMakingError);
        BlockInput();
    }

    void OnMatchMakingSuccess(MatchmakingResponse response)
    {

        textMatchFindMessage.text = response.JSONString;
    }

    void OnMatchNotFound(MatchNotFoundMessage message)
    {
        UnblockInput();
        textMatchFindMessage.text = message.JSONString;
    }

    void OnChallengeStarted(ChallengeStartedMessage message)
    {
        UnblockInput();
        panelFindMatch.SetActive(false);
        _textError.gameObject.SetActive(false);
    }

    void OnMatchMakingError(MatchmakingResponse response)
    {
        UnblockInput();
        textMatchFindMessage.text = response.JSONString;

    }
    void BlockInput()
    {
        _panelBlocker.SetActive(true);
    }

    void UnblockInput()
    {
        _panelBlocker.SetActive(false);
    }

    public string LocalPlayerId { get; set; }
    void OnLoginSuccess(AuthenticationResponse response)
    {
        UnblockInput();
        LocalPlayerId = response.UserId;
        _panelLogin.SetActive(false);


    }

    void OnLoginError(AuthenticationResponse response)
    {
        UnblockInput();
        _textError.text = response.Errors.JSON.ToString();
    }

    void OnRegistrationSuccess(RegistrationResponse response)
    {
        UnblockInput();
        OnClickLogin();
    }

    void OnRegistrationError(RegistrationResponse response)
    {
        UnblockInput();
        _textError.text = response.Errors.JSON.ToString();
    }

    public void SetEndMessage(bool bPanelActive, string message)
    {
        _panelEndMessage.SetActive(bPanelActive);
        _textEndMessage.text = message;
    }

    [SerializeField] Text _textTurnMessage = null;

    public void SetTurnMessage(string message)
    {
        _textTurnMessage.text = message;
    }

    
    [Tooltip("The position of the icon indicating the local block type")]
    /// <summary>
    /// The position of the icon indicating the local block type
    /// </summary>
    [SerializeField] Transform _localSlotInfoPos;
    public void SetSlotInfo(GameObject prefabLocalSlot)
    {
        GameObject obj = Instantiate(prefabLocalSlot);
        obj.transform.position = _localSlotInfoPos.position;
        obj.transform.localScale = new Vector3(40, 40, 10);
       // obj.transform.localScale = Vector3.one * 45;
    }
    
}
