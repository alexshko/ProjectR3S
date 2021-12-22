using UnityEngine;

public class LifeEngine : MonoBehaviour
{
    [Tooltip("Amount of initial life the player/enemy has")]
    public float initialLife = 100;
    [Tooltip("how long to wait when destroying the player/enemy. So all the animations will end.")]
    public float timeToDestroyWhenDie = 3;

    [Tooltip("reference to effect of stars on hit.")]
    public Transform effectStarsOnHitRef;
    //[Tooltip("How long unitil destroying the hit effect.")]
    //public float hitEffectDuration = 1.2f;

    //how much life left:
    public float lifeLeft { get; set; }

    //is player/enemy dead:
    private bool isDead;
    public bool IsDead
    {
        get => isDead;
    }

    private void Start()
    {
        lifeLeft = initialLife;
        isDead = false;
    }

    //call this function to take life from the player/enemy.
    public void TakeHit(float damage)
    {
        if (!isDead)
        {
            Debug.Log(transform.gameObject.name + ": Ouch.");
            //reduce the damage:
            lifeLeft -= damage;
            //if it has Lifebar (currently only enemies) then update it:
            AdjustLifeBarIfExist();
            //play effect on hit.
            PlayHitEffect();

            //the animator of the player/enemy has to have Hit trigger for animation of being hit.
            GetComponent<Animator>().SetTrigger("Hit");

            //if after taking the hit, 0 or less life remains, it means the character is dead:
            if (lifeLeft <= 0)
            {
                //the animator of the player/enemy has to have Die trigger for animation of being hit.
                GetComponent<Animator>().SetTrigger("Die");

                //if the enemy has ability to drop hearts, then activate it.
                //Calculated by checking if it has EnemyDropHearts component
                if (GetComponent<EnemyDropHearts>() != null)
                {
                    GetComponent<EnemyDropHearts>().PutHeartOnGround();
                }

                //destroy the object after a while. the time should be bigger then the time required for the animation.
                Destroy(gameObject, timeToDestroyWhenDie);
                //mark it as dead.
                isDead = true;
            }
        }
    }

    private void PlayHitEffect()
    {
        if (effectStarsOnHitRef != null)
        {
            Vector2 headOfPerson = FindHeadOfPerson();
            Transform effect = Instantiate(effectStarsOnHitRef, headOfPerson, effectStarsOnHitRef.transform.rotation);
        }
    }

    private Vector2 FindHeadOfPerson()
    {
        foreach (var item in transform.GetComponentsInChildren<Transform>())
        {
            if (item.gameObject.name == "Head")
            {
                return item.position;
            }
        }
        return Vector2.zero;
    }

    private void AdjustLifeBarIfExist()
    {
        //if there is lifebar, update it:
        EnemyLifeBarEngine lBar = GetComponentInChildren<EnemyLifeBarEngine>();
        if (lBar != null) { 
            lBar.lifeBarValue = Mathf.Max(0,lifeLeft) / initialLife; 
        }
    }
}
