using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProjectile : MonoBehaviour, IDecayable
{
    [SerializeField]
    private float decayTime = 10;
    [SerializeField]
    private float remainingDecayTime = 0;
    [SerializeField]
    private int penetration = 1;
    [SerializeField]
    private int damge = 20;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        remainingDecayTime = decayTime;
    }
    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update()
    {
        Decay();
    }

    // OnCollisionEnter2D is called when this collider2D/rigidbody2D has begun touching another rigidbody2D/collider2D (2D physics only)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Demon"))
        {
            Debug.Log("Hit demon");
            Demon hitDemon = collision.collider.gameObject.GetComponent<Demon>();
            if (hitDemon.Damage(damge)) 
            {
                penetration--;
                if (penetration < 0) Destroy(gameObject);
            }
        }
    }
    #region interface members
    public void Decay()
    {
        remainingDecayTime -= Time.deltaTime;
        if (remainingDecayTime < .1f) Destroy(gameObject);
    }

    public float getRemainingDecayTime()
    {
        return remainingDecayTime;
    }

    public float getDecayTime()
    {
        return decayTime;
    }
    #endregion
}
