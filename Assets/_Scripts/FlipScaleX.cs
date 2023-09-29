using UnityEngine;

public class FlipScaleX : MonoBehaviour
{

    Vector3 origScale; //stores orig scale on start

    void Awake()
    {
        origScale = transform.localScale;
    }

    public void FlipX(string newDir = "left")
    {
        transform.localScale = newDir == "left" ? new Vector3(-origScale.x, origScale.y, origScale.z) : origScale;
    }
}
