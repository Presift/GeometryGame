using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectionManager : MonoBehaviour {

	public TetrisPiece tetris1;
	public TetrisPiece tetris2;

	public Text comparisonMode;

	TetrisPiece largerArea;
	TetrisPiece largerPerimeter;

	int currentLevel = 0;
	int minLevel = 0;
	int maxLevel = 3;

	string currentComparison = "area";

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) 
			
		{
			tetris1.SetUpPieceAndStats( currentLevel );
			tetris2.SetUpPieceAndStats( currentLevel );

			SetLargerAreaPiece();

			SetLargerPerimeterPiece();
		}

		ChangeCurrentComparisonFromInput ();
		ChangeLevel ();
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

	void ChangeCurrentComparisonFromInput()
	{
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			currentComparison = "perimeter";
			Debug.Log ("PERIMETER");
		}
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			currentComparison = "area";
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

	public bool IsSelectionCorrect( TetrisPiece selection )
	{
		switch ( currentComparison ) 
		{
			case "area":
			if( selection == largerArea)
			{
				Debug.Log ("correct!");
				return true;
			}
			Debug.Log ("incorrect!");
			return false;

		case "perimeter":
			if( selection == largerPerimeter)
			{
				Debug.Log ("correct!");
				return true;
			}
			Debug.Log ("incorrect!");
			return false;
		}

		Debug.Log ("warning : No matching case");
		return false;
	}

	public void EqualButtonPressed ()
	{
		IsSelectionCorrect (null);
	}
}
