using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakePlate : MonoBehaviour {

	public Vector3 cakePlatePosition = new Vector3( 0 , -6, 0 );
	public Selection selectionManager;
	public Vector3 previousTierTopPosition;
	public float previousTierHeight; 

	public float rotationSpeed = 50;
	public float finalYRotation = 350;
	public bool rotating = false;

	public float singleTierHeight;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (rotating) 
		{
			RotatePlate();

		}
	}

	public void WipeCakePlate()
	{
		previousTierHeight = 0;
		previousTierTopPosition = cakePlatePosition;
		DestroyCurrentChildren ();
		ResetRotation ();
	
	}


	void DestroyCurrentChildren()
	{
		
		Transform parent = gameObject.transform;
		
		foreach (Transform child in parent) 
		{
			Destroy(child.gameObject);
		}
		
		gameObject.transform.DetachChildren ();
		
	}

	public Vector3 TierPositionOnPlate( float tierHeight )
	{
		float centerOffset = tierHeight - ( singleTierHeight / 2 );
		Vector3 newTierPosition = previousTierTopPosition + new Vector3 (0, centerOffset, 0);
		previousTierTopPosition = newTierPosition + new Vector3 (0, singleTierHeight / 2, 0);
		return newTierPosition;

	}

	public void RotatePlate()
	{
		if (transform.localEulerAngles.y < finalYRotation) {
			transform.localEulerAngles += new Vector3 (0, rotationSpeed * Time.deltaTime, 0);
		} 
		else 
		{
			selectionManager.ShowNewProblem();
			rotating = false;
		}
	}

	void ResetRotation()
	{
		transform.localEulerAngles = Vector3.zero;
	}
}
