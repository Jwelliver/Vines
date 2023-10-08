using UnityEngine;

public class MatchCameraPosition : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    // [SerializeField] bool matchX;

    // * Was going to generalize with vector2 to represent followspeed, but only implementing X for now since that's all we need
    // [SerializeField] bool matchY;


    Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {

        myTransform.position = new Vector2(camTransform.position.x, myTransform.position.y);

    }
}
