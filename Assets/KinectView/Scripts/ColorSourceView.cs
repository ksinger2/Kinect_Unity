using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorSourceView : MonoBehaviour
{
	public GameObject colorSourceManager;
    private ColorSourceManager _colorManager;
    //private ColorDepthView _colorManager;

    public Texture colorOutputTexture;
	
    void Start ()
    {
      //  gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }
    
    void Update()
    {
		if (colorSourceManager == null)
		{
			return;
		}

        _colorManager = colorSourceManager.GetComponent<ColorSourceManager>();
        //_colorManager = colorSourceManager.GetComponent<ColorDepthView>();
		if (_colorManager == null)
		{
			return;
		}

        //gameObject.GetComponent<Renderer>().material.mainTexture = _colorManager.GetColorTexture();
        colorOutputTexture = _colorManager.GetColorTexture();
    }
}
