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

                resetFlagTimer = 0;
                if (TMS.STATE <= 4)
                {
                    TMS.onBeat(TMS.timeStamp(), 1);
                }
                //if (PhotonNetwork.offlineMode) {
                //    MoveDirection(transform.position + Vector3.up * MoveDistance);
                //} else {
                //    _photonView.RPC("MoveDirection",
                //        PhotonTargets.All,
                //        new object[] { transform.position + Vector3.up * MoveDistance });
                //}
            } else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && resetFlagTimer >= inputCoolDown) {
                resetFlagTimer = 0;
                if (TMS.STATE <= 4)
                {
                    TMS.onBeat(TMS.timeStamp(), 2);
                }
                //if (PhotonNetwork.offlineMode) {
                //    MoveDirection(transform.position + Vector3.down * MoveDistance);
                //} else {
                //    _photonView.RPC("MoveDirection",
                //        PhotonTargets.All,
                //        new object[] { transform.position + Vector3.down * MoveDistance });
                //}
            } else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && resetFlagTimer >= inputCoolDown) {
                resetFlagTimer = 0;
                if (TMS.STATE <= 4)
                {
                    TMS.onBeat(TMS.timeStamp(), 3);
                }
                //if (PhotonNetwork.offlineMode) {
                //    MoveDirection(transform.position + Vector3.left * MoveDistance);
                //} else {
                //    _photonView.RPC("MoveDirection",
                //        PhotonTargets.All,
                //        new object[] { transform.position + Vector3.left * MoveDistance });
                //}
            } else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && resetFlagTimer >= inputCoolDown) {
                resetFlagTimer = 0;
                if (TMS.STATE <= 4)
                {
                    TMS.onBeat(TMS.timeStamp(), 4);
                }
                //if (PhotonNetwork.offlineMode) {
                //    MoveDirection(transform.position + Vector3.right * MoveDistance);
                //} else {
                //    _photonView.RPC("MoveDirection",
                //        PhotonTargets.All,
                //        new object[] { transform.position + Vector3.right * MoveDistance });
                //}
            } else if ((Input.GetKeyDown(KeyCode.J)) && resetFlagTimer >= inputCoolDown) {
                resetFlagTimer = 0;
                if (TMS.STATE <= 4)
                {
                    TMS.onBeat(TMS.timeStamp(), 5);
                }
                //if (PhotonNetwork.offlineMode) {
                //    CoroutineAttack();
                //} else {
                //    _photonView.RPC("CoroutineAttack",
                //        PhotonTargets.All,
                //        new object[] { });
                //}
            }
        }
    }

    [PunRPC]
    void CoroutineAttack() {
        StartCoroutine("Attack");
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
        while (_attackAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
            if (checker.hittingOpponent) {
                _photonView.RPC("Defeat", PhotonTargets.Others, new object[] { });
                Victory();
            }
            yield return null;
        }
        _itweening = false;
    }

    [PunRPC]
    void MoveDirection(Vector3 targetPosition) {
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

    public void moveHeroLeft()
    {
        if (PhotonNetwork.offlineMode)
        {
            MoveDirection(transform.position + Vector3.left * MoveDistance);
        }
        else
        {
            _photonView.RPC("MoveDirection",
                PhotonTargets.All,
                new object[] { transform.position + Vector3.left * MoveDistance });
        }
    }

    public void moveHeroRight()
    {
        if (PhotonNetwork.offlineMode)
        {
            MoveDirection(transform.position + Vector3.right * MoveDistance);
        }
        else
        {
            _photonView.RPC("MoveDirection",
                PhotonTargets.All,
                new object[] { transform.position + Vector3.right * MoveDistance });
        }
    }

    public void moveHeroUp()
    {
        if (PhotonNetwork.offlineMode)
        {
            MoveDirection(transform.position + Vector3.up * MoveDistance);
        }
        else
        {
            _photonView.RPC("MoveDirection",
                PhotonTargets.All,
                new object[] { transform.position + Vector3.up * MoveDistance });
        }
    }

    public void moveHeroDown()
    {
        if (PhotonNetwork.offlineMode)
        {
            MoveDirection(transform.position + Vector3.down * MoveDistance);
        }
        else
        {
            _photonView.RPC("MoveDirection",
                PhotonTargets.All,
                new object[] { transform.position + Vector3.down * MoveDistance });
        }
    }

    public void moveHeroAttack()
    {
        if (PhotonNetwork.offlineMode)
        {
            CoroutineAttack();
        }
        else
        {
            _photonView.RPC("CoroutineAttack",
                PhotonTargets.All,
                new object[] { });
        }
    }

    public void nQ (int playerMove)
    {
        pQ.Enqueue(playerMove);
    }

    public void dQ (int moveNum)
    {
        if (moveNum == 1)
        {
            moveHeroUp();
        }
        if (moveNum == 2)
        {
            moveHeroDown();
        }
        if (moveNum == 3)
        {
            moveHeroLeft();
        }
        if (moveNum == 4)
        {
            moveHeroRight();
        }
        if (moveNum == 5)
        {
            moveHeroAttack();
        }
    }

    /// <summary>
    /// Below is previous use of code
    /// </summary>
    //public void moveHeroLeft() {
    //    if (!_itweening) {
    //        MoveDirection(Vector3.left);
    //        _spriteRenderer.flipX = false;
    //    }
    //}

    //public void moveHeroRight() {
    //    if (!_itweening) {
    //        MoveDirection(Vector3.right);
    //        _spriteRenderer.flipX = true;
    //    }
    //}

    //public void moveHeroUp() {
    //    if (!_itweening) {
    //        MoveDirection(Vector3.up);
    //    }
    //}

    //public void moveHeroDown() {
    //    if (!_itweening) {
    //        MoveDirection(Vector3.down);
    //    }
    //}
}
