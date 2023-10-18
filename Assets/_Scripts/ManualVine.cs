using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualVine : MonoBehaviour
{
    [SerializeField] VineFactoryConfig vineFactoryConfig;

    // Start is called before the first frame update
    void Start()
    {
        VineFactory.Instance.GenerateVine(transform.position, transform, vineFactoryConfig);
    }
}
