using UnityEngine;
using System.Collections;

public class PlayerInputScript : MonoBehaviour {

    public TimerManagerScript TMS;
    private float resetFlagTimer;
    public float inputCoolDown;
    private int playerNum;

	// Use this for initialization
	void Start () {

        if (this.gameObject.name == "Player1")
        {
            playerNum = 1;
        }
        else
        {
            playerNum = 2;
        }
	}
	
	// Update is called once per frame
	void Update () {

        resetFlagTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && resetFlagTimer >= inputCoolDown)
        {
            resetFlagTimer = 0;
            if (TMS.STATE <= 4)
            {
                TMS.onBeat(TMS.timeStamp(), playerNum);
            }
        }

    }

    void moveUp ()
    {

    }

    void moveDown()
    {

    }

    void moveLeft ()
    {

    }

    void moveRight ()
    {

    }

    void moveAttack ()
    {

    }
}
