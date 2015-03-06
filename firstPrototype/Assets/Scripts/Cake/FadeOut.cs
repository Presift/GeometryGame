using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {

	public bool fading = false;
	public float fadeOutTimeStart;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (fading) 
		{
			Fade();
		}
	}

	public void Fade()
	{
		if (Time.time >= fadeOutTimeStart ) 
		{
			gameObject.SetActive( false );
			fading = false;
		}
	}
	
	public void StartFadeOut( float time )
	{
		fadeOutTimeStart = time;
		fading = true;
		Debug.Log (" time : " + fadeOutTimeStart + ", object : " + gameObject.name);
	}
}
