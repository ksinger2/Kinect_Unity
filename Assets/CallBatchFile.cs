using UnityEngine;
using System.Collections;

public class CallBatchFile : MonoBehaviour {

    public string Path;

	// Use this for initialization
	void Start () {
        Debug.Log("starting");

        System.Diagnostics.Process.Start(Path);


	}
	
}
