using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Selection : MonoBehaviour {
	
	public List<float> volumeOrder = new List<float> ();
	public List<CakeTier> currentCakeTiers = new List<CakeTier>();

	public GameModel gameManager;
	public CakePlate cakePlate;

	public Text comparisonLabel;

	public GameObject correctAnswerImage;
	public GameObject incorrectAnswerImage;

	GameObject feedbackImage;

	bool showFeedback = false;

	public float timeToShowFeedback;


	float timeShowingFeeback;

	CakeLayer largerArea;
	CakeLayer largerPerimeter;

	GameObject cakeLayer;
	GameObject cakeTier;


	// Use this for initialization
	void Start () {
		cakeLayer = (GameObject)Resources.Load("Cake Layer");
		cakeTier = (GameObject)Resources.Load ("Cake Tier");
//		ShowNewProblem ();
	}
	
	// Update is called once per frame
	void Update () {
		if (showFeedback)
		{
			FeedbackTimer();
		}

	}

	public void ShowNewProblem()
	{
		WipePreviousProblem ();
		cakePlate.WipeCakePlate ();

		int numberOfTiers = gameManager.NumberOfTiersByLevel ();

		List <GameObject> availableTiles = gameManager.GetAvailableTilesByLevel ();

		List<int> possibleTileCounts = gameManager.GetPossibleTileCountsByLevel ();

		List<int[,]> allLayerCoordinates = new List<int[,]> ();
		List<Vector2> layerSizes = new List<Vector2> ();
		int totalTilesAcross = 0;
		int totalTilesDown = 0;
		for (int newTier = 0; newTier < numberOfTiers; newTier++) 
		{
			int random = Random.Range (0, possibleTileCounts.Count );
			int[,] pieceCoordinates = GetPieceCoordinatesFromTileCount( possibleTileCounts, random );
			allLayerCoordinates.Add ( pieceCoordinates );
			Vector2 layerSizeInTiles = GetSizeOfPieceFromCoordinates( pieceCoordinates );
			layerSizes.Add (layerSizeInTiles);
			totalTilesAcross += (int)layerSizeInTiles.x;
			totalTilesDown += (int)layerSizeInTiles.y;
		}

		//calculate new tile sizes
		float tileSize = gameManager.MaxTileSize( numberOfTiers, totalTilesAcross, totalTilesDown );
		float scaleChange = gameManager.ScaleChange( tileSize );
		float tileHeight = gameManager.TileHeight( scaleChange );
		cakePlate.singleTierHeight = tileHeight * 1.25f;

		List<Vector3> tierStartPositions = gameManager.CakeTierStartPositions (tileSize, layerSizes, totalTilesAcross);

		for (int newTier = 0; newTier < numberOfTiers; newTier++) 
		{
			int[,] pieceCoordinates = allLayerCoordinates[newTier];
			int layerCount = gameManager.GetLayerCount( ( pieceCoordinates.Length/2) );

			int tilesAcross = (int)layerSizes[ newTier ].x;
			int tilesDown = (int)layerSizes[ newTier ].y;

			//get position for new tier
			Vector3 startPosition = tierStartPositions[ newTier ];


			//create new cake tier
			GameObject newCakeTier = (GameObject)Instantiate( cakeTier, new Vector3( 0, 0, 0), Quaternion.identity);
			newCakeTier.name = "Cake Tier " + newTier;

			//parent newCakeTier to this GO
			newCakeTier.transform.parent = this.gameObject.transform;

			//create cake layers on cake tier
			CakeTier tierScript = (CakeTier)newCakeTier.GetComponent(typeof(CakeTier));
			tierScript.manager = gameManager;
			tierScript.selectionManager = this;
			tierScript.NewCakeTier( pieceCoordinates, layerCount, tileSize, tileHeight, scaleChange, gameManager.cubeTile, startPosition, tierScript, availableTiles, volumeOrder );


			currentCakeTiers.Add ( tierScript );
			volumeOrder.Add ( tierScript.volume );

		}
		
		SetVolumeOrder();

		//unlock answer input
		LockInput (false);
	
	}

	Vector2 GetSizeOfPieceFromCoordinates( int[,] pieceCoordinates )
	{
		int smallestX = 1000;
		int largestX = 0;
		int smallestY = 1000;
		int largestY = 0;

		for( int coordinate = 0; coordinate < ( pieceCoordinates.Length / 2 ); coordinate ++ )
		{
			int xCoord = pieceCoordinates[ coordinate, 0 ];
			int yCoord = pieceCoordinates[ coordinate, 1 ];

			//if x coordinate is smaller than smallest
			if( xCoord < smallestX )
			{
				smallestX = xCoord;
			}
			//if x coordinate is larger than largest
			if( xCoord > largestX )
			{
				largestX = xCoord;
			}
			//if y coordinate is smaller than smallest
			if( yCoord < smallestY )
			{
				smallestY = yCoord;
			}
			//if y coordinate is larger than largest
			if( yCoord > largestY )
			{
				largestY = yCoord;
			}
		}
		int tilesAcross = Mathf.Abs (largestX - smallestX);
		int tilesDown = Mathf.Abs (largestY - smallestY);
		return new Vector2 (tilesAcross + 1 , tilesDown + 1);
	}

	int[,] GetPieceCoordinatesFromTileCount( List<int> tileCounts, int random )
	{
		int tileCount = tileCounts[ random ];
		int[,] pieceCoordinates = gameManager.PieceCoordinates( tileCount );
		return pieceCoordinates;
	}

	void WipePreviousProblem()
	{
		DestroyCurrentChildren ();
		currentCakeTiers = new List<CakeTier> ();
		volumeOrder = new List<float> ();

	}


	void SetVolumeOrder()  //order is from largest volume to smallest
	{
		volumeOrder.Sort ();
		volumeOrder.Reverse ();
	}
	

	public bool IsSelectionCorrect( CakeTier selection )
	{
		Vector3 feedbackPosition;


		feedbackPosition = new Vector3( selection.centerPosition.x, selection.centerPosition.y, -1 );
	
		//if selection is largest volume ( correct )
		if( selection.volume ==  volumeOrder[ 0 ] )
	   	{
			//remove selection from volumeOrder and current cake tiers
			selection.positionOnCakePlate = cakePlate.TierPositionOnPlate( selection.tierHeight );
			selection.MoveToCakePlate( cakePlate );
			volumeOrder.RemoveAt( 0 );
			currentCakeTiers.Remove( selection );
			feedbackImage = correctAnswerImage;
			gameManager.UpdateStatsAndLevel( true );
			ShowFeedback (feedbackPosition);
			return true;
		}
		else
		{
			feedbackImage = incorrectAnswerImage;
			gameManager.UpdateStatsAndLevel( false );
			ShowFeedback (feedbackPosition);
			return false;
		}
	}

	void ShowFeedback( Vector3 position )
	{
		//lock answer input
//		LockInput ( true );

		feedbackImage.transform.position = position;
		feedbackImage.SetActive (true);
		//start countdown
		showFeedback = true;
		timeShowingFeeback = 0;
	}

	void FeedbackTimer()
	{
		if (timeShowingFeeback < timeToShowFeedback) 
		{
			timeShowingFeeback += Time.deltaTime;
		}
		else
		{
			feedbackImage.SetActive( false );
			showFeedback = false;

			//if volumeOrder list is empty
			if( volumeOrder.Count == 0 )
			{
				//show final cake
				cakePlate.rotating = true;

			}
			else
			{
//				LockInput ( false );
			}
				
		}
	}

	void LockInput( bool locked )
	{
			//unlock input on all cake tiles in current cakes
			for( int tier = 0; tier < currentCakeTiers.Count; tier++ )
			{
				currentCakeTiers[ tier ].inputLocked = ( locked );
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
