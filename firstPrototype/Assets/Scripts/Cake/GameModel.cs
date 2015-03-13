using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameModel : MonoBehaviour 
{
	public GameObject canvas;

	public Text scoreDisplay;
	public Text levelDisplay;
	public Text finalScoreDisplay;

//	public GameData data;

	public GameObject endGameDisplay;

	public int pointsForCorrect;

	public int numberOfTiers;

	public int minLevel = 0;
	public int maxLevel = 15;
	public int currentLevel = 0;
	public int minTiersInCake = 2;
	public int maxTiersInCake = 5;
	public int maxLayerInTier = 1;
	public int maxRoundTilesPerTier = 0;

	public List<int> possibleTileCounts;
	public List<GameObject> availableTiles;

	public int correctAnswersInARow = 0;
	public int correctInARowForLevelUp = 3;

	public GameObject cubeTile;
	GameObject isoTile;
	GameObject roundTile;

	Vector3 leftScreen;
	Vector3 rightScreen;

	private float originalTileSize;
	private float horizontalWorldDistance;
	public float originalTileHeight;

	public float scaleChange;

	public int score = 0;

	public float timeInLevel = 0;
	public float timeLimit = 180;
	public bool levelIsTimed = true;

	public float minVolumeDifference;

	public Selection selectionManager;

	public float frostingHeightMultiplier = .5f;  // multiplied by single cake tier height to get frosting height
	public Material chocolate;
	public Material vanilla;

	List<int[,]> twoTilePieces = new List <int[,]>();
	List<int[,]> fourTilePieces = new List <int[,]>();
	List<int[,]> fiveTilePieces = new List <int[,]>();
	List<int[,]> threeTilePieces = new List <int[,]>();

	//two tile pieces
	int[,] line2Coordinates = {{ 0, 0 }, { 0, 1 }};

	//three tile pieces
	int[,] line3Coordinates = {{ 0, 0 }, { 0, 1 }, { 0, 2 }}; 
	
	int[,] corner3Coordinates = {{ 0, 0 }, { 0, 1 }, { 1, 0 }}; 
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




	// Use this for initialization
	void Awake () {

		cubeTile = (GameObject)Resources.Load("cube");
		isoTile = (GameObject)Resources.Load("iso");
		roundTile = (GameObject)Resources.Load("round");

		MeshRenderer tileRenderer = (MeshRenderer)isoTile.GetComponent (typeof(MeshRenderer));
		originalTileSize = tileRenderer.bounds.extents.x * 2;
//		Debug.Log (originalTileSize);
		originalTileHeight = tileRenderer.bounds.extents.z * 2;
//		Debug.Log (originalTileHeight);

		leftScreen = Camera.main.ScreenToWorldPoint (new Vector3( 0, 0, 0 ));
		leftScreen = new Vector3 (leftScreen.x, 0, 0);
		rightScreen = Camera.main.ScreenToWorldPoint (new Vector3(Screen.width, 0, 0 ));
		horizontalWorldDistance = Mathf.Abs (rightScreen.x - leftScreen.x);


	}

	void Start()
	{
		GameData.dataControl.Load ();
		currentLevel = GameData.dataControl.previousFinalLevel;
		Debug.Log ("starting level : " + currentLevel);
		scoreDisplay.text = "Score : " + score;
		levelDisplay.text = "Level " + ( currentLevel + 1 );

		twoTilePieces.Add (line2Coordinates);

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

		UpdateLevelingStats ();
		selectionManager.ShowNewProblem();


	}
	
	// Update is called once per frame
	void Update () {
		ChangeLevel ();

		if(levelIsTimed)
		{
			TimeLevel();
		}
	}


	void TimeLevel()
	{
		if (timeInLevel < timeLimit) 
		{
			timeInLevel += Time.deltaTime;

		}
		else
		{
			GameData.dataControl.previousFinalLevel = currentLevel;
			GameData.dataControl.Save ();
			//show "time up"
			finalScoreDisplay.text += " " + score;
			endGameDisplay.SetActive( true );
			levelIsTimed = false;
		}
	}



	public Material CakeColor( int index )
	{
		if( index % 2 == 0 )
		{
			return chocolate;
		}
		return vanilla;
	}

	public int[,] PieceCoordinates( int tileCount )
	{
		int random;
		switch (tileCount) 
		{
		case 2:
			random = Random.Range( 0, twoTilePieces.Count );
			return twoTilePieces[ random ];
		case 3:
			random = Random.Range( 0, threeTilePieces.Count );
			return threeTilePieces[ random ];
		case 4:
			random = Random.Range( 0, fourTilePieces.Count );
			return fourTilePieces[ random ];
		case 5:
			random = Random.Range( 0, fiveTilePieces.Count );
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
		float widthOfSpacer = ( horizontalWorldDistance - (totalTilesAcross * maxTileSize)) / (tierCount + 1 );
		List<Vector3> cakeTierPositions = new List<Vector3> ();
		Vector3 startPositionOfTileSpace = new Vector3(leftScreen.x, -2, 0 );

		for (int tier = 0; tier< tierCount; tier++) 
		{
			float tilesFromLeft = cakeSizesInTiles[ tier ].x;
			startPositionOfTileSpace += new Vector3( widthOfSpacer + ((tilesFromLeft - .5f) * maxTileSize), 0, 0);
			cakeTierPositions.Add (startPositionOfTileSpace);
			startPositionOfTileSpace += new Vector3( .5f * maxTileSize, 0, 0 );
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


	public List<int> GetPossibleTileCounts()
	{

		List<int> tileCounts = new List<int>();
		for (int i = 0; i < possibleTileCounts.Count; i++) 
		{
			tileCounts.Add ( possibleTileCounts[ i ]);
		}
		
		return tileCounts;
	}

		public List<GameObject> GetAvailableTilesByLevel()
		{
			List<GameObject> possibleTiles = new List<GameObject>();
			
			for (int i = 0; i < availableTiles.Count; i++) 
			{
				possibleTiles.Add ( availableTiles[ i ]);
			}

			return possibleTiles;
		}
	

	public void UpdateLevelingStats()
	{
		possibleTileCounts = new List<int>();
		availableTiles = new List<GameObject>();


		possibleTileCounts.Add ( 2 );
		possibleTileCounts.Add ( 3 );
		numberOfTiers = 2;
		minVolumeDifference = .5f;
		availableTiles.Add (cubeTile);
		maxLayerInTier = 1;

		if (currentLevel >= 1 ) 
		{
			availableTiles.Add (isoTile);

		}
		if (currentLevel >= 2 ) 
		{
			maxLayerInTier = 2;

		}
		if (currentLevel >= 3 ) {

			numberOfTiers = 3;
		} 

		if (currentLevel >= 4 ) 
		{
			availableTiles.Add (roundTile);
			maxRoundTilesPerTier = 1;
			//limit to 1 round tile per layer
		}
		if ( currentLevel >= 5 )
		{
			minVolumeDifference = .215f;
		}
		if (currentLevel >= 8 ) 
		{
			possibleTileCounts.Add ( 4 );

		}
		if (currentLevel >= 9) 
		{

		}

		if( currentLevel >= 10 )
		{
			maxRoundTilesPerTier = 2;

		}

		if (currentLevel >= 13) 
		{
			maxLayerInTier = 3;
		}
		if (currentLevel >= 14 ) 
		{
			possibleTileCounts.Add ( 5 );
		}

		if (currentLevel >= 15 ) 
		{
			numberOfTiers = 4;
		}
		if (currentLevel >= 18 ) 
		{
			minVolumeDifference = .2f;
		}
		if( currentLevel >= 20 )
		{
			maxRoundTilesPerTier = 3;
			minVolumeDifference = .1f;
		}
		if( currentLevel >= 22 )
		{
			numberOfTiers = 5;
		}
		
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

		levelDisplay.text = "Level " + ( currentLevel + 1 );
		UpdateLevelingStats ();
	}

	public void UpdateStatsAndLevel( bool correctAnswer )
	{
		if (correctAnswer) {
			int scoreIncrease = ( currentLevel + 1 ) * pointsForCorrect;
			score += scoreIncrease;
			scoreDisplay.text = "Score : " + score;

			//if entire problem is completed
			if( selectionManager.currentCakeTiers.Count == 0 )
			{
				correctAnswersInARow ++;
				if (correctAnswersInARow == correctInARowForLevelUp) 
				{
					UpdateLevel( 1 );
				}
			}
		} 
		else 
		{
			UpdateLevel( -1 );
		}
	}
	

	void UpdateLevel( int levelChange )
	{
		switch (levelChange) 
		{
		case 1:
			//increase 1 level, not over max level
			currentLevel = Mathf.Min ( maxLevel, currentLevel + 1);
			Debug.Log ("LEVELED UP ");
			break;
		case -1:
			currentLevel = Mathf.Max ( minLevel, currentLevel - 1);
			Debug.Log ("LEVELED DOWN ");
			break;
		default:
			//no level change
			break;
		}
		correctAnswersInARow = 0;
		UpdateLevelingStats();
		levelDisplay.text = "Level " + ( currentLevel + 1 );

	}

	public void ResetGame()
	{
		GameData.dataControl.previousFinalLevel = 0;
		GameData.dataControl.Save ();
		Application.LoadLevel ("Cake");
	}

	public void PlayAgain()
	{
//		GameData.dataControl.previousFinalLevel = currentLevel;
//		GameData.dataControl.Save ();
		Application.LoadLevel ("Cake");
	}



}
