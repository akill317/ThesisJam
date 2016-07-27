using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class InputManagerScript : MonoBehaviour
{

    public Queue<int> p1Q = new Queue<int>(4);
    public Queue<int> p2Q = new Queue<int>(4);

    private int p1Qint;
    private int p1Qcheck;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R) && p1Q.Count >=1 )
        //{
        //    p1Qint = p1Q.Dequeue();
        //    Debug.Log(p1Qint);
        //}
    }

    public void inputIntoMoveQueue(int playerNum, int playerMove)
    {
        if (playerNum == 1)
        {
            p1Q.Enqueue(playerMove);
        }

        else if (playerNum == 2)
        {
            p2Q.Enqueue(playerMove);
        }
    }
}
