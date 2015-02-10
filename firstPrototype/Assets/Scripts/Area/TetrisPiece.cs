using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetrisPiece : MonoBehaviour {

	public Vector3 centerPosition;

	float scaleChange;

	float tileSize;

	int[,] squareCoordinates = {{ 0, 0 }, { 0, 1 }, { 1, 1 }, { 1, 0 }}; 

	int[,] lineCoordinates = {{ 0, 0 }, { 0, 1 }, { 0, 2 }, { 0, 3 }}; 

	int[,] LShapeCoordinates = {{ 1, 0 }, { 0, 0 }, { 0, 1 }, { 0, 2 }}; 

	GameObject defaultTile;

	public List<GameObject> tilesUsed;

	public float area;
	public float perimeter;

	public SelectionManager selectionManager;
	public GameManager gameManager;

	// Use this for initialization
	void Start () {
		scaleChange = gameManager.scaleChange;
		tileSize = gameManager.tileSize;
		defaultTile = gameManager.squareTile;
	}
	
	// Update is called once per frame
	void Update () {

	}


	public void Selected()
	{
		selectionManager.IsSelectionCorrect ( this );
	}

	public void SetUpPieceAndStats( int level, List<GameObject> previouslyUsedTiles )
	{
		DestroyCurrentChildren();

		//choose random tetris shape
		List<string> tetrisShapes = gameManager.GetTetrisShapes ();
		int random = Random.Range( 0, tetrisShapes.Count );
		CreateTetrisPiece( tetrisShapes[ random ], level, previouslyUsedTiles );
		area = CalculateArea();
		perimeter = CalculatePerimeter();
	}


	void CreateTetrisPiece( string tetrisShape, int level, List<GameObject> previouslyUsedTiles )
	{
		int[,] coordinates = getTetrisPieceCoordinates (tetrisShape);
		tilesUsed = new List<GameObject> ();  //reset list of tiles used
		List<GameObject> availableTiles = new List<GameObject>();

//		if (previouslyUsedTiles != null) //if not first tetris piece
//		{ 
//			availableTiles = previouslyUsedTiles;
//		}


		for (int coordinateIndex = 0; coordinateIndex < 4; coordinateIndex++) 
		
		{
//			if( previouslyUsedTiles == null ) //if this is the first tetris piece being created
//			{
				availableTiles = gameManager.GetAvailableTiles();
//				Debug.Log (" available Tiles count : " + availableTiles.Count);
//			}
	

			int column = coordinates [ coordinateIndex, 0 ];
			int row = coordinates [ coordinateIndex, 1 ];

			List<bool> sideExposures = getTilePieceSideExposure( tetrisShape, coordinateIndex );

			GameObject newTile = makeNewFittedTile( availableTiles, sideExposures, column, row );
			//instantiate tile at scaled size at coordinate

			newTile.transform.localScale *= scaleChange;

			//set side Exposure bools on this tile
			Tile tileScript = (Tile)newTile.GetComponent(typeof(Tile));
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
			Tile tileScript = (Tile)child.GetComponent(typeof(Tile));
			area += tileScript.area;

		}
//		Debug.Log ("area : " + area);
		return area;

	}

	float CalculatePerimeter()
	{

		float perim = 0;
		Transform parent = gameObject.transform;

		//for each child 
		foreach (Transform child in parent) 
		{
			//add child's area to total area
			Tile tileScript = (Tile)child.GetComponent(typeof(Tile));
			List<bool> sideExposures = tileScript.sideExposures;
			List<float> sidePerimeters = tileScript.sidePerimeters;
			for( int side = 0; side < 4; side++)
			{
				if(sideExposures[ side ]) //if side is exposed
				{
					//add perimeter to perimeter total
					perim += sidePerimeters[ side ];
//					Debug.Log ("perim : " + sidePerimeters[ side ]);
				}
			}
		}
//		Debug.Log ("perimeter : " + perim);
		return perim;

	}


	int getTileAndRotation( Tile tileScript, List<bool> sideExposures )
	{
		for (int rotation = 0; rotation < 4; rotation ++) 
		{
			List<float> sidePerimeters = tileScript.GetNewPerimetersAfterRotation ( 90 * rotation );
//			Debug.Log ( "rotation try : " + (rotation * 90));
			if( tileFitsInCoordinates( sidePerimeters, sideExposures ))
			{
				//return side perimeters
				return ( 90 * rotation );
			}
		}

//		Debug.Log (tileScript.name + "won't fit here");
		return -1;
	}

	GameObject makeNewFittedTile( List<GameObject> availableTiles, List<bool> sideExposures, int column, int row )
	{
		GameObject fittedTile = null;

		while ( fittedTile == null)
		{
//			Debug.Log (name + " available tiles : " + availableTiles.Count);

//			if( availableTiles.Count == 0 )
//			{
//				Debug.Log("no more available tiles");
//				return CreateNewTile( defaultTile, column, row, 0, fittedTile );
//			}

			//randomly choose a tile from available tile and remove it from list of available tiles
			int random = Random.Range( 0, availableTiles.Count );
			GameObject possibleTile = availableTiles[ random ];

//			if( name == "Tetris Piece1")
//			{
				availableTiles.RemoveAt( random );
//			}


			//get tile script
			Tile tileScript = ( Tile ) possibleTile.GetComponent(typeof(Tile));

			//get successful rotation for possible tile
			int rotation = getTileAndRotation( tileScript, sideExposures);

			if(rotation >= 0 )
			{
//				if( name != "Tetris Piece1")
//				{
//					availableTiles.RemoveAt( random ); 
//				}

				return CreateNewTile( possibleTile, column, row, rotation, fittedTile );
			}


		}

		Debug.Log ("could not find tile from availableTiles to fit, had to use square tile");
		return CreateNewTile( defaultTile, column, row, 0, fittedTile );
	}

	GameObject CreateNewTile( GameObject newTile, int column, int row, int rotation, GameObject fittedTile )
	{
		//increase count of this tile type
		UpdateTileTypesUsed( newTile );
		
		//instantiate new tile at rotation
		fittedTile = (GameObject)Instantiate( newTile, new Vector3( column * tileSize + centerPosition.x, row * tileSize + centerPosition.y, centerPosition.z ), Quaternion.identity);
		Tile fittedTileScript = ( Tile ) fittedTile.GetComponent(typeof(Tile));
		fittedTileScript.SetNewPerimetersAfterRotation( rotation );
		return fittedTile;
	}

	public void UpdateTileTypesUsed( GameObject tile )
	{
		tilesUsed.Add (tile);
//		Debug.Log (name + "  " + tilesUsed.Count);
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

	void DestroyCurrentChildren()
	{

		Transform parent = gameObject.transform;

		foreach (Transform child in parent) 
		{
			Destroy(child.gameObject);
		}

		gameObject.transform.DetachChildren ();

	}

	

}

