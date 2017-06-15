using UnityEngine;
using System.Collections;

public class DepthSourceKaren : MonoBehaviour {


    public GameObject depthSourceManager;
    private DepthSourceManager _depthManager;

    public Texture depthOutputTexture;



    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
        
    }

    void Update()
    {
        if (depthSourceManager == null)
        {
            return;
        }
        _depthManager = depthSourceManager.GetComponent<DepthSourceManager>();
        
        if (_depthManager == null)
        {
            return;
        }

        gameObject.GetComponent<Renderer>().material.mainTexture = _depthManager.GetDepthTexture();
        depthOutputTexture = _depthManager.GetDepthTexture();


    }
}
