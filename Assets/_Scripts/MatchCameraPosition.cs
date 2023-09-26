using UnityEngine;

public class MatchCameraPosition : MonoBehaviour
{
    [SerializeField] Camera camera;
    // [SerializeField] bool matchX;

    // * Was going to generalize with vector2 to represent followspeed, but only implementing X for now since that's all we need
    // [SerializeField] bool matchY;


    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector2(camera.transform.position.x, transform.position.y);

    }
}
