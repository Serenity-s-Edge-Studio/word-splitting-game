using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public type gateType;
    public enum type
    {
        Heaven,
        Hell
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Person"))
        {
            PeopleManager.instance.Remove(collision.gameObject.GetComponent<Person>());
            Destroy(collision.gameObject);
            ScoreManager.instance.PersonReached(gateType);
        }
    }

}
