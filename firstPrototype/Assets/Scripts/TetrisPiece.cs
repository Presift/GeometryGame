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

	int squareTileCount = 0;
	int rightTriangleCount = 0;
	int isoscelesCount = 0;
	int circleCount = 0;
	int twoTriangleCount = 0;

	public float area;
	public float perimeter;

	public SelectionManager selectionManager;
	public GameManager gameManager;

	// Use this for initialization
	void Start () {
		scaleChange = gameManager.scaleChange;
		tileSize = gameManager.tileSize;
	}
	
	// Update is called once per frame
	void Update () {

	}


	public void Selected()
	{
		selectionManager.IsSelectionCorrect ( this );
	}

	public void SetUpPieceAndStats( int level )
	{
		DestroyCurrentChildren();

		//choose random tetris shape
		List<string> tetrisShapes = gameManager.GetTetrisShapes ();
		int random = Random.Range( 0, tetrisShapes.Count );
		CreateTetrisPiece( tetrisShapes[ random ], level );
		area = CalculateArea();
		perimeter = CalculatePerimeter();
	}


	void CreateTetrisPiece( string tetrisShape, int level )
	{
		int[,] coordinates = getTetrisPieceCoordinates (tetrisShape);

		for (int coordinateIndex = 0; coordinateIndex < 4; coordinateIndex++) 
		
		{
			List<GameObject> availableTiles = gameManager.GetAvailableTiles();

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
//		Debug.Log ("available pieces count : " + availableTiles.Count);

		while ( fittedTile == null)
		{
			//randomly choose a tile from available tile and remove it from list of available tiles
			int random = Random.Range( 0, availableTiles.Count );
			GameObject possibleTile = availableTiles[ random ];
			availableTiles.RemoveAt( random );

			//get tile script
			Tile tileScript = ( Tile ) possibleTile.GetComponent(typeof(Tile));

			//get successful rotation for possible tile
			int rotation = getTileAndRotation( tileScript, sideExposures);

			if(rotation >= 0 )
			{
				//increase count of this tile type
				UpdateTileTypeCount( tileScript.name );

				//instantiate new tile at rotation
				fittedTile = (GameObject)Instantiate( possibleTile, new Vector3( column * tileSize + centerPosition.x, row * tileSize + centerPosition.y, centerPosition.z ), Quaternion.identity);
				Tile fittedTileScript = ( Tile ) fittedTile.GetComponent(typeof(Tile));
				fittedTileScript.SetNewPerimetersAfterRotation( rotation );
				return fittedTile;
			}
		}
		return null;
	}

	public void UpdateTileTypeCount(string tileType)
	{
		switch (tileType) 
		{
		case "Square":
			squareTileCount++;
			break;
		case "RightTriangle":
			rightTriangleCount++;
			break;
		case "Isosceles":
			isoscelesCount++;
			break;
		case "Circ":
			circleCount++;
			break;
		case "2RightTriangles":
			twoTriangleCount++;
			break;
		default:
			Debug.Log ("tileType not found ");
			break;
		}

	}

	public void ResetTileTypeCount()
	{
		squareTileCount = 0;
		rightTriangleCount = 0;
		isoscelesCount = 0;
		circleCount = 0;
		twoTriangleCount = 0;
	}

	public void PrintTileTypeCount()
	{
//		Debug.Log (this.name + ";   square : " + squareTileCount + ";   triangle : " + rightTriangleCount + ";   isosceles : " + isoscelesCount + ";   circle : " + circleCount + ";   twoTriangles : " + twoTriangleCount);
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

