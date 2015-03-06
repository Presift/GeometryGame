using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Feedback : MonoBehaviour {

	public float takeAwayFrequency = 1.0f;

	public Selection selection;
	public GameObject correctAnswerImage;
	public GameObject incorrectAnswerImage;
	public GameObject arrow;
	
	bool showFeedback = false;
	bool timeCorrectAnswer = false;
	float timeToShowCorrectAnswer;

	bool incorrectAnswerShown = false;

	public float timeToShowFeedback;
	
	
	float timeShowingFeeback;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (showFeedback)
		{
			FeedbackTimer();
		}

		if( timeCorrectAnswer )
		{
			CorrectAnswerTimer( timeToShowCorrectAnswer );
		}
	}

	public void WipeFeedback()
	{
		correctAnswerImage.SetActive ( false );
		incorrectAnswerImage.SetActive ( false );
		arrow.SetActive (false);
	}

	public void ShowCorrectAnswer()
	{
		//get correct cake
		CakeTier correctSelection = null;
		Vector3 feedbackPosition;
		
		for(  int cakeTier = 0; cakeTier < selection.currentCakeTiers.Count; cakeTier ++ )
		{
			if( selection.currentCakeTiers[ cakeTier ].volume == selection.volumeOrder[ 0 ] )
			{
				correctSelection = selection.currentCakeTiers [ cakeTier ];
			}
		}

		feedbackPosition = new Vector3( correctSelection.centerPosition.x, correctSelection.centerPosition.y - (correctSelection.tierHeight / 2 ), -1 );
		ShowFeedback ( false, feedbackPosition, -1 );
	}

	void CorrectAnswerTimer( float startTime )
	{
		if ( Time.time >= startTime ) 
		{
			timeCorrectAnswer = false;
			ShowCorrectAnswer();
		}
	}
	
	public void ShowFeedback( bool correctAnswer, Vector3 position, float startTime )
	{
		GameObject feedbackImage;
		if (correctAnswer) 
		{
			feedbackImage = correctAnswerImage;
		}
		else
		{
			if( incorrectAnswerShown )
			{
				feedbackImage = arrow;
				incorrectAnswerShown = false;
			}
			else
			{
				feedbackImage = incorrectAnswerImage;
				incorrectAnswerShown = true;
			}

		}
		feedbackImage.transform.position = position;
		feedbackImage.SetActive (true);
		//start countdown
		showFeedback = true;
		timeShowingFeeback = startTime;  
	}
	
	void FeedbackTimer()
	{
//		Debug.Log ("FeedbackTimer");
		if (timeShowingFeeback < timeToShowFeedback) 
		{
			timeShowingFeeback += Time.deltaTime;
		}
		else
		{
			bool activeArrow = arrow.activeInHierarchy;
			bool activeIncorrect = incorrectAnswerImage.activeInHierarchy;

			bool correctAnswer = !activeArrow && !activeIncorrect;
			 

			if( correctAnswer || ( !correctAnswer && activeArrow ))
			{
				selection.NextStep( correctAnswer );
			}
			else
			{
				timeToShowCorrectAnswer = TakeAwaySimilarities( selection.currentCakeTiers );
				timeCorrectAnswer = true;

			}

			WipeFeedback();
			
			showFeedback = false;
	
			
		}
	}

	public float TakeAwaySimilarities( List<CakeTier> remainingTierScripts )
	{
		Debug.Log ("Take away ");
		float startTime = Time.time;
		startTime += ( TakeAwaySharedCubes (remainingTierScripts, startTime)) * takeAwayFrequency;
		startTime += ( TakeAwaySharedIsos (remainingTierScripts, startTime)) * takeAwayFrequency;
		startTime += ( TakeAwaySharedRounds (remainingTierScripts, startTime)) * takeAwayFrequency;

		return startTime;

	}

	int TakeAwaySharedCubes( List<CakeTier> remainingTierScripts, float startTime )
	{

		int fewestCubes = 100;
		for (int script = 0; script < remainingTierScripts.Count; script ++)
		{
			int cubeCount = remainingTierScripts[ script ].squareTiles.Count;
			if( cubeCount < fewestCubes )
			{
				fewestCubes = cubeCount;
			}
		}

		for ( int cube = 0; cube < fewestCubes; cube ++ )
		{
			for (int script = 0; script < remainingTierScripts.Count; script ++)
			{
				//remove last two isos from iso tiles list (because these compose one square tile
//				int isoTilesRemovalIndex = remainingTierScripts[ script ].isoTiles.Count - 1;
//				remainingTierScripts[ script ].isoTiles.RemoveAt( isoTilesRemovalIndex );
//				remainingTierScripts[ script ].isoTiles.RemoveAt( isoTilesRemovalIndex - 1 );

				remainingTierScripts[ script ].isoTiles.RemoveAt( 0 );
				remainingTierScripts[ script ].isoTiles.RemoveAt( 0 );

//				int indexOfTopmostCube = remainingTierScripts[ script ].squareTiles.Count - ( cube + 1 );
//				GameObject cubeToRemove = remainingTierScripts[ script ].squareTiles[ indexOfTopmostCube ];
				GameObject cubeToRemove = remainingTierScripts[ script ].squareTiles[ 0 ];
				remainingTierScripts[ script ].squareTiles.RemoveAt( 0 );
				float startFadeOutTime = startTime + ( cube * takeAwayFrequency );
				Debug.Log ( "cube index : " + cube + " , script : " + remainingTierScripts[ script ].name + ", name : " + cubeToRemove.name );
				cubeToRemove.SendMessage("StartFadeOut", startFadeOutTime );

      		}
		}

		Debug.Log ("FEWEST CUBES : " + fewestCubes);
		return fewestCubes;

	}

	int TakeAwaySharedIsos( List<CakeTier> remainingTierScripts, float startTime )
	{
		int fewestIsos = 100;
		for (int script = 0; script < remainingTierScripts.Count; script ++)
		{
			int isoCount = remainingTierScripts[ script ].isoTiles.Count;
			if( isoCount < fewestIsos )
			{
				fewestIsos = isoCount;
			}
		}
		Debug.Log ("FEWEST ISOS :" + fewestIsos);
		
		for ( int iso = 0; iso < fewestIsos; iso ++ ) 
		{
			for (int script = 0; script < remainingTierScripts.Count; script ++)
			{
//				int indexOfTopmostIso = remainingTierScripts[ script ].isoTiles.Count - ( iso + 1 );
				GameObject isoToRemove = remainingTierScripts[ script ].isoTiles[ 0 ];
				remainingTierScripts[ script ].isoTiles.RemoveAt( 0 );

//				int indexOfTopmostIso = remainingTierScripts[ script ].isoTiles.Count - ( iso + 1 );
//				GameObject isoToRemove = remainingTierScripts[ script ].isoTiles[ indexOfTopmostIso ];

//				remainingTierScripts[ script ].isoTiles.RemoveAt( indexOfTopmostIso );
				Debug.Log ( "iso index : " + iso + " , script : " + remainingTierScripts[ script ].name );
				float startFadeOutTime = startTime + ( iso * takeAwayFrequency );
				isoToRemove.SendMessage("StartFadeOut", startFadeOutTime );
				
			}
		}
//		Debug.Log ("fewest isos : " + fewestIsos);
		return fewestIsos;
	}

	int TakeAwaySharedRounds( List<CakeTier> remainingTierScripts, float startTime )
	{
		int fewestRounds = 100;
		for (int script = 0; script < remainingTierScripts.Count; script ++)
		{
			int isoCount = remainingTierScripts[ script ].roundTiles.Count;
			if( isoCount < fewestRounds )
			{
				fewestRounds = isoCount;
			}
		}
		
		for ( int iso = 0; iso < fewestRounds; iso ++ )
		{
			for (int script = 0; script < remainingTierScripts.Count; script ++)
			{
//				int indexOfTopmostRound = remainingTierScripts[ script ].roundTiles.Count - ( iso + 1 );
//				GameObject roundToRemove = remainingTierScripts[ script ].roundTiles[ indexOfTopmostRound ];

//				int indexOfTopmostRound = remainingTierScripts[ script ].roundTiles.Count - ( iso + 1 );
				GameObject roundToRemove = remainingTierScripts[ script ].roundTiles[ 0 ];
				remainingTierScripts[ script ].roundTiles.RemoveAt( 0 );

				float startFadeOutTime = startTime + ( iso * takeAwayFrequency );
				Debug.Log ( startFadeOutTime );
				roundToRemove.SendMessage("StartFadeOut", startFadeOutTime );
				
			}
		}
//		Debug.Log (" fewest rounds " + fewestRounds);
		return fewestRounds;
	}
}
