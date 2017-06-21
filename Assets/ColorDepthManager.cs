using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDepthManager : MonoBehaviour {



    public DepthSourceManager depthSourceManager;
    public ColorSourceManager colorSourceManager;

    public Material highVertexPlaneMat;

    /*		_MainTex("Color", 2D) = "white" {}
		_DepthFeedTex("Depth", 2D) = "white" {}*/


    private void Start()
    {
        //highVertexPlaneMat.SetTextureScale("_MainTex", new Vector2(-1, 1));

    }
    private void Update()
    {
        //Color:
        highVertexPlaneMat.SetTexture("_MainTex", colorSourceManager.GetColorTexture());

        //Depth:
        highVertexPlaneMat.SetTexture("_DepthFeedTex", depthSourceManager.GetDepthTexture());

    }

}
