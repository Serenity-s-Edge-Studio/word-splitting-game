using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    [SerializeField]
    private int health = 10;
    public Vector3 TargetPos;
    public Vector3 SpawnPostion;
    public DemonManager.DemonState state = DemonManager.DemonState.TargetRandomPatrol;
    private void Awake()
    {
        SpawnPostion = transform.position;
    }
    public bool Damage(int amount)
    {
        health = Mathf.Max(0, health - amount);
        if (health == 0)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
    private void OnDestroy()
    {
        DemonManager.instance.Remove(this);
        ScoreManager.instance.DemonDispelled();
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Person"))
        {
            collision.gameObject.GetComponent<Person>().influenceLevel--;
            state = DemonManager.DemonState.ReturningFromTarget;
        }
    }

}
