using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakeLayer : MonoBehaviour {
	
	float scaleChange;

	float tileSize;

	int[,] squareCoordinates = {{ 0, 0 }, { 0, 1 }, { 1, 1 }, { 1, 0 }}; 

	int[,] lineCoordinates = {{ 0, 0 }, { 0, 1 }, { 0, 2 }, { 0, 3 }}; 

	int[,] LShapeCoordinates = {{ 1, 0 }, { 0, 0 }, { 0, 1 }, { 0, 2 }}; 

	GameObject defaultTile;

	public List<GameObject> tilesUsed;

	public float area;

	public Selection selectionManager;
	public GameModel gameManager;

	public Color frostingColor;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown (KeyCode.Space)) 
//		{
//			SetUpPieceAndStats( 0, null );
//		}
	}


//	public void Selected()
//	{
//		selectionManager.IsSelectionCorrect ( this );
//	}

	public float SetUpPieceAndStats( int level, float newTileSize, float newScaleChange, GameObject newDefaultTile, Vector3 centerPosition, List<string> tetrisShapes, List<GameObject> availableTiles, CakeTier tierScript )

	{
		scaleChange = newScaleChange;
		tileSize = newTileSize;
		defaultTile = newDefaultTile;

		int random = Random.Range( 0, tetrisShapes.Count );
		CreateTetrisPiece( tetrisShapes[ random ], level, centerPosition, availableTiles, tierScript );
		area = CalculateArea();

		return area;
	}


	void CreateTetrisPiece( string tetrisShape, int level, Vector3 centerPosition, List<GameObject> availableTiles, CakeTier tierScript )
	{
		int[,] coordinates = getTetrisPieceCoordinates (tetrisShape);


		for (int coordinateIndex = 0; coordinateIndex < 4; coordinateIndex++) 
		
		{
			List < GameObject > refreshedTiles = new List < GameObject > ( availableTiles );

			int column = coordinates [ coordinateIndex, 0 ];
			int row = coordinates [ coordinateIndex, 1 ];

			List<bool> sideExposures = getTilePieceSideExposure( tetrisShape, coordinateIndex );

			GameObject newTile = makeNewFittedTile( refreshedTiles, sideExposures, column, row, centerPosition );
			//instantiate tile at scaled size at coordinate

			newTile.transform.localScale *= scaleChange;

			//set side Exposure bools on this tile
			CakeTile tileScript = (CakeTile)newTile.GetComponent(typeof(CakeTile));
			tileScript.SetGrandparentScript( tierScript );
			tileScript.sideExposures = sideExposures;

			//attach tile to parent piece
			newTile.transform.parent = this.gameObject.transform;
		}
	}
	

	float CalculateArea()
	{
		float area = 0;
		Transform parent = gameObject.transform;
		
		//for each child 
		foreach (Transform child in parent) 
		{
			//add child's area to total area
			CakeTile tileScript = (CakeTile)child.GetComponent(typeof(CakeTile));
			area += tileScript.area;

		}
		return area;

	}



	int getTileAndRotation( CakeTile tileScript, List<bool> sideExposures )
	{
		for (int rotation = 0; rotation < 4; rotation ++) 
		{
			List<float> sidePerimeters = tileScript.GetNewPerimetersAfterRotation ( 90 * rotation );
			if( tileFitsInCoordinates( sidePerimeters, sideExposures ))
			{
				//return side perimeters
				return ( 90 * rotation );
			}
		}

//		Debug.Log (tileScript.name + "won't fit here");
		return -1;
	}

	GameObject makeNewFittedTile( List<GameObject> availableTiles, List<bool> sideExposures, int column, int row, Vector3 centerPosition )
	{
		GameObject fittedTile = null;

		while ( fittedTile == null)
		{

			//randomly choose a tile from available tile and remove it from list of available tiles
			int random = Random.Range( 0, availableTiles.Count );
			GameObject possibleTile = availableTiles[ random ];

			availableTiles.RemoveAt( random );


			//get tile script
			CakeTile tileScript = ( CakeTile ) possibleTile.GetComponent(typeof(CakeTile));

			//get successful rotation for possible tile
			int rotation = getTileAndRotation( tileScript, sideExposures);

			if(rotation >= 0 )
			{
				return CreateNewTile( possibleTile, column, row, rotation, fittedTile, centerPosition );
			}


		}

		Debug.Log ("could not find tile from availableTiles to fit, had to use square tile");
		return CreateNewTile( defaultTile, column, row, 0, fittedTile, centerPosition );
	}

	GameObject CreateNewTile( GameObject newTile, int column, int row, int rotation, GameObject fittedTile, Vector3 centerPosition )
	{
		//increase count of this tile type
		UpdateTileTypesUsed( newTile );
		
		//instantiate new tile at rotation
		fittedTile = (GameObject)Instantiate( newTile, new Vector3( column * tileSize + centerPosition.x, row * tileSize + centerPosition.y, centerPosition.z ), Quaternion.identity);
		CakeTile fittedTileScript = ( CakeTile ) fittedTile.GetComponent(typeof(CakeTile));
		fittedTileScript.SetNewPerimetersAfterRotation( rotation );
		return fittedTile;
	}

	public void UpdateTileTypesUsed( GameObject tile )
	{
		tilesUsed.Add (tile);
	}


	bool tileFitsInCoordinates ( List<float> sidePerimeters, List<bool> sideExposures ) //side exposures is a list of left, top, right, bottom sides of a tile
	{
		for (int side = 0; side < 4; side++) 
		{
			if(!sideExposures[ side ])
			{
				//check to see if corresponding side on tileScript is a full side
				if(sidePerimeters [ side ] != 1 )
				{
					return false;
				}
			}

		}
		return true;
	}

	int[,] getTetrisPieceCoordinates ( string tetrisShape )
	{
		switch ( tetrisShape )
		{
			case "square":
				return squareCoordinates;
			case "line":
				return lineCoordinates;
			case "LShape":
				return LShapeCoordinates;
			default:
			Debug.Log (" not valid tetris shape found");
			return squareCoordinates;
		}
	}

	List<bool> getTilePieceSideExposure ( string tetrisShape, int coordinateIndex )
	{
		bool exposed = true;

		switch ( tetrisShape )
		{
			case "square":
			switch ( coordinateIndex )
			{
			case 0:
				return new List< bool > ( new bool[] { exposed, !exposed, !exposed, exposed } );
			case 1:
				return new List< bool > ( new bool[] { exposed, exposed, !exposed, !exposed });
			case 2:
				return new List< bool > ( new bool[] { !exposed, exposed, exposed, !exposed });
			case 3:
				return new List< bool > ( new bool[] { !exposed, !exposed, exposed, exposed });
			default:
				Debug.Log ("not corresponding coordinateIndex");
				return new List< bool > ( new bool[] { exposed, !exposed, !exposed, exposed } );
			}

		case "line":
			switch ( coordinateIndex )
			{
			case 0:
				return new List< bool > ( new bool[] { exposed, !exposed, exposed, exposed } );
			case 1:
				return new List< bool > ( new bool[] { exposed, !exposed, exposed, !exposed });
			case 2:
				return new List< bool > ( new bool[] { exposed, !exposed, exposed, !exposed });
			case 3:
				return new List< bool > ( new bool[] { exposed, exposed, exposed, !exposed });
			default:
				Debug.Log ("not corresponding coordinateIndex");
				return new List< bool > ( new bool[] { exposed, !exposed, !exposed, exposed } );
			}

		case "LShape":
			switch ( coordinateIndex )
			{
			case 0:
				return new List< bool > ( new bool[] { !exposed, exposed, exposed, exposed } );
			case 1:
				return new List< bool > ( new bool[] { exposed, !exposed, !exposed, exposed });
			case 2:
				return new List< bool > ( new bool[] { exposed, !exposed, exposed, !exposed });
			case 3:
				return new List< bool > ( new bool[] { exposed, exposed, exposed, !exposed });
			default:
				Debug.Log ("not corresponding coordinateIndex");
				return new List< bool > ( new bool[] { exposed, exposed, !exposed, exposed } );
			}
			
		default:
			Debug.Log ("not corresponding coordinateIndex");
			return new List< bool > ( new bool[] { exposed, !exposed, exposed, !exposed } );
		}

	}

	public void ChangeChildrenColor()
	{
		Transform parent = gameObject.transform;

		foreach (Transform child in parent) 
		{
			//set child color
			child.renderer.material.color = frostingColor;
		}
	}

//	void DestroyCurrentChildren()
//	{
//
//		Transform parent = gameObject.transform;
//
//		foreach (Transform child in parent) 
//		{
//			Destroy(child.gameObject);
//		}
//
//		gameObject.transform.DetachChildren ();
//
//	}

	

}

