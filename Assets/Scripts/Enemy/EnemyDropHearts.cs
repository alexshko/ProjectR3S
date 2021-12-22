using UnityEngine;

//class for Droping hearts when enemies die. the hearts used to refill life.
public class EnemyDropHearts : MonoBehaviour
{
    [Tooltip("Heart prefab")]
    public Transform HeartPref;
    [Tooltip("Reference to the head of the enemy. so it will fall from its head")]
    public Transform HeadRefOfEnemy;
    [Tooltip("the force used for dropping the heart.")]
    public float droppingForce = 1000;

    //how much time the heart has been falling.
    private float timeFlying = 0;
    //reference to the created heart Transform
    private Transform heart;
    //force to apply on the heart:
    private Vector2 forceToApply;

    public void PutHeartOnGround()
    {
        if (HeartPref != null)
        {
            heart = Instantiate(HeartPref, HeadRefOfEnemy.position, HeartPref.rotation);
            //add the force on the heart so it will have a cool animation of dropping:
            timeFlying = 0;
            //should be thrown up 45 degrees and let the gravity do its magic:
            forceToApply = (new Vector2(5,5)).normalized * droppingForce;
            forceToApply += heart.GetComponent<Rigidbody2D>().mass * Physics2D.gravity;
            heart.GetComponent<Rigidbody2D>().AddForce(forceToApply);

            Debug.Log("force on heart: " + forceToApply);
        }
    }

    private void Update()
    {
        //only if the heart has been created, then apply the external force 45 degrees and gravity for 0.05 seconds
        if (heart && timeFlying <= 0.05f)
        {
            heart.GetComponent<Rigidbody2D>().AddForce(forceToApply);
            timeFlying += Time.deltaTime;
        }
        //after 0.05 seconds,  apply the gravity only for 0.45 seconds
        else if (heart && timeFlying <= 0.5f)
        {
            heart.GetComponent<Rigidbody2D>().AddForce(heart.GetComponent<Rigidbody2D>().mass * Physics2D.gravity);
            timeFlying += Time.deltaTime;
        }
    }
}
