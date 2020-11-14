using UnityEngine;
using System.Collections;

public class Person : MonoBehaviour
{
    private Animator animator;
    public float influenceLevel = 0;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = Random.Range(.8f, 1f);
    }
}
