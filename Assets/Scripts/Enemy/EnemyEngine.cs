using Pathfinding;
using UnityEngine;

//the possible states for the enemy to be:
enum EnemyState { idle, follow, attack, returningToInitPos, die};

[RequireComponent(typeof(LifeEngine))]
public class EnemyEngine : MonoBehaviour
{
    [Tooltip("Distance to search for the player. when the player is in the radar distance the enemy goes to follow state")]
    public float radarDistance = 15;
    [Tooltip("When in that distance the enemy starts to hit the player")]
    public float attackDistance = 2;
    [Tooltip("if set to true, will only try to attack inside the radar distance. " +
        "if the player leaves the radar distance, it will return to the post.")]
    public bool stayInDiameter = false;
    [Tooltip("Pointer to the initial position of the enemy. will be used to calculate the diameter of the gaurdians")]
    public Transform initPosition;
    [Tooltip("The appriximate radius from the initial position which is considered ok to return to. When the gaurdian is in retrn state," +
        " he will keep going unitil he's in this radius.")]
    public float epsilonToInitPosition = 1.5f;
    //[Tooltip("How fast the enemy will walk towards the player")]
    //public float forceToWalk = 10;
    [Tooltip("The damage inflicted by the enemy on every hit")]
    public int DamageHit = 10;

    //sound effect
    public AudioSource audioSource;
    public AudioClip attectEffect;

