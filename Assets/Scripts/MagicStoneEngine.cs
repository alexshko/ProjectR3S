using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MagicStoneEngine : MonoBehaviour
{
    [Tooltip("prefab of the effect of taking stone of magic.")]
    public Transform TakeStoneEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the one who's stepped on it is the player, then the stone shoud be collected:
        if (collision.tag == "Player")
        {
            //find the correct stone in the canvas and enable its picture:
            GameObject stoneUI = FindStoneUI();
            if (stoneUI)
            {
                stoneUI.GetComponent<Image>().enabled = true;
            }

            //Make an effect of collecting a magic stone:
            Transform effect = Instantiate(TakeStoneEffect, transform.position, TakeStoneEffect.rotation);

            ///Destroy the magic stone. Bacuse it was already collected.
            Destroy(gameObject);

            checkIfCompletedQuest();
        }
    }

    //check if the quest has been completed
    private void checkIfCompletedQuest()
    {
        //count how many stones already been found.
        int countCompletedStones = 0;
        foreach (var item in GameObject.FindGameObjectsWithTag("MagicStone"))
        {
            if (item.gameObject.layer == LayerMask.NameToLayer("UI") && item.GetComponent<Image>().enabled)
            {
                countCompletedStones++;
            }
        }
        //if there are 3 stones completed then go to the corresponding scene:
        if  (countCompletedStones == 3)
        {
            SceneManager.LoadScene("GameFinishedSuccessfuly");
        }
    }

    private GameObject FindStoneUI()
    {
        //find the matching stone in the canvas, by going over all object with the matching tag and in layer UI.
        //then check if the name is just like the stone's name with "UI"
        foreach (var item in GameObject.FindGameObjectsWithTag("MagicStone"))
        {
            if (item.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                if (item.name == gameObject.name + "UI")
                {
                    return item.gameObject;
                }
            }
        }
        return null;
    }
}
