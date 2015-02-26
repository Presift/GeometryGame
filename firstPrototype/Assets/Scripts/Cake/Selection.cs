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
	public GameObject arrow;

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

		float minVolDiff = gameManager.minVolumeDifference;
		int numberOfTiers = gameManager.numberOfTiers;
		int maxRoundTilesPerTier = gameManager.maxRoundTilesPerTier;

		List <GameObject> availableTiles = gameManager.GetAvailableTilesByLevel ();

		List<int> possibleTileCounts = new List<int>();

		List<int[,]> allLayerCoordinates = new List<int[,]> ();
		List<Vector2> layerSizes = new List<Vector2> ();
		int totalTilesAcross = 0;
		int totalTilesDown = 0;
		for (int newTier = 0; newTier < numberOfTiers; newTier++) 
		{

			if( possibleTileCounts.Count == 0 )
			{
				possibleTileCounts = gameManager.GetPossibleTileCounts ();

			}
			int random = Random.Range (0, possibleTileCounts.Count );
			int tileCount = possibleTileCounts[ random ];
			possibleTileCounts.RemoveAt ( random );
			int[,] pieceCoordinates = gameManager.PieceCoordinates( tileCount );
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
		cakePlate.singleTierHeight = tileHeight * (1 + gameManager.frostingHeightMultiplier );

		List<Vector3> tierStartPositions = gameManager.CakeTierStartPositions (tileSize, layerSizes, totalTilesAcross);

		for (int newTier = 0; newTier < numberOfTiers; newTier++) 
		{
			int[,] pieceCoordinates = allLayerCoordinates[newTier];

			int tilesAcross = (int)layerSizes[ newTier ].x;
			int tilesDown = (int)layerSizes[ newTier ].y;

			//get position for new tier
			Vector3 startPosition = tierStartPositions[ newTier ];

			Material cakeColor = gameManager.CakeColor( newTier );
			//create new cake tier
			GameObject newCakeTier = (GameObject)Instantiate( cakeTier, new Vector3( 0, 0, 0), Quaternion.identity);
			newCakeTier.name = "Cake Tier " + newTier;

			//parent newCakeTier to this GO
			newCakeTier.transform.parent = this.gameObject.transform;

			//create cake layers on cake tier
			CakeTier tierScript = (CakeTier)newCakeTier.GetComponent(typeof(CakeTier));
			tierScript.manager = gameManager;
			tierScript.selectionManager = this;
			tierScript.NewCakeTier( pieceCoordinates, tileSize, tileHeight, scaleChange, gameManager.cubeTile, startPosition, tierScript, availableTiles, volumeOrder, cakeColor, gameManager.frostingHeightMultiplier, minVolDiff, maxRoundTilesPerTier );


			currentCakeTiers.Add ( tierScript );
			volumeOrder.Add ( tierScript.volume );

		}

		//cake tiers ordered by smallest to largest volumes
		currentCakeTiers.Sort (
			delegate ( CakeTier tier1, CakeTier tier2)
			{
				return tier1.volume.CompareTo( tier2.volume );
			}
		);

		List<int> bestLayerConfigs = new List<int>();

		volumeOrder.Sort ();
		List<float> originalVolumes = volumeOrder;  //sorted from smallest to greatest
		float bestVolumeDiff = AverageVolumeDiff ( volumeOrder, minVolDiff );

		int maxLayer = gameManager.maxLayerInTier;
		int iterations = ( int ) Mathf.Pow (maxLayer, numberOfTiers);

		for (int configAttempt = iterations -1; configAttempt > 0; configAttempt-- ) 
		{
			int remainder = configAttempt;
			List<int> newLayerConfig = new List<int>();
			List<float> newVolumes = new List<float>();

			for ( int tier = numberOfTiers; tier > 0; tier-- )
			{
				int addLayerCount = remainder / (int)Mathf.Pow( maxLayer, ( tier - 1 ));
				remainder -= addLayerCount * (int)Mathf.Pow ( maxLayer, (tier - 1 ));
				newLayerConfig.Add ( addLayerCount );
				float newLayerVolume = ( addLayerCount + 1 ) * originalVolumes[ numberOfTiers - tier ];
				newVolumes.Add ( newLayerVolume );
			}

			newVolumes.Sort ();

			float newAvgVolumeDiff = AverageVolumeDiff( newVolumes, minVolDiff );
			//if avg volume diff is not below min diff and is less than best volume diff
			if( newAvgVolumeDiff!= - 1 )
			{
				if( newAvgVolumeDiff < bestVolumeDiff )
				{
					bestLayerConfigs = newLayerConfig;
					bestVolumeDiff = newAvgVolumeDiff;
					volumeOrder = newVolumes;
				}
				else if( newAvgVolumeDiff == bestVolumeDiff && bestLayerConfigs.Count == 0 )
				{
					Debug.Log ( " best min diff was equal ");
					bestLayerConfigs = newLayerConfig;
					bestVolumeDiff = newAvgVolumeDiff;
					volumeOrder = newVolumes;
				}
			}

		}

		//set cake layers based on optimal differences

		if( bestLayerConfigs.Count != 0 )
		{
			for ( int tier = 0; tier < numberOfTiers; tier++ )
			{
				int additionalLayer = bestLayerConfigs[ tier ];
				currentCakeTiers[ tier ].MakeAdditionalLayers( additionalLayer );
				currentCakeTiers[ tier ].volume *= ( additionalLayer + 1 );
			}
		}


		SetVolumeOrder();

		//unlock answer input
		LockInput (false);
	
	}

	float AverageVolumeDiff( List<float> orderedVolumes, float minDifference )
	{
		float totalVolumeSpaces = 0;
		for (int volume = 1; volume < orderedVolumes.Count; volume++) 
		{
			float volumeDiff = orderedVolumes[ volume ] - orderedVolumes[ volume - 1 ];
			totalVolumeSpaces += volumeDiff;
			if( volumeDiff < minDifference )
			{
				return -1;
			}
		}
		float avgVolumeDiff = totalVolumeSpaces / (orderedVolumes.Count - 1);
		return avgVolumeDiff;
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
		GameObject feedbackImage;

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
			ShowFeedback ( feedbackImage, feedbackPosition, 0 );
			return true;
		}
		else
		{
			LockInput ( true );
			feedbackImage = incorrectAnswerImage;
			//show correct answer
			gameManager.UpdateStatsAndLevel( false );
			ShowFeedback ( feedbackImage, feedbackPosition, -1 );
			ShowCorrectAnswer();
			return false;
		}
	}

	void ShowCorrectAnswer()
	{
		//get correct cake
		CakeTier correctSelection = null;
		Vector3 feedbackPosition;

		for(  int cakeTier = 0; cakeTier < currentCakeTiers.Count; cakeTier ++ )
		{
			if( currentCakeTiers[ cakeTier ].volume == volumeOrder[ 0 ] )
			{
				correctSelection = currentCakeTiers [ cakeTier ];
			}
		}


		feedbackPosition = new Vector3( correctSelection.centerPosition.x, correctSelection.centerPosition.y - (correctSelection.tierHeight / 2 ), -1 );
		ShowFeedback ( arrow, feedbackPosition, -1 );
	}

	void ShowFeedback( GameObject feedbackImage, Vector3 position, float startTime )
	{
		feedbackImage.transform.position = position;
		feedbackImage.SetActive (true);
		//start countdown
		showFeedback = true;
		timeShowingFeeback = startTime;
	}

	void FeedbackTimer()
	{
		if (timeShowingFeeback < timeToShowFeedback) 
		{
			timeShowingFeeback += Time.deltaTime;
		}
		else
		{
			bool correctAnswer = !arrow.activeInHierarchy;

			WipeFeedback();
			showFeedback = false;

			NextStep( correctAnswer );

		}
	}

	void NextStep( bool correctAnswer )
	{
		if( !correctAnswer )
		{
			ShowNewProblem();
			return;
		}

		if( volumeOrder.Count == 0 )
		{
			//show final cake
			cakePlate.rotating = true;
		}
		return;
	}
	
	public void WipeFeedback()
	{
		correctAnswerImage.SetActive ( false );
		incorrectAnswerImage.SetActive ( false );
		arrow.SetActive (false);
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
