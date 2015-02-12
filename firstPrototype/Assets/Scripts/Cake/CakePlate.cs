using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakePlate : MonoBehaviour {

	public Vector3 cakePlatePosition = new Vector3( 0 , -6, 0 );

	public Vector3 previousTierPositionOnPlate;
	public float previousTierHeight; 

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void WipeCakePlate()
	{
		previousTierHeight = 0;
		previousTierPositionOnPlate = cakePlatePosition;
		DestroyCurrentChildren ();
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
}
