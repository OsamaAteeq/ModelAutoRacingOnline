using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class center : MonoBehaviour
{
    protected Bounds _combinedBounds;
    protected Renderer _renderer;
    // Start is called before the first frame update
    void Awake()
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            _combinedBounds = (renderers[0].bounds);
            for (int i = 1, len = renderers.Length; i < len; i++)
            {
                _combinedBounds.Encapsulate(renderers[i].bounds);
            }
        }

        Debug.Log(_combinedBounds.center);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
