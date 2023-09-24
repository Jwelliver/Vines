using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualVine : MonoBehaviour
{
    [SerializeField] VineFactory vineFactory;
    [SerializeField] VineFactoryConfig vineFactoryConfig;

    // Start is called before the first frame update
    void Start()
    {
        vineFactory.GenerateVine(transform.position, transform, vineFactoryConfig);
    }
}
