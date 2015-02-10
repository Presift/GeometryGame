using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameModel : MonoBehaviour 
{
	public GameObject mainCamera;
	public GameObject canvas;

	public Text scoreDisplay;

	public int pointsForCorrect;

	public int minLevel = 0;
	public int maxLevel = 6;
	public int currentLevel = 0;

	public int correctAnswersInARow = 0;
	public int correctInARowForLevelUp = 3;

	public GameObject cubeTile;
	GameObject isoTile;
	GameObject roundTile;

	public float tileSize;
	
	public float scaleChange;

	public int score = 0;

	// Use this for initialization
	void Awake () {
		tileSize = setMaxTileSize ();

		cubeTile = (GameObject)Resources.Load("cube");
		isoTile = (GameObject)Resources.Load("iso");
		roundTile = (GameObject)Resources.Load("round");

		//get scale change
//		SpriteRenderer tileRenderer = (SpriteRenderer)squareTile.GetComponent(typeof(SpriteRenderer));
		MeshRenderer tileRenderer = (MeshRenderer)cubeTile.GetComponent (typeof(MeshRenderer));
		float currentTileSize = tileRenderer.bounds.extents.x * 2;
		Debug.Log (currentTileSize);
		
		scaleChange = tileSize / currentTileSize;
		Debug.Log (scaleChange);

	}

	void Start()
	{
		scoreDisplay.text = "Score : " + score;
	}
	
	// Update is called once per frame
//	void Update () {
//		ChangeLevel ();
//	}

	float setMaxTileSize()
	{
		//get height of world from camera
		float height = Camera.main.orthographicSize * 2.0f;
		float width = height * Screen.width/Screen.height;
		float maxTileSize = width/ 10;
		
		return maxTileSize;
	}

	public List<GameObject> GetAvailableTiles()
	{
		List<GameObject> availableTiles = new List<GameObject>();
		
		availableTiles.Add (cubeTile);
		availableTiles.Add (isoTile);

		if (currentLevel >= 2) 
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


	public List<string> GetTetrisShapes()
	{
		List<string> tetrisShapes = new List<string> ();
		tetrisShapes.Add ("line");
		tetrisShapes.Add ("square");

		if (currentLevel >= 1 ) 
		{
			tetrisShapes.Add ("LShape");
		}

		return tetrisShapes;
	}
//
//	void ChangeLevel()
//	{
//		if (Input.GetKeyDown (KeyCode.UpArrow)) 
//		{
//			currentLevel = Mathf.Min( currentLevel + 1, maxLevel );
//			Debug.Log ("CURRENT LEVEL: " + currentLevel);
//		}
//		if (Input.GetKeyDown (KeyCode.DownArrow)) 
//		{
//			currentLevel = Mathf.Max (0, currentLevel - 1);
//			Debug.Log ("CURRENT LEVEL: " + currentLevel);
//		}
//	}
//
//	public void UpdateStatsAndLevel( bool correctAnswer )
//	{
//		if (correctAnswer) {
//
//			correctAnswersInARow ++;
//			int scoreIncrease = ( currentLevel + 1 ) * pointsForCorrect;
//			score += scoreIncrease;
//			scoreDisplay.text = "Score : " + score;
//		} 
//		else 
//		{
//			correctAnswersInARow = 0;
//			currentLevel = Mathf.Max(minLevel, currentLevel - 1 );
//			Debug.Log ("leveled down");
//		}
//
//		UpdateLevel ();
//	}
//
//	void UpdateLevel()
//	{
//		if (correctAnswersInARow == correctInARowForLevelUp) 
//		{
//			currentLevel = Mathf.Min ( maxLevel, currentLevel + 1);
//			correctAnswersInARow = 0;
//			Debug.Log ("leveled up");
//		}
//
//	}

}
