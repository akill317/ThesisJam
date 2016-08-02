using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInRoomManager : MonoBehaviour {

    public Button WhiteButton;
    public Button RedButton;
    public Button BlueButton;
    public Button GreenButton;
    public Button YellowButton;

    public Button ReadyButton;

    SpriteRenderer _spriteRenderer;
    Button[] _colorButton;
    PhotonView _photonView;
    Animator _animator;
    Light _spotLight;
    Transform _playerReadyPanel;
    // Use this for initialization
    void Start() {
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        _spotLight = GetComponentInChildren<Light>();
        _playerReadyPanel = GameObject.Find("PlayerReadyPanel").transform;
        NetworkManager.Instance.OnPlayerLeftRoom += OnPlayerLeft;
        GetAndSetButton();
        SetPlayerDefault();
    }

    void GetAndSetButton() {
        WhiteButton = GameObject.Find("WhiteButton").GetComponent<Button>();
        RedButton = GameObject.Find("RedButton").GetComponent<Button>();
        BlueButton = GameObject.Find("BlueButton").GetComponent<Button>();
        GreenButton = GameObject.Find("GreenButton").GetComponent<Button>();
        YellowButton = GameObject.Find("YellowButton").GetComponent<Button>();
        ReadyButton = GameObject.Find("ReadyButton").GetComponent<Button>();

        _colorButton = new Button[5];
        _colorButton[0] = WhiteButton;
        _colorButton[1] = RedButton;
        _colorButton[2] = BlueButton;
        _colorButton[3] = GreenButton;
        _colorButton[4] = YellowButton;

        WhiteButton.onClick.AddListener(() => ColorButtonClicked(WhiteButton.name));
        RedButton.onClick.AddListener(() => ColorButtonClicked(RedButton.name));
        BlueButton.onClick.AddListener(() => ColorButtonClicked(BlueButton.name));
        GreenButton.onClick.AddListener(() => ColorButtonClicked(GreenButton.name));
        YellowButton.onClick.AddListener(() => ColorButtonClicked(YellowButton.name));

        ReadyButton.onClick.AddListener(() => GetReady());
    }

    void SetPlayerDefault() {
        if (_spriteRenderer == null) {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        _spriteRenderer.color = Color.white;
        GameManager.Instance.PlayerColor = 0;
        WhiteButton.interactable = false;
    }

    void ColorButtonClicked(string buttonName) {
        foreach (Button btn in _colorButton) {
            btn.interactable = true;
        }
        Button clickedButton = StringToButton(buttonName);
        clickedButton.interactable = false;
        GameManager.Instance.PlayerColor = ButtonNameToIndex(clickedButton.name);
        if (!_photonView.isMine) {
            return;
        }
        if (_photonView.isMine) {
            if (PhotonNetwork.offlineMode) {
                ChangeHeroColor(buttonName);
            } else {
                _photonView.RPC("ChangeHeroColor",
                                PhotonTargets.All,
                                new object[] { buttonName });
            }
        }
    }

    [PunRPC]
    void ChangeHeroColor(string buttonName) {
        _spriteRenderer.color = PlayerMovement.IndexToColor(ButtonNameToIndex(buttonName));
    }

    void OnPlayerLeft(int playerId) {
        if (PhotonNetwork.player.ID > playerId) {
            PhotonNetwork.player.InternalChangeLocalID(PhotonNetwork.player.ID - 1);
            if (_photonView.isMine) {
                if (PhotonNetwork.offlineMode) {
                    ChangePlayerIdAndPosition();
                } else {
                    _photonView.RPC("ChangePlayerIdAndPosition",
                                    PhotonTargets.All,
                                    new object[] { });
                }
            }
        }
    }

    [PunRPC]
    void ChangePlayerIdAndPosition() {
        transform.parent = _playerReadyPanel.GetChild(PhotonNetwork.player.ID);
        transform.localPosition = Vector3.zero;
    }

    Button StringToButton(string buttonName) {
        Button btn = WhiteButton;
        switch (buttonName) {
            case "WhiteButton":
                btn = WhiteButton;
                break;
            case "RedButton":
                btn = RedButton;
                break;
            case "BlueButton":
                btn = BlueButton;
                break;
            case "GreenButton":
                btn = GreenButton;
                break;
            case "YellowButton":
                btn = YellowButton;
                break;
        }
        return btn;
    }

    int ButtonNameToIndex(string buttonName) {
        int index = 0;
        if (buttonName == "WhiteButton") {
            index = 0;
        } else if (buttonName == "RedButton") {
            index = 1;
        } else if (buttonName == "BlueButton") {
            index = 2;
        } else if (buttonName == "GreenButton") {
            index = 3;
        } else if (buttonName == "YellowButton") {
            index = 4;
        }
        return index;
    }

    void GetReady() {
        ReadyButton.interactable = false;
        ReadyButton.GetComponentInChildren<Text>().text = "Wait Other";
        if (_photonView.isMine) {
            if (PhotonNetwork.offlineMode) {
                PhotonGetReady();
            } else {
                _photonView.RPC("PhotonGetReady",
                                PhotonTargets.All,
                                new object[] { });
            }
        }
    }

    [PunRPC]
    void PhotonGetReady() {
        _animator.enabled = true;
        _spotLight.spotAngle = 30;
        GameManager.Instance.ReadyPlayerNumber += 1;
        if (GameManager.Instance.ReadyPlayerNumber == PhotonNetwork.room.playerCount && PhotonNetwork.room.playerCount >= 1) {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame() {
        ReadyButton.GetComponentInChildren<Text>().text = "3";
        yield return new WaitForSeconds(1);
        ReadyButton.GetComponentInChildren<Text>().text = "2";
        yield return new WaitForSeconds(1);
        ReadyButton.GetComponentInChildren<Text>().text = "1";
        yield return new WaitForSeconds(1);
        if (PhotonNetwork.isMasterClient) {
            PhotonNetwork.LoadLevel("Game");
        }
    }
}
