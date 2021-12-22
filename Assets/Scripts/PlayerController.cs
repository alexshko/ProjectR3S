using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    float speed = 5;
    
    float x;
    float y;
    Animator anm;
    

    public float attackRadius = 0.5f;

    public float physics2d;


    //Sound effect
    public AudioSource AudioSource;
    public AudioClip attectEffect;
    

    //affect for taking hearts:
    public Transform TakeHeartAffectRef;

    // Start is called before the first frame update
    void Start()
    {
        speed = 5f;

        anm = GetComponent<Animator>();

        // Jumping
        rb = GetComponent<Rigidbody2D>();

        

    }

    // Update is called once per frame
    void Update()
    {
        
        //Move Player

            x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
       // transform.Translate(x, 0, 0);

        y = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        //transform.Translate(0, y, 0);

        
        // Move Player Up Down
        if(Input.GetAxis("Horizontal" )!=0 && (Input.GetAxis("Vertical")!=0))
        {
            x = x / Mathf.Sqrt(2);
            y = y / Mathf.Sqrt(2);

        }
        transform.Translate(0, y, 0);
        transform.Translate(x, 0, 0);

        // Animation
        if ((x == 0) && (y==0))
        {
            anm.SetBool("isWalking", false);

        }
        else if (x > 0) // right
        {
            anm.SetBool("isWalking", true);
            transform.localScale = new Vector3(4, transform.localScale.y, transform.localScale.z);

        }
        else // left
        {
            anm.SetBool("isWalking", true);
            transform.localScale = new Vector3(-4, transform.localScale.y, transform.localScale.z);
        }
        // Attacing
        if (Input.GetKeyDown(KeyCode.Space) == true) 
        {
            
            anm.SetTrigger("isAttacing1");
            Collider2D[] objectsIntersect = Physics2D.OverlapCircleAll(transform.position, attackRadius);

            // //Sound effect
            AudioSource.PlayOneShot(attectEffect);
            anm.SetTrigger("isAttacing1");


            // Take Hit Demege
            foreach (Collider2D item in objectsIntersect)
            {
                Debug.Log("attect" + item.name);
                if(item.tag == "Enemy")
                {
                    item.GetComponent<LifeEngine>().TakeHit(20);
                }
                    
            }
            
            

        }  

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Called trigger");
        if (collision.gameObject.tag== "Heart")
        {
            Debug.Log("PictHeart");

            //Player life Left
            GetComponent<LifeEngine>().lifeLeft = GetComponent<LifeEngine>().initialLife;
            Destroy(collision.gameObject);

            //make effect for taking the heart:
            Transform eff = Instantiate(TakeHeartAffectRef, transform.position, TakeHeartAffectRef.rotation);
            //Destroy(eff, 1);
           

        }       

    }

    private void OnDestroy()
    {
        if (GetComponent<LifeEngine>().IsDead)
        {
            SceneManager.LoadScene("GameOverNew");
        }
    }

    

}