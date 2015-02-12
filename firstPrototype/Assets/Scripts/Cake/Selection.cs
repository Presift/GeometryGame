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
//	Vector3 comparisonLabelPosition;

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
//		comparisonLabelPosition = new Vector3 (0f, 5f, 0f);
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

//		int numberOfTiers = gameManager.NumberOfTiersByLevel ();
		int numberOfTiers = 5;

		//calculate new tile sizes
		float tileSize = gameManager.MaxTileSize( numberOfTiers * 4 );
		float scaleChange = gameManager.ScaleChange( tileSize );
		float tileHeight = gameManager.TileHeight( scaleChange );

		for (int newTier = 0; newTier < numberOfTiers; newTier++) 
		{
			//get position for new tier
			Vector3 startPosition = gameManager.CakeTierStartPosition( numberOfTiers, newTier );

			//create new cake tier
			GameObject newCakeTier = (GameObject)Instantiate( cakeTier, new Vector3( 0, 0, 0), Quaternion.identity);
			newCakeTier.name = "Cake Tier " + newTier;

			//parent newCakeTier to this GO
			newCakeTier.transform.parent = this.gameObject.transform;

			//create cake layers on cake tier
			CakeTier tierScript = (CakeTier)newCakeTier.GetComponent(typeof(CakeTier));
			tierScript.manager = gameManager;
			tierScript.selectionManager = this;
			tierScript.NewCakeTier( 2, tileSize, tileHeight, scaleChange, gameManager.cubeTile, startPosition, gameManager.GetTetrisShapes(), tierScript );


			currentCakeTiers.Add ( tierScript );
			volumeOrder.Add ( tierScript.volume );

		}
		
		SetVolumeOrder();


		//unlock answer input
		LockInput (false);
	
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
//		for (int i = 0; i < volumeOrder.Count; i ++) 
//		{
//			Debug.Log ( "volume : " + volumeOrder[ i ]);	
//		}
	}
	

	public void IsSelectionCorrect( CakeTier selection )
	{
		Vector3 feedbackPosition;


		feedbackPosition = new Vector3( selection.centerPosition.x, selection.centerPosition.y, -1 );
	
		//if selection is largest volume
		if( selection.volume ==  volumeOrder[ 0 ] )
	   	{
			//remove selection from volumeOrder and current cake tiers
//			Debug.Log (cakePlate.cakePlatePosition);
			selection.positionOnCakePlate = cakePlate.TierPositionOnPlate( selection.tierHeight );
			selection.MoveToCakePlate( cakePlate );
			volumeOrder.RemoveAt( 0 );
			currentCakeTiers.Remove( selection );
			feedbackImage = correctAnswerImage;
			gameManager.UpdateStatsAndLevel( true );
		}

		else

		{
			feedbackImage = incorrectAnswerImage;
			gameManager.UpdateStatsAndLevel( false );
		}

		ShowFeedback (feedbackPosition);
	}

	void ShowFeedback( Vector3 position )
	{
		//lock answer input
		LockInput ( true );

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

				//prompt new problem
				ShowNewProblem();
			}
			else
			{
				LockInput ( false );
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

//	public void EqualButtonPressed ()
//	{
//		IsSelectionCorrect (null);
//	}

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
