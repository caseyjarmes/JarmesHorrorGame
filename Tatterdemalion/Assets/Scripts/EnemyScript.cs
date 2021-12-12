using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State
{
    Stunned,
    Chasing,
    Searching
}

public class EnemyScript : MonoBehaviour
{
    private Rigidbody rb;
    private NavMeshAgent navMeshAgent;

    //variables
    private Animator anim;
    public Transform Player;
    public Transform Target;
    public int Speed;
    public int searchSpeed;
    public State state;
    int stunnedTimer = 0;

    //td walks between a collection of invisible game objects, stored in this array
    public GameObject[] patrolPoints;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //state machine
        switch (state)
        {
            //chasing state, runs towards player
            case State.Chasing:
                transform.LookAt(Player);
                navMeshAgent.destination = Player.position;
                break;

             //searching state, selects random patrol point then walks to it
            case State.Searching:

                //walk towards target
                transform.LookAt(Target);
                navMeshAgent.destination = Target.position;

                //change patrol point when get close to one
                if (Vector3.Distance(Target.position, transform.position) < 5)
                {
                    chosePatrolPoint();
                }

                //switch to chasing if near player
                if (Vector3.Distance(Player.position, transform.position) < 10)
                {
                    //this is for animation
                    anim.SetBool("stunned", false);
                    anim.SetBool("searching", false);
                    anim.SetBool("chasing", true);

                    state = State.Chasing;
                    navMeshAgent.speed = Speed;
                }

                break;

            //activated by player flashlight, cannot move for a few seconds
            case State.Stunned:

                //for animation
                if (stunnedTimer == 0)
                {
                    anim.SetBool("stunned", true);
                }

                stunnedTimer++;

                //go back to searching
                if (stunnedTimer == 200)
                {
                    stunnedTimer = 0;

                    anim.SetBool("stunned", false);
                    anim.SetBool("chasing", false);
                    anim.SetBool("searching", true);
                    state = State.Searching;
                    navMeshAgent.speed = searchSpeed;
                }
                break;

            default:
                break;
        }
    }

    //choses random patrol point
    void chosePatrolPoint()
    {
        int r = Random.Range(0, patrolPoints.Length);
        Target = patrolPoints[r].transform;
    }
}
