using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakePlate : MonoBehaviour {

	public Vector3 cakePlatePosition = new Vector3( 0 , -6, 0 );
	public Selection selectionManager;
	public Vector3 previousTierCenterPositionOnPlate;
	public float previousTierHeight; 

	public float rotationSpeed = 50;
	public float finalYRotation = 350;
	public bool rotating = false;

	public float singleTierHeight;

	// Use this for initialization
	void Start () {
//		Debug.Log (finalYRotation);
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
		previousTierCenterPositionOnPlate = cakePlatePosition;
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
		float centerOffset = (( tierHeight / singleTierHeight ) - 1) / 2 * singleTierHeight;
		Debug.Log ("center offset : " + centerOffset);

//		Vector3 newTierPosition = previousTierCenterPositionOnPlate + new Vector3 ( 0, ( previousTierHeight + tierHeight + centerOffset ) / 2, 0 );
//		Vector3 newTierPosition = previousTierCenterPositionOnPlate + new Vector3 ( 0, ( previousTierHeight + tierHeight ) / 2, 0 );
		Vector3 newTierPosition = previousTierCenterPositionOnPlate + new Vector3 ( 0, ( previousTierHeight + singleTierHeight ) / 2, 0 );


		previousTierHeight = tierHeight;
		previousTierCenterPositionOnPlate = newTierPosition + new Vector3( 0, centerOffset, 0);
		Debug.Log (" tier height : " + tierHeight);
		Debug.Log (" tier position : " + newTierPosition);
//		Debug.Log (newTierPosition);
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
