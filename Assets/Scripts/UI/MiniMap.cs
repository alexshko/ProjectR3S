using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //if pressed TAB button, then activate/deactivate the map on the canvas
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GetComponent<RawImage>().enabled = !GetComponent<RawImage>().enabled;
        }
    }
}
