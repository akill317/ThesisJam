using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class InputManagerScript : MonoBehaviour {
    public PlayerMovement Hero1;
    public PlayerMovement Hero2;

    public Queue<int> p1Q = new Queue<int>(4);
    public Queue<int> p2Q = new Queue<int>(4);

    private int p1Qint;
    private int p1Qcheck;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        //if (Input.GetKeyDown(KeyCode.R) && p1Q.Count >=1 )
        //{
        //    p1Qint = p1Q.Dequeue();
        //    Debug.Log(p1Qint);
        //}
    }

    public void inputIntoMoveQueue(int playerNum, int playerMove) {
        if (playerNum == 1) {
            p1Q.Enqueue(playerMove);
        } else if (playerNum == 2) {
            p2Q.Enqueue(playerMove);
        }
    }

    public void executeMove(int playerNum, int moveNum) {
        if (playerNum == 1) {
            if (moveNum == 1) {
                Hero1.moveHeroUp();
            }
            if (moveNum == 2) {
                Hero1.moveHeroDown();
            }
            if (moveNum == 3) {
                Hero1.moveHeroLeft();
            }
            if (moveNum == 4) {
                Hero1.moveHeroRight();
            }
        }
    }
}