    //the current state of the enemy. his actions are according to the state.
    private EnemyState state;
    //the Transform of the player:
    private Transform playerRef;
    //reference to the animator. for animations:
    private Animator anim;
    //the ditance between the player and the enemy. updated every frame
    private float distPlayerToEnemy;
    //distance between player and the initial position of the enemy. will be used for Gaurdians. Because when they move, their transform moves with them.
    //So we should have reference to their initial Transform, which will not change when they move:
    private float distPlayerToInitPos;
    private bool isPlayerInsideDiameter;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        //set the target of the Pathfinder to the legs of the player. it will be actived only when in 'follow' state
        transform.parent.GetComponent<AIDestinationSetter>().target = GetBottomLegsOfPlayer();
        //initial state of the enemy is 'idle':
        state = EnemyState.idle;
    }


    private void Update()
    {
        //only execute if it's not game over (if player is not dead):
        if (playerRef != null && !playerRef.GetComponent<LifeEngine>().IsDead)
        {
            UpdateEnemyState();
            FollowPlayer();
            adjustDirectionOfMove();
            AdjustAnimation();
        }
    }

    //get the bottom of the player by going over all of the children of the player which have Transform and check its tag:
    private Transform GetBottomLegsOfPlayer()
    {
        foreach (var child in playerRef.gameObject.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.tag == "BottomLegsPos")
            {
                return child;
            }
        }
        return null;
    }

    //calculate the state of the enemy every frame:
    //possible state changes:
    //
    //idle -> follow (if he is in the radar)
    //follow -> attack (if player inside attack diameter)
    //follow -> idle (if the player got out of the radar)
    //follow -> returningToInitPos (if it's a gaurdian and the player is out of the radar from its intial position)
    //follow -> die (if he got killed during the proccess)
    //attack -> follow (if player out of attack range but still in the diameter)
    //returningToInitPos -> idle (if the gaurdian is close enough to the initial position)
    private void UpdateEnemyState()
    {
        distPlayerToInitPos = Vector2.Distance(initPosition.position, GetBottomLegsOfPlayer().position);
        distPlayerToEnemy = Vector2.Distance(transform.position, GetBottomLegsOfPlayer().position);
        isPlayerInsideDiameter = (!stayInDiameter && distPlayerToEnemy < radarDistance) || (stayInDiameter && distPlayerToInitPos < radarDistance);

        if (state == EnemyState.idle && isPlayerInsideDiameter)
        {
            Debug.Log("Enemy: player is near. go to him.");
            state = EnemyState.follow;
        }
        else if (state == EnemyState.follow)
        {
            if (isPlayerInsideDiameter && (distPlayerToEnemy <= attackDistance))
            {
                Debug.Log("Enemy: Attack!");
                state = EnemyState.attack;
            }
            else if (!isPlayerInsideDiameter)
            {
                ReturnToIdlePos();
            }
            else if (GetComponent<LifeEngine>().IsDead)
            {
                state = EnemyState.die;
            }
        }
        else if (state == EnemyState.attack)
        {
            if (distPlayerToEnemy > attackDistance)
            {
                Debug.Log("Trying to follow");
                state = EnemyState.follow;
            }
        }
        else if (state == EnemyState.returningToInitPos)
        {
            if (Vector2.Distance(transform.position, initPosition.position) < epsilonToInitPosition)
            {
                state = EnemyState.idle;
            }
        }
    }

    private void ReturnToIdlePos()
    {
        if (!stayInDiameter)
        {
            Debug.Log("Enemy: minding my buisness... that's all.");
            state = EnemyState.idle;
        }
        else
        {
            state = EnemyState.returningToInitPos;
        }

    }

    //set the animation of the enemy according to the state:
    private void AdjustAnimation()
    {
        if (state == EnemyState.idle)
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isWalking", false);
        }
        else if (state == EnemyState.follow || state == EnemyState.returningToInitPos)
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isWalking", true);
        }
        else if (state == EnemyState.attack)
        {
            anim.SetBool("isAttacking", true);
            anim.SetBool("isWalking", false);



        }
    }

    //make the enemy follow the player or returning to the initial position, using A* package
    //by setting the target. If the enemy follows the player, then target is the legs of the player. If the player going the initial position if he's returning there.
    private void FollowPlayer()
    {
        //If the enemy follows the player, then target is the legs of the player
        if (state == EnemyState.follow)
        {
            //make pathfinder work.
            transform.parent.GetComponent<AIDestinationSetter>().target = GetBottomLegsOfPlayer();
            transform.parent.GetComponent<AIPath>().canMove = true;
        }
        //If the player returning to the initial position, then it's the target
        else if (state == EnemyState.returningToInitPos)
        {
            //make pathfinder work.
            transform.parent.GetComponent<AIDestinationSetter>().target = initPosition;
            transform.parent.GetComponent<AIPath>().canMove = true;
        }
        //any other case, he should not be moving and set the target to the player so he will be 'ready' if need to attck.
        else
        {
            //need to change the target to the player because next time it will move will be when the player enters
            transform.parent.GetComponent<AIDestinationSetter>().target = GetBottomLegsOfPlayer();
            transform.parent.GetComponent<AIPath>().canMove = false;


        }
    }

    //called from event in the animation:
    //reduces life of the player through its LifeEngine component:
    public void MakeHitOnPlayer()
    {
        if (playerRef != null)
        {
            //gets LifeEngine component which responsible for its life and calls TakeHit method to reduce its life:
            playerRef.GetComponent<LifeEngine>().TakeHit(DamageHit);
            Debug.Log(transform.gameObject + ": attack");

            
        }
    }

    //the enemy should look at the player.
    private void adjustDirectionOfMove()
    {
        //calculate the distance between the enemy and the player
        float distToPlayer = transform.position.x - playerRef.position.x;
        //if bigger then zero, it means the enemy is from the right:
        if (distToPlayer >= 0)
        {
            transform.localScale = new Vector3(1, 1, 1);

            //also adjust the Lifebar so it will remain from left to right.
            GetComponentInChildren<EnemyLifeBarEngine>().transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);

            //also adjust the Lifebar so it will remain from left to right.
            GetComponentInChildren<EnemyLifeBarEngine>().transform.localScale = new Vector3(1, 1, 1);
        }
    }


    //draw gizmos for the attack and radar radius:
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (!stayInDiameter)
        {
            Gizmos.DrawWireSphere(transform.position, radarDistance);
        }
        else
        {
            Gizmos.DrawWireSphere(initPosition.transform.position, radarDistance);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    //when the enemy is being destroyed after finding its collider, it has
    //to destroy the parent, which is used for the AI
    private void OnDestroy()
    {
        //Destroy(initPosition.gameObject);
        Destroy(this.transform.parent.parent.gameObject);
    }

    
}
