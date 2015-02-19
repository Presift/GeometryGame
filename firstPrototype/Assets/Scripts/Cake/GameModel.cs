using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameModel : MonoBehaviour 
{
//	public GameObject mainCamera;
	public GameObject canvas;

	public Text scoreDisplay;

	public int pointsForCorrect;

	public int minLevel = 0;
	public int maxLevel = 6;
	public int currentLevel = 0;
//	public int minLayersInTier = 1;
//	public int maxLayersInTier = 4;
	public int minTiersInCake = 2;
	public int maxTiersInCake = 5;

	public int correctAnswersInARow = 0;
	public int correctInARowForLevelUp = 3;

	public GameObject cubeTile;
	GameObject isoTile;
	GameObject roundTile;

	Vector3 leftScreen;
	Vector3 rightScreen;

	private float originalTileSize;
	private float horizontalWorldDistance;
	private float originalTileHeight;

	public float scaleChange;

	public int score = 0;

	public Selection selectionManager;

	List<int[,]> fourTilePieces = new List <int[,]>();
	List<int[,]> fiveTilePieces = new List <int[,]>();
	List<int[,]> threeTilePieces = new List <int[,]>();

	//four tile pieces
	int[,] squareCoordinates = {{ 0, 0 }, { 0, 1 }, { 1, 1 }, { 1, 0 }}; 
	
	int[,] line4Coordinates = {{ 0, 0 }, { 0, 1 }, { 0, 2 }, { 0, 3 }}; 
	
	int[,] LShapeCoordinates = {{ 1, 0 }, { 0, 0 }, { 0, 1 }, { 0, 2 }}; 
	
	int[,] topHatCoordinates = {{ 0, 0 }, { 1, 0 }, { 2, 0 }, { 1, 1 }};

	int[,] zigZagCoordinates = {{ 0, 0 }, { 1, 0 }, { 1, 1 }, { 2, 1 }};

	//five tile pieces
	int[,] line5Coordinates = {{ 1, 0 }, { 0, 0 }, { 0, 1 }, { 0, 2 }, { 0, 3 }};
	
	int[,] crossCoordinates = {{ 0, 1 }, { 1, 0 }, { 1, 1 }, { 2, 1 }, { 1, 2 } };

	int[,] bucketCoordinates = {{ 0, 1 }, { 0, 0 }, { 1, 0 }, { 2, 0 }, { 2, 1 } };

	int[,] snakeCoordinates = {{ 0, 0 }, { 1, 0 }, { 1, 1 }, { 2, 1 }, { 2, 2 } };

	//three tile pieces
	int[,] line3Coordinates = {{ 0, 0 }, { 0, 1 }, { 0, 2 }}; 

	int[,] corner3Coordinates = {{ 0, 0 }, { 0, 1 }, { 1, 0 }}; 


	// Use this for initialization
	void Awake () {

		cubeTile = (GameObject)Resources.Load("cube");
		isoTile = (GameObject)Resources.Load("iso");
		roundTile = (GameObject)Resources.Load("round");

		MeshRenderer tileRenderer = (MeshRenderer)cubeTile.GetComponent (typeof(MeshRenderer));
		originalTileSize = tileRenderer.bounds.extents.x * 2;
		originalTileHeight = tileRenderer.bounds.extents.z * 2;

		leftScreen = Camera.main.ScreenToWorldPoint (new Vector3( 0, 0, 0 ));
		leftScreen = new Vector3 (leftScreen.x, 0, 0);
		rightScreen = Camera.main.ScreenToWorldPoint (new Vector3(Screen.width, 0, 0 ));
		horizontalWorldDistance = Mathf.Abs (rightScreen.x - leftScreen.x);

//		Debug.Log (leftScreen);
	}

	void Start()
	{
		scoreDisplay.text = "Score : " + score;

		fourTilePieces.Add (squareCoordinates);
		fourTilePieces.Add (line4Coordinates);
		fourTilePieces.Add (LShapeCoordinates);
		fourTilePieces.Add (topHatCoordinates);
		fourTilePieces.Add (zigZagCoordinates);
		
		fiveTilePieces.Add (line5Coordinates);
		fiveTilePieces.Add (crossCoordinates);
		fiveTilePieces.Add (bucketCoordinates);
		fiveTilePieces.Add (snakeCoordinates);

		threeTilePieces.Add (line3Coordinates);
		threeTilePieces.Add (corner3Coordinates);

		selectionManager.ShowNewProblem();

	}
	
	// Update is called once per frame
	void Update () {
		ChangeLevel ();

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			selectionManager.ShowNewProblem();
		}
	}

	public int[,] PieceCoordinates( int tileCount)
	{
		int random;
		switch (tileCount) 
		{
		case 3:
//			Debug.Log (" tile count : " + tileCount );
			random = Random.Range( 0, threeTilePieces.Count );
//			Debug.Log ("random : " + random);
			return threeTilePieces[ random ];
		case 4:
//			Debug.Log (" tile count : " + tileCount );
			random = Random.Range( 0, fourTilePieces.Count );
//			Debug.Log ("random : " + random);
			return fourTilePieces[ random ];
		case 5:
//			Debug.Log (" tile count : " + tileCount );
			random = Random.Range( 0, fiveTilePieces.Count );
//			Debug.Log ("random : " + random);
			return fiveTilePieces[ random ];
		default:
			Debug.Log (" not a valid tile count for piece coordinates");
			random = Random.Range( 0, fourTilePieces.Count );
			return fourTilePieces[ random ];
		}
	}

	public List<Vector3> CakeTierStartPositions( float maxTileSize, List<Vector2> cakeSizesInTiles, int totalTilesAcross )
	{
		int tierCount = cakeSizesInTiles.Count;
		float widthOfSpacer = (horizontalWorldDistance - (totalTilesAcross * maxTileSize)) / (tierCount + 1);
		List<Vector3> cakeTierPositions = new List<Vector3> ();
		Vector3 startPositionOfTileSpace = new Vector3(leftScreen.x, -2, 0 );
//		Debug.Log (" far left : " + startPositionOfTileSpace);
		for (int tier = 0; tier< tierCount; tier++) 
		{
			float tilesAcrossInThisTier = cakeSizesInTiles[ tier ].x;
			float widthOfPiece = tilesAcrossInThisTier * maxTileSize;
//			float spaceForThisPiece = ((tilesAcrossInThisTier/2 + 1) * maxTileSize);
			Vector3 startPositionOfPiece = startPositionOfTileSpace + new Vector3( widthOfSpacer, 0, 0);
//			Debug.Log ( "start position of piece : " + startPositionOfPiece);
			startPositionOfTileSpace = startPositionOfPiece + new Vector3( widthOfPiece, 0, 0 );
//			Debug.Log ( "new far left : " + startPositionOfPiece);
			cakeTierPositions.Add (startPositionOfPiece);
//			Debug.Log (" tier " + tier + " : " + startPositionOfPiece );
		}

		return cakeTierPositions;
	}

	public float MaxTileSize( int tierCount, int tilesToFitAcross, int tileToFitDown )
	{
		//get height of world from camera
		float height = Camera.main.orthographicSize * 2.0f;
		float width = height * Screen.width/Screen.height;
		float maxTileWidth = width/ ( tilesToFitAcross + (tierCount + 1)); //includes space on left and right of each tier
		float maxTileHeight = height / (tileToFitDown * 2);  //how many tiles fit in half height of screen
		float maxTileSize = Mathf.Min (maxTileWidth, maxTileHeight);
		return maxTileSize;
	}

	public float ScaleChange( float maxTileSize )
	{
		float scaleChange = maxTileSize / originalTileSize;
		return scaleChange;
	}

	public float TileHeight( float scaleChange )
	{
		return (scaleChange * originalTileHeight);
	}

	public List<int> GetPossibleTileCountsByLevel()
	{
		List<int> possibleTileCounts = new List<int>();
		if (currentLevel >= 0 ) 
		{
			possibleTileCounts.Add ( 4 );
		}
		if( currentLevel >= 2 )
		{
			possibleTileCounts.Add ( 3 );
		}
		if( currentLevel >= 4 )
		{
			possibleTileCounts.Add ( 5 );
		}

		return possibleTileCounts;
	}

	public int GetLayerCount( int tileCount )  // based on tile count and level, make this more random later
	{
		switch (tileCount) 
		{
		case 3:
			if( currentLevel < 4 )
			{
				return 2;
			}
			else
			{
				return 3;
			}
		case 4:
			if( currentLevel < 4 )
			{
				return 1;
			}
			else
			{
				return 2;
			}
		case 5:
			return 1;
		default :
			Debug.Log ("not valid tile count");
			return 1;
				}
	}
	

	public List<GameObject> GetAvailableTilesByLevel()
	{
		List<GameObject> availableTiles = new List<GameObject>();
		
		availableTiles.Add (cubeTile);
		availableTiles.Add (isoTile);

		if (currentLevel >= 1) 
		{
			availableTiles.Add (roundTile);
		}

//		if(currentLevel >= 4 )
//		{
//			availableTiles.Add (isosceles);
//		}
//
//		if (currentLevel >= 6)
//		{
//			availableTiles.Add (twoTriangles);
//		}
		
		return availableTiles;
		
	}

	public int NumberOfTiersByLevel()
	{
		int numberOfTiers = 2;

		if ( currentLevel > 0 ) 
		{
			numberOfTiers ++;
		}
		if (currentLevel > 2) 
		{
			numberOfTiers ++;
		}
		if (currentLevel > 4) 
		{
			numberOfTiers ++;
		}
//		Debug.Log (numberOfTiers);
		return numberOfTiers;
	}
	

	void ChangeLevel()
	{
		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			currentLevel = Mathf.Min( currentLevel + 1, maxLevel );
			Debug.Log ("CURRENT LEVEL: " + currentLevel);
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			currentLevel = Mathf.Max (0, currentLevel - 1);
			Debug.Log ("CURRENT LEVEL: " + currentLevel);
		}
	}

	public void UpdateStatsAndLevel( bool correctAnswer )
	{
		if (correctAnswer) {
			int scoreIncrease = ( currentLevel + 1 ) * pointsForCorrect;
			score += scoreIncrease;
			scoreDisplay.text = "Score : " + score;

			if( selectionManager.currentCakeTiers.Count <= 1 )
			{
				correctAnswersInARow ++;
			}
		} 

		else 
		{
			correctAnswersInARow = 0;
//			currentLevel = Mathf.Max(minLevel, currentLevel - 1 );
			Debug.Log ("leveled down");
		}

		UpdateLevel ();
	}

	void UpdateLevel()
	{
		if (correctAnswersInARow == correctInARowForLevelUp) 
		{
			currentLevel = Mathf.Min ( maxLevel, currentLevel + 1);
			correctAnswersInARow = 0;
			Debug.Log ("leveled up");
		}

	}

}
