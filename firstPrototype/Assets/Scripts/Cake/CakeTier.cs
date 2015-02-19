using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakeTier : MonoBehaviour {
	
	public GameModel manager;
	public Selection selectionManager;

	public float volume;
	public float tierHeight;

	public Vector3 centerPosition;
	public bool inputLocked;

	public Vector3 positionOnCakePlate;

	public int tilesAcross;
	public int tilesDown;

	Vector3 startMarker;
	Vector3 endMarker;
	float speed = 5;
	float journeyLength;
	float startTime;

	bool move = false;
	bool rotate = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (move) 
		{
			Move ();	
		}
		if (rotate) 
		{
	
		}
	}

	public float NewCakeTier ( int[,] pieceCoordinates, int numberOfLayers, float newTileSize, float newTileHeight, float scaleChange, GameObject defaultTile, Vector3 position, CakeTier tierScript, List<GameObject> availableTiles, List<float> cakeVolumes )
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
		float area = layerScript.SetUpPieceAndStats( pieceCoordinates, newTileSize, scaleChange, defaultTile, position, availableTiles, tierScript );

		int redoAttempts = 0;

		//while potential cake volume is equal to another cake volume
		while( VolumeMatchesAnotherCakeColume( area * numberOfLayers, cakeVolumes ) && redoAttempts < 50 )
      	{
			//wipe previous layer
			layerScript.WipeCurrentLayer();
			//set up a new layer
			area = layerScript.SetUpPieceAndStats( pieceCoordinates, newTileSize, scaleChange, defaultTile, position, availableTiles, tierScript );
			redoAttempts ++;
		}

		Debug.Log (" cake remake count : " + redoAttempts);
			

		float frostingHeight = newTileHeight/4;

		//add a layer of frosting
		position += new Vector3( 0, 0, ( newTileHeight + frostingHeight ) / 2  );
		GameObject frostingLayer = ( GameObject )Instantiate( newCakeLayerObject, position, Quaternion.identity );
		//parent frosting to cake layer
		frostingLayer.transform.parent = layerScript.transform;
		//change color to frosting color
		CakeLayer frostingScript = ( CakeLayer )frostingLayer.GetComponent( typeof( CakeLayer ));
		frostingScript.ChangeChildrenColor ();
//		frostingLayer.transform.scale.z = frostingHeight;
		frostingLayer.transform.localScale = new Vector3( frostingLayer.transform.localScale.x, frostingLayer.transform.localScale.y, frostingLayer.transform.localScale.z / 4 );
		frostingLayer.name = "Frosting";

		//for each additional layer over 1
		for (int layer = 1; layer < numberOfLayers; layer++) 
		{
			//add a cake layer
			position += new Vector3(0, 0, ( newTileHeight + frostingHeight ) / 2 );
			//duplicate cakeObject
			GameObject additionalLayer = ( GameObject )Instantiate( newCakeLayerObject, position, Quaternion.identity );
			additionalLayer.transform.parent = this.transform;

			//add a layer of frosting
			position += new Vector3( 0, 0, ( newTileHeight + frostingHeight ) / 2  );
			GameObject additionalFrostingLayer = ( GameObject )Instantiate( frostingLayer, position, Quaternion.identity );
			//parent frosting to cake layer
			additionalFrostingLayer.transform.parent = additionalLayer.transform;
			additionalFrostingLayer.name = "Frosting";


		}

		volume = area * numberOfLayers;

		tierHeight = (newTileHeight + frostingHeight) * numberOfLayers;
		return volume;
	}

	bool VolumeMatchesAnotherCakeColume( float thisCakeVolume, List<float> cakeVolumes )
	{
		for (int cakeVolume = 0; cakeVolume < cakeVolumes.Count; cakeVolume++) 
		{
			if( thisCakeVolume == cakeVolumes[ cakeVolume ] )
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
//		Debug.Log (transform.position);
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

//		Debug.Log (positionOnCakePlate);
		StartMove ( positionOnCakePlate );

		//parent to cake plate
		transform.parent = plate.transform;
	}

	public void StartRotation( Vector3 endRotation )
	{

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
//		inputLocked = true;
	}
	
}
