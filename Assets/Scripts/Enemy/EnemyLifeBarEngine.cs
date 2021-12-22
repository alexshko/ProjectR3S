using UnityEngine;

public class EnemyLifeBarEngine : MonoBehaviour
{
    public Transform LifeBarSprite;

    float lifeBarVal;

    public float lifeBarValue
    {
        get => lifeBarVal;
        set
        {

            //adjust the scale.x of the lifebar. values between 0 and 1.
            lifeBarVal = value;
            LifeBarSprite.localScale = new Vector3(value, LifeBarSprite.localScale.y, LifeBarSprite.localScale.z);
        }
    }
}
