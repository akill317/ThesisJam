using UnityEngine;
using System.Collections;

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

        //Ensure everyframe can only press one key
        if (!_itweening) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                if (PhotonNetwork.offlineMode) {
                    MoveDirection(Vector3.up);
                } else {
                    _photonView.RPC("MoveDirection",
                        PhotonTargets.All,
                        new object[] { Vector3.up });
                }
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                if (PhotonNetwork.offlineMode) {
                    MoveDirection(Vector3.down);
                } else {
                    _photonView.RPC("MoveDirection",
                        PhotonTargets.All,
                        new object[] { Vector3.down });
                }
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                if (PhotonNetwork.offlineMode) {
                    MoveDirection(Vector3.left);
                } else {
                    _photonView.RPC("MoveDirection",
                        PhotonTargets.All,
                        new object[] { Vector3.left });
                }
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                if (PhotonNetwork.offlineMode) {
                    MoveDirection(Vector3.right);
                } else {
                    _photonView.RPC("MoveDirection",
                        PhotonTargets.All,
                        new object[] { Vector3.right });
                }
            } else if (Input.GetKeyDown(KeyCode.J)) {
                if (PhotonNetwork.offlineMode) {
                    CoroutineAttack();
                } else {
                    _photonView.RPC("CoroutineAttack",
                        PhotonTargets.All,
                        new object[] { });
                }
            }
        }
    }

    [PunRPC]
    void CoroutineAttack() {
        StartCoroutine("Attack");
    }

    IEnumerator Attack() {
        _itweening = true;
        _attackAnimator.transform.position = transform.position + _facingDirection * MoveDistance + _spriteCenterOffset * Mathf.Abs(_facingDirection.y);
        _attackAnimator.transform.rotation = Quaternion.FromToRotation(Vector3.right, _facingDirection);
        _attackAnimator.Play("attack");
        while (_attackAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack")) {
            yield return null;
        }
        _itweening = false;
    }

    [PunRPC]
    void MoveDirection(Vector3 direction) {
        //Check whether there is a tile at the direciton that we are moving
        Collider2D tile = Physics2D.OverlapPoint(transform.position + direction * MoveDistance + _spriteCenterOffset);
        if (tile == null) {
            MoveFailed(direction);
            return;
        }
        //Create jump path
        Vector3[] path = new Vector3[] { transform.position, transform.position + new Vector3(direction.x * 0.5f, direction.y + 0.3f) * MoveDistance, transform.position + direction * MoveDistance };
        iTween.MoveTo(gameObject, iTween.Hash("path", path, "time", MoveTime, "easetype", MoveEaseType, "oncomplete", "MoveSuccess"));
        if (direction.x > 0) {
            _spriteRenderer.flipX = true;
        } else if (direction.x < 0) {
            _spriteRenderer.flipX = false;
        }
        _itweening = true;
        _facingDirection = direction;
    }

    void MoveSuccess() {
        _itweening = false;
    }

    void MoveFailed(Vector3 direction) {
        Vector3[] path = new Vector3[] { transform.position, transform.position + new Vector3(direction.x * 0.5f, direction.y + 0.3f) * MoveDistance, transform.position };
        iTween.MoveTo(gameObject, iTween.Hash("path", path, "time", MoveTime, "easetype", MoveEaseType, "oncomplete", "MoveSuccess"));
        _itweening = true;
    }
}
