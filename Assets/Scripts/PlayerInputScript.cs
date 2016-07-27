using UnityEngine;
using System.Collections;

public class PlayerInputScript : MonoBehaviour {

    public TimerManagerScript TMS;
    private float resetFlagTimer;
    public float inputCoolDown;
    private int playerNum;

    // Use this for initialization
    void Start() {

        if (this.gameObject.name == "Player1") {
            playerNum = 1;
        } else {
            playerNum = 2;
        }
    }

    // Update is called once per frame
    void Update() {

        resetFlagTimer += Time.deltaTime;

        //if (Input.GetKeyDown(KeyCode.Space) && resetFlagTimer >= inputCoolDown)
        //{
        //    resetFlagTimer = 0;
        //    if (TMS.STATE <= 4)
        //    {
        //        TMS.onBeat(TMS.timeStamp(), playerNum, 9);
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.W) && resetFlagTimer >= inputCoolDown) {
            moveUp();
        }
        if (Input.GetKeyDown(KeyCode.S) && resetFlagTimer >= inputCoolDown) {
            moveDown();
        }
        if (Input.GetKeyDown(KeyCode.A) && resetFlagTimer >= inputCoolDown) {
            moveLeft();
        }
        if (Input.GetKeyDown(KeyCode.D) && resetFlagTimer >= inputCoolDown) {
            moveRight();
        }
        if (Input.GetKeyDown(KeyCode.Space) && resetFlagTimer >= inputCoolDown) {
            moveAttack();
        }
    }

    void moveUp() {
        resetFlagTimer = 0;
        if (TMS.STATE <= 4) {
            TMS.onBeat(TMS.timeStamp(), playerNum, 1);
        }
    }

    void moveDown() {
        resetFlagTimer = 0;
        if (TMS.STATE <= 4) {
            TMS.onBeat(TMS.timeStamp(), playerNum, 2);
        }
    }

    void moveLeft() {
        resetFlagTimer = 0;
        if (TMS.STATE <= 4) {
            TMS.onBeat(TMS.timeStamp(), playerNum, 3);
        }
    }

    void moveRight() {
        resetFlagTimer = 0;
        if (TMS.STATE <= 4) {
            TMS.onBeat(TMS.timeStamp(), playerNum, 4);
        }
    }

    void moveAttack() {
        resetFlagTimer = 0;
        if (TMS.STATE <= 4) {
            TMS.onBeat(TMS.timeStamp(), playerNum, 5);
        }
    }
}
