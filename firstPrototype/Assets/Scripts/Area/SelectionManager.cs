using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {

	public TetrisPiece tetris1;
	public TetrisPiece tetris2;

	public GameManager gameManager;

	public Text comparisonLabel;
	Vector3 comparisonLabelPosition;

	public GameObject correctAnswerImage;
	public GameObject incorrectAnswerImage;

	GameObject feedbackImage;

	bool showFeedback = false;

	public float timeToShowFeedback;
	float timeShowingFeeback;

	TetrisPiece largerArea;
	TetrisPiece largerPerimeter;

	string currentComparison = "area";

	// Use this for initialization
	void Start () {
		comparisonLabelPosition = new Vector3 (0f, -2.5f, 0f);
		ShowNewProblem ();

		comparisonLabel.text = "choose the shape with the largest " + currentComparison;
	}
	
	// Update is called once per frame
	void Update () {

		ChangeCurrentComparisonFromInput ();


		if (showFeedback)
		{
			FeedbackTimer();
		}
	}

	void ShowNewProblem()
	{
		tetris1.SetUpPieceAndStats( gameManager.currentLevel, null );

		List<GameObject> usedTiles = tetris1.tilesUsed;
//		Debug.Log ("used Tile Count : " + usedTiles.Count);

		tetris2.SetUpPieceAndStats( gameManager.currentLevel, usedTiles );
		
		SetLargerAreaPiece();
		
		SetLargerPerimeterPiece();
	}



	void ChangeCurrentComparisonFromInput()
	{
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			currentComparison = "perimeter";
			comparisonLabel.text = "choose the shape with the largest " + currentComparison;
			Debug.Log ("PERIMETER");
		}
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			currentComparison = "area";
			comparisonLabel.text = "choose the shape with the largest " + currentComparison;
			Debug.Log ("AREA");
		}
	}

	void SetLargerAreaPiece()
	{
		float area1 = tetris1.area;
		float area2 = tetris2.area;

		if (area1 > area2) 
		{
			largerArea = tetris1;
//			Debug.Log (" area : " + area1);
		} 
		else if (area2 > area1) 
		{
			largerArea = tetris2;	
//			Debug.Log (" area : " + area2);
		}
		else
		{
			largerArea = null;
//			Debug.Log (" area : equal");
		}
	}

	void SetLargerPerimeterPiece()
	{
		float perimeter1 = tetris1.perimeter;
		float perimeter2 = tetris2.perimeter;

		if (perimeter1 > perimeter2) {
			largerPerimeter = tetris1;
//			Debug.Log (" perimeter : " + perimeter1);
		} 
		else if (perimeter2 > perimeter1) {
			largerPerimeter = tetris2;
//			Debug.Log (" perimeter : " + perimeter2);
		} 
		else 
		{
			largerPerimeter = null;
//			Debug.Log (" perimeter : equal");
		}
	}

	public void IsSelectionCorrect( TetrisPiece selection )
	{
		Vector3 feedbackPosition;

		if (selection == null) 
		{

			feedbackPosition = comparisonLabelPosition;

		}
		else
		{
			feedbackPosition = selection.centerPosition;
		}
//		Debug.Log (feedbackPosition);

		switch ( currentComparison ) 
		{
		case "area":
			if( selection == largerArea)
			{
				feedbackImage = correctAnswerImage;
				gameManager.UpdateStatsAndLevel( true );
			}
			else
			{
				feedbackImage = incorrectAnswerImage;
				gameManager.UpdateStatsAndLevel( false );
			}

			break;

		case "perimeter":
			if( selection == largerPerimeter)
			{
				feedbackImage = correctAnswerImage;
				gameManager.UpdateStatsAndLevel( true );

			}
			else
			{
				feedbackImage = incorrectAnswerImage;
				gameManager.UpdateStatsAndLevel( false );
			}
			break;
		}

		ShowFeedback (feedbackPosition);
	}

	void ShowFeedback( Vector3 position )
	{
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
			//prompt new problem
			ShowNewProblem();
		}
	}

	public void EqualButtonPressed ()
	{
		IsSelectionCorrect (null);
	}
}
