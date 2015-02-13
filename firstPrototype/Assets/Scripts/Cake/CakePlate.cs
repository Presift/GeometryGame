using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakePlate : MonoBehaviour {

	public Vector3 cakePlatePosition = new Vector3( 0 , -6, 0 );
	public Selection selectionManager;
	public Vector3 previousTierPositionOnPlate;
	public float previousTierHeight; 
	public float rotationSpeed = 50;
	public float finalYRotation = 350;
	public bool rotating = false;

	// Use this for initialization
	void Start () {
		Debug.Log (rotationSpeed);
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
		previousTierPositionOnPlate = cakePlatePosition;
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

		Vector3 newTierPosition = previousTierPositionOnPlate + new Vector3 ( 0, ( previousTierHeight + tierHeight ) / 2, 0 );

		previousTierHeight = tierHeight;
		previousTierPositionOnPlate = newTierPosition;

		Debug.Log (newTierPosition);
		return newTierPosition;
	}

	public void RotatePlate()
	{
		if (transform.rotation.y < finalYRotation) {
			transform.localEulerAngles += new Vector3 (0, rotationSpeed * Time.deltaTime, 0);
			Debug.Log (rotationSpeed);
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
