using UnityEngine;
using System.Collections;

public class TimerManagerScript : MonoBehaviour {

    public SoundManagerScript SMS;
    //public InputManagerScript IMS;
    public PlayerMovement PM;

    private float SPB;
    public float BPM;

    public float sceneTimer;
    public float timingWindow;

    private float nextMark;

    private float coTimer;
    private int pQint;

    // numbers to track beats
    public int state;
    public int STATE {
        get {
            return state;
        }
        set {
            if (value != state) {
                state = value;
                if (value == 9)
                {
                    PM.pQ.Clear();
                }

                if (state <= 4) {
                    setTimer(Timer.timerOn);
                } else if (state > 4 && state <= 8) {
                    //Dequeues based on time, and tells PM to execute the proper move.
                    pQint = PM.pQ.Dequeue();
                    PM.dQ(pQint);
                    Debug.Log(pQint);
                }
            }
        }
    }


    // State of timer
    private delegate void TimerState();
    private TimerState timerState;

    public void setTimer(Timer TS) {
        switch (TS) {
            case (Timer.timerOn):
                coTimer = 0;
                timerState = TimerOnFunc;
                break;
            case (Timer.timerOff):
                timerState = TimerOffFunc;
                break;
        }
    }

    void Awake()
    {
        SMS = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
    }

    // Use this for initialization
    void Start() {

        SPB = 60 / BPM;
        PM.inputCoolDown = SPB * .6f;
        Debug.Log(SPB);
        setTimer(Timer.timerOff);

    }

    // Update is called once per frame
    void Update() {

        sceneTimer += Time.deltaTime;

        if (nextMark <= sceneTimer) {
            SMS.Play("blip1");
            nextMark += SPB;
            //Debug.Log(nextMark);

            STATE += 1;
            if (STATE >= 9) {
                STATE = 1;
            }
        }

        if (timerState != null) {
            timerState();
        }
    }


    void TimerOnFunc() {
        // -- coTimer is reset when the state switches.
        coTimer += Time.deltaTime;
        if (coTimer >= timingWindow) {
            // -- If timingWindow's amount of time passes after the beat, it checks to see if the Queue's registered an input.  If it didn't, it inputs a 0.
            if (PM.pQ.Count < STATE) {
                PM.nQ(0);
            }
            setTimer(Timer.timerOff);
        }
    }

    void TimerOffFunc() {

    }



    public float timeStamp() {
        return sceneTimer;
    }

    public bool onBeat(float thisTime) {
        //SMS.Play("blip2");
        if ((nextMark - thisTime) % SPB <= timingWindow / 2) {
            // Before beat, but in window
            Debug.Log("Case1: (" + nextMark + " - " + thisTime + ") % " + SPB + " = " + (nextMark - thisTime) % SPB);

            //PM.nQ(actionNum);

            //IMS.inputIntoMoveQueue(playerNum, actionNum /*placeholderforinput*/);
            return true;
        } else if ((nextMark - thisTime) % SPB >= SPB - (timingWindow / 2)) {
            // After beat, but in window
            Debug.Log("Case2: (" + nextMark + " - " + thisTime + ") % " + SPB + " = " + (nextMark - thisTime) % SPB);

            //PM.nQ(actionNum);

            //IMS.inputIntoMoveQueue(playerNum, actionNum /*placeholderforinput*/);
            return true;

        } else {
            // Out of window
            Debug.Log("Fail: (" + nextMark + " - " + thisTime + ") % " + SPB + " = " + (nextMark - thisTime) % SPB);
            //IMS.inputIntoMoveQueue(playerNum, 0 /*placeholderforinput*/);
            return false;
        }
    }

    private void StartCoTimer() {

    }

    public enum Timer { timerOn, timerOff };

}
