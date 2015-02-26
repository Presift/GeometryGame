﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakeTier : MonoBehaviour {
	
	public GameModel manager;
	public Selection selectionManager;

	public float volume;
	public float tierHeight;

	public Vector3 centerPosition;
	public Vector3 startPositionForNextCakeTier;

	public bool inputLocked;

	public Vector3 positionOnCakePlate;
	GameObject cakeLayer;
	GameObject frostingLayer;

	public int tilesAcross;
	public int tilesDown;

	Vector3 startMarker;
	Vector3 endMarker;
	float speed = 5;
	float journeyLength;
	float startTime;

	bool move = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (move) 
		{
			Move ();	
		}
	}

	public float NewCakeTier ( int[,] pieceCoordinates, float newTileSize, float newTileHeight, float scaleChange, GameObject defaultTile, Vector3 position, CakeTier tierScript, List<GameObject> availableTiles, List<float> cakeVolumes, Material cakeColor, float frostingHeightMultiplier, float minVolumeDiff, int maxRoundTiles )
	{
		centerPosition = position;
		this.transform.position = centerPosition;

		//instantiate a cake layer
		GameObject newCakeLayer = (GameObject)Resources.Load("Cake Layer");
		GameObject newCakeLayerObject = (GameObject)Instantiate( newCakeLayer, position, Quaternion.identity);

		//add it as child of cake tier
		newCakeLayerObject.transform.parent = this.transform;
		
		//create cake tile pieces on layer
		CakeLayer layerScript = ( CakeLayer )newCakeLayerObject.GetComponent( typeof( CakeLayer ));
		float area = layerScript.SetUpPieceAndStats( pieceCoordinates, newTileSize, scaleChange, defaultTile, position, availableTiles, tierScript, cakeColor, maxRoundTiles );

		int redoAttempts = 0;

		//while potential cake volume is equal to another cake volume
		while( VolumeApproxMatchesAnotherCakeColume( area, cakeVolumes, minVolumeDiff ) && redoAttempts < 50 )
      	{
			//wipe previous layer
			layerScript.WipeCurrentLayer();
			//set up a new layer
			area = layerScript.SetUpPieceAndStats( pieceCoordinates, newTileSize, scaleChange, defaultTile, position, availableTiles, tierScript, cakeColor, maxRoundTiles );
			redoAttempts ++;
		}

		float frostingHeight = newTileHeight * frostingHeightMultiplier;

		//add a layer of frosting
		position += new Vector3( 0, 0, ( newTileHeight + frostingHeight ) / 2  );
		GameObject frostingLayer = ( GameObject )Instantiate( newCakeLayerObject, position, Quaternion.identity );
		//parent frosting to cake layer
		frostingLayer.transform.parent = layerScript.transform;
		//change color to frosting color
		CakeLayer frostingScript = ( CakeLayer )frostingLayer.GetComponent( typeof( CakeLayer ));
		frostingScript.ChangeChildrenColor ();
		frostingLayer.transform.localScale = new Vector3( frostingLayer.transform.localScale.x, frostingLayer.transform.localScale.y, frostingLayer.transform.localScale.z / 4 );
		frostingLayer.name = "Frosting";

		cakeLayer = newCakeLayerObject;
		startPositionForNextCakeTier = position;

		volume = area;

		tierHeight = newTileHeight + frostingHeight;

		return area;
	}

	public void MakeAdditionalLayers( int additionalLayers )
	{
		//for each additional layer over 1
		for (int layer = 0; layer < additionalLayers; layer++) 
		{
			//add a cake layer
			startPositionForNextCakeTier += new Vector3(0, 0, ( tierHeight ) / 2 );
			//duplicate cakeObject
			GameObject additionalLayer = ( GameObject )Instantiate( cakeLayer, startPositionForNextCakeTier, Quaternion.identity );
			startPositionForNextCakeTier += new Vector3(0, 0, ( tierHeight ) / 2 );
			additionalLayer.transform.parent = this.transform;
				
		}

		tierHeight *= (1 + additionalLayers);
	}

	bool VolumeApproxMatchesAnotherCakeColume( float thisCakeVolume, List<float> cakeVolumes, float minVolumeDifference )
	{
		for (int cake = 0; cake < cakeVolumes.Count; cake++) 
		{
			float volumeDifference = Mathf.Abs( thisCakeVolume - cakeVolumes[ cake ]);

			if( volumeDifference < minVolumeDifference )
			{
				return true;
			}
		}

		return false;
	}
	

	void Move ()
	{
		float distCovered = (Time.time - startTime) * speed;

		float fracJourney = distCovered / journeyLength;
		if (fracJourney >= 1) {
			move = false;
			transform.position = endMarker;
		} 
		else 
		{
			transform.position = Vector3.Lerp ( startMarker, endMarker, fracJourney);
		}
	}

	public void MoveToCakePlate( CakePlate plate )
	{
		//start rotation
		transform.localEulerAngles = new Vector3 (270, 110, 20);

		StartMove ( positionOnCakePlate );

		//parent to cake plate
		transform.parent = plate.transform;
	}


	public void StartMove( Vector3 newEndMarker )
	{
		endMarker = newEndMarker;
		startMarker = centerPosition;
		journeyLength = Vector3.Distance ( startMarker, newEndMarker );
		startTime = Time.time;
		move = true;
	}

	public void Selected()
	{
		inputLocked = selectionManager.IsSelectionCorrect ( this );
	}
	
}
