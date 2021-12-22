using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeBar : MonoBehaviour
{
    //Reference to the player:
    private Transform playerRef;
    //reference to the scrollbar:
    private Scrollbar scrollBarRef;
    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        scrollBarRef = GetComponent<Scrollbar>();
    }
    // Update is called once per frame
    void Update()
    {
        //if the player is not destroyed:
        if (playerRef != null)
        {
            //update the scroll bar
            LifeEngine playerLife = playerRef.GetComponent<LifeEngine>();
            //the current life of the player might be negative if the last hit had bigger damage than the life.
            //the size should always be between 0 and 1.
            scrollBarRef.size = Mathf.Clamp01(playerLife.lifeLeft/playerLife.initialLife);
        }
    }
}
