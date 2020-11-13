using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    [SerializeField]
    private int health = 10;
    public Vector2Int Speed;
    public Vector3 SpawnPostion;
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
        DemonManager.instance.remove(this);
    }
}
