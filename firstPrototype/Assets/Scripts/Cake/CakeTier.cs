using UnityEngine;
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

	public List<GameObject> squareTiles;  //in order from bottom layer to topmost
	public List<GameObject> isoTiles;
	public List<GameObject> roundTiles;

	public int tilesAcross;
	public int tilesDown;

	Vector3 startMarker;
	Vector3 endMarker;
	float speed = 5;
	float journeyLength;
	float startTime;

	int layerCount = 1;

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

	public float NewCakeTier ( int[,] pieceCoordinates, float newTileSize, float newTileHeight, float scaleChange, 
	                          GameObject defaultTile, Vector3 position, CakeTier tierScript, List<GameObject> availableTiles, 
	                          List<float> cakeVolumes, Material cakeColor, float frostingHeightMultiplier, float minVolumeDiff, 
	                          int maxRoundTiles )
	{
		centerPosition = position;
		this.transform.position = centerPosition;

		//instantiate a cake layer
		GameObject newCakeLayer = (GameObject)Resources.Load("Cake Layer");
		GameObject newCakeLayerObject = (GameObject)Instantiate( newCakeLayer, position, Quaternion.identity);

		//add it as child of cake tier
		newCakeLayerObject.transform.parent = this.transform;

		float frostingHeight = newTileHeight * frostingHeightMultiplier;
		//create cake tile pieces on layer
		CakeLayer layerScript = ( CakeLayer )newCakeLayerObject.GetComponent( typeof( CakeLayer ));
		float area = layerScript.SetUpPieceAndStats( pieceCoordinates, newTileSize, scaleChange, defaultTile, 
		                                            position, availableTiles, tierScript, cakeColor, 
		                                            maxRoundTiles, manager.originalTileHeight, frostingHeightMultiplier);

		int redoAttempts = 0;

		//while potential cake volume is equal to another cake volume
		while( VolumeApproxMatchesAnotherCakeColume( area, cakeVolumes, minVolumeDiff ) && redoAttempts < 50 )
      	{
			//wipe previous layer
			layerScript.WipeCurrentLayer();
			//set up a new layer
			area = layerScript.SetUpPieceAndStats( pieceCoordinates, newTileSize, scaleChange, defaultTile, 
			                                      position, availableTiles, tierScript, cakeColor, 
			                                      maxRoundTiles, manager.originalTileHeight, frostingHeight );
			redoAttempts ++;
		}

		squareTiles = layerScript.squareTiles;
		roundTiles = layerScript.roundTiles;
		isoTiles = layerScript.isoTiles;

		tierHeight = newTileHeight + frostingHeight;
		cakeLayer = newCakeLayerObject;
		startPositionForNextCakeTier = position - new Vector3( 0, 0, tierHeight  );

		volume = area;

		return area;
	}
	

	public void MakeAdditionalLayers( int additionalLayers )
	{
		//for each additional layer
		layerCount += additionalLayers;
		for (int layer = 0; layer < additionalLayers; layer++) 
		{
			//duplicate cakeObject
			GameObject additionalLayer = ( GameObject )Instantiate( cakeLayer, startPositionForNextCakeTier, cakeLayer.transform.rotation );
			startPositionForNextCakeTier += new Vector3(0, 0, tierHeight );
			additionalLayer.transform.parent = this.transform;

			CakeLayer cakeLayerScript = ( CakeLayer )additionalLayer.GetComponent( typeof( CakeLayer ));

			//add square tiles from layer to list of all square tiles in tier
			for( int square = 0; square < cakeLayerScript.squareTiles.Count; square ++ )
			{
				squareTiles.Add ( cakeLayerScript.squareTiles[ square ] );
			}

			for( int round = 0; round < cakeLayerScript.roundTiles.Count; round ++ )
			{
				roundTiles.Add ( cakeLayerScript.roundTiles[ round ] );
			}

			for( int iso = 0; iso < cakeLayerScript.isoTiles.Count; iso ++ )
			{
				isoTiles.Add ( cakeLayerScript.isoTiles[ iso ] );
			}
				
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
