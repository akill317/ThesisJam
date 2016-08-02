using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

    public float MoveDistance;
    public float MoveTime;
    public iTween.EaseType MoveEaseType;

    bool _itweening;
    Vector3 _spriteCenterOffset;
    Animator _attackAnimator;
    SpriteRenderer _spriteRenderer;
    Vector3 _facingDirection = Vector3.right;
    PhotonView _photonView;


    // -- Timer and Queuing
    public TimerManagerScript TMS;
    public Queue<int> pQ = new Queue<int>(4);
    // Anti-Key Spamming Timer
    public float inputCoolDown; // Set by timer for 60% of SPB
    private float resetFlagTimer;


    // Use this for initialization
    void Start() {
        _attackAnimator = transform.FindChild("Attack").GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _photonView = GetComponent<PhotonView>();
        _spriteCenterOffset = Vector3.down * 0.5f;
        if(transform.rotation.eulerAngles.y == 0) {
            transform.rotation = Quaternion.identity;
            SetFacingDirection(Vector3.right);
        }else {
            transform.rotation = Quaternion.identity;
            SetFacingDirection(Vector3.left);
        }
        ChangeHeroColor();
    }

    // Update is called once per frame
    void Update() {

        if (!_photonView.isMine) {
            return;
        }

        resetFlagTimer += Time.deltaTime;

        //Ensure everyframe can only press one key
        if (!_itweening) {

            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && resetFlagTimer >= inputCoolDown) {
                if (TMS.STATE <= 4 && TMS.onBeat(TMS.timeStamp())) {

                    resetFlagTimer = 0;
                    if (PhotonNetwork.offlineMode) {
                        nQ(1);
                    } else {
                        _photonView.RPC("nQ",
                            PhotonTargets.All,
                            new object[] { 1 });
                    }
                }
            } else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && resetFlagTimer >= inputCoolDown) {
                if (TMS.STATE <= 4 && TMS.onBeat(TMS.timeStamp())) {
                    resetFlagTimer = 0;
                    if (PhotonNetwork.offlineMode) {
                        nQ(2);
                    } else {
                        _photonView.RPC("nQ",
                            PhotonTargets.All,
                            new object[] { 2 });
                    }
                }
            } else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && resetFlagTimer >= inputCoolDown) {
                if (TMS.STATE <= 4 && TMS.onBeat(TMS.timeStamp())) {
                    resetFlagTimer = 0;
                    if (PhotonNetwork.offlineMode) {
                        nQ(3);
                    } else {
                        _photonView.RPC("nQ",
                            PhotonTargets.All,
                            new object[] { 3 });
                    }
                }
            } else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && resetFlagTimer >= inputCoolDown) {
                if (TMS.STATE <= 4 && TMS.onBeat(TMS.timeStamp())) {
                    resetFlagTimer = 0;
                    if (PhotonNetwork.offlineMode) {
                        nQ(4);
                    } else {
                        _photonView.RPC("nQ",
                            PhotonTargets.All,
                            new object[] { 4 });
                    }
                }
            } else if ((Input.GetKeyDown(KeyCode.J)) && resetFlagTimer >= inputCoolDown) {
                if (TMS.STATE <= 4 && TMS.onBeat(TMS.timeStamp())) {
                    resetFlagTimer = 0;
                    if (PhotonNetwork.offlineMode) {
                        nQ(5);
                    } else {
                        _photonView.RPC("nQ",
                            PhotonTargets.All,
                            new object[] { 5 });
                    }
                }
            }
        }
    }

    [PunRPC]
    void CoroutineAttack() {
        if (!_itweening) {
            StartCoroutine(Attack());
        }
    }

    [PunRPC]
    void Defeat() {
        Debug.Log("You lost");
    }

    void Victory() {
        Debug.Log("You win!!");
    }

    IEnumerator Attack() {
        AttackCheck checker = _attackAnimator.GetComponent<AttackCheck>();
        _itweening = true;
        _attackAnimator.transform.position = transform.position + _facingDirection * MoveDistance + _spriteCenterOffset * Mathf.Abs(_facingDirection.y);
        _attackAnimator.transform.rotation = Quaternion.FromToRotation(Vector3.right, _facingDirection);
        _attackAnimator.Play("attack");
        float t = 0;
        while(t < _attackAnimator.GetCurrentAnimatorStateInfo(0).length) {
        	t += Time.deltaTime;
			if (checker.hittingOpponent) {
                _photonView.RPC("Defeat", PhotonTargets.Others, new object[] { });
                Victory();
            }
            yield return null;
        }
        _itweening = false;
    }

    void MoveDirection(Vector3 targetPosition) {
        if (!_itweening) {
            Vector3 direction = targetPosition - transform.position;
            //Check whether there is a tile at the direciton that we are moving
            Collider2D tile = Physics2D.OverlapPoint(targetPosition + _spriteCenterOffset, LayerMask.GetMask(new string[] { "Tile" }));
            if (tile == null) {
                MoveFailed(direction);
                return;
            }
            //Create jump path
            Vector3[] path = new Vector3[] { transform.position, transform.position + new Vector3(direction.x * 0.5f, direction.y + 0.3f) * MoveDistance, targetPosition };
            iTween.MoveTo(gameObject, iTween.Hash("path", path, "time", MoveTime, "easetype", MoveEaseType, "oncomplete", "MoveSuccess"));
            SetFacingDirection(direction);
            _itweening = true;
        }
    }

    void MoveSuccess() {
        _itweening = false;
    }

    void MoveFailed(Vector3 direction) {
        Vector3[] path = new Vector3[] { transform.position, transform.position + new Vector3(direction.x * 0.5f, direction.y + 0.3f) * MoveDistance, transform.position };
        iTween.MoveTo(gameObject, iTween.Hash("path", path, "time", MoveTime, "easetype", MoveEaseType, "oncomplete", "MoveSuccess"));
        _itweening = true;
    }

    public void SetFacingDirection(Vector3 direction) {
        if (_spriteRenderer == null) {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (direction.x > 0) {
            _spriteRenderer.flipX = true;
        } else if (direction.x < 0) {
            _spriteRenderer.flipX = false;
        }
        _facingDirection = direction;
    }


    public void moveHeroRight() {
        if (PhotonNetwork.offlineMode) {
            MoveDirection(transform.position + Vector3.right * MoveDistance);
        } else {
            _photonView.RPC("MoveDirection",
                PhotonTargets.All,
                new object[] { transform.position + Vector3.right * MoveDistance });
        }
    }

    public void moveHeroUp() {
        if (PhotonNetwork.offlineMode) {
            MoveDirection(transform.position + Vector3.up * MoveDistance);
        } else {
            _photonView.RPC("MoveDirection",
                PhotonTargets.All,
                new object[] { transform.position + Vector3.up * MoveDistance });
        }
    }

    public void moveHeroDown() {
        if (PhotonNetwork.offlineMode) {
            MoveDirection(transform.position + Vector3.down * MoveDistance);
        } else {
            _photonView.RPC("MoveDirection",
                PhotonTargets.All,
                new object[] { transform.position + Vector3.down * MoveDistance });
        }
    }

    public void moveHeroAttack() {
        if (PhotonNetwork.offlineMode) {
            CoroutineAttack();
        } else {
            _photonView.RPC("CoroutineAttack",
                PhotonTargets.All,
                new object[] { });
        }
    }

    [PunRPC]
    public void nQ(int playerMove) {
        pQ.Enqueue(playerMove);
    }

    public void dQ(int moveNum) {
        if (moveNum == 1) {
            MoveDirection(transform.position + Vector3.up * MoveDistance);
        }
        if (moveNum == 2) {
            MoveDirection(transform.position + Vector3.down * MoveDistance);
        }
        if (moveNum == 3) {
            MoveDirection(transform.position + Vector3.left * MoveDistance);
        }
        if (moveNum == 4) {
            MoveDirection(transform.position + Vector3.right * MoveDistance);
        }
        if (moveNum == 5) {
            CoroutineAttack();
        }
    }

    public void ChangeHeroColor() {
        if (_photonView == null) {
            _photonView = GetComponent<PhotonView>();
        }
        if (_photonView.isMine) {
            if (PhotonNetwork.offlineMode) {
                PhotonChangeHeroColor(GameManager.Instance.PlayerColor);
            } else {
                _photonView.RPC("PhotonChangeHeroColor", PhotonTargets.All, new object[] { GameManager.Instance.PlayerColor });
            }
        }
    }

    [PunRPC]
    void PhotonChangeHeroColor(int colorIndex) {
        if (_spriteRenderer == null) {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        _spriteRenderer.color = IndexToColor(colorIndex);
    }

    public static Color IndexToColor(int colorIndex) {
        Color color = Color.white;
        switch (colorIndex) {
            case 0:
                color = Color.white;
                break;
            case 1:
                color = new Color(1, 0.3f, 0.3f);
                break;
            case 2:
                color = new Color(0.1f, 0.4f, 1);
                break;
            case 3:
                color = new Color(0.1f, 1, 0.3f);
                break;
            case 4:
                color = new Color(1, 0.95f, 0.2f);
                break;
        }
        return color;
    }
}
