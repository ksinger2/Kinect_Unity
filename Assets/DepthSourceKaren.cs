using UnityEngine;
using System.Collections;

public class DepthSourceKaren : MonoBehaviour {


    public GameObject DepthSourceManager;
    private DepthSourceManager _DepthManager;



    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }

    void Update()
    {
        if (DepthSourceManager == null)
        {
            return;
        }
        _DepthManager = DepthSourceManager.GetComponent<DepthSourceManager>();
        
        if (_DepthManager == null)
        {
            return;
        }

        gameObject.GetComponent<Renderer>().material.mainTexture = _DepthManager.GetDepthTexture();

        
    }
}
