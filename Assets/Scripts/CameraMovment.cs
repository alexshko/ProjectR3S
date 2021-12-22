using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    public Transform playertrn;

    // Update is called once per frame
    void Update()
    {
        if (playertrn != null)
        {
            transform.position = new Vector3(playertrn.position.x, playertrn.position.y, -25);
        }
    }
}
