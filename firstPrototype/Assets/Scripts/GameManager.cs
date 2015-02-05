using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public GameObject mainCamera;
	public GameObject canvas;

	public int pointsForCorrect;

	public int minLevel = 0;
	public int maxLevel = 6;
	public int currentLevel = 0;

	public int correctAnswersInARow = 0;
	public int correctInARowForLevelUp = 3;

	GameObject squareTile;
	GameObject rightTriangleTile;
	GameObject isosceles;
	GameObject circle;
	GameObject twoTriangles;

	public float tileSize;
	
	public float scaleChange;

	public int score = 0;

	// Use this for initialization
	void Awake () {
		tileSize = setMaxTileSize ();

		squareTile = (GameObject)Resources.Load("Square");
		rightTriangleTile = (GameObject)Resources.Load("RightTriangle");
		isosceles = (GameObject)Resources.Load("Isosceles");
		circle = (GameObject)Resources.Load("Circ");
		twoTriangles = (GameObject)Resources.Load("2RightTriangles");

		//get scale change
		SpriteRenderer tileRenderer = (SpriteRenderer)squareTile.GetComponent(typeof(SpriteRenderer));
		float currentTileSize = tileRenderer.bounds.extents.x * 2;
		
		scaleChange = tileSize / currentTileSize;

	}

	void Start(){
	}
	
	// Update is called once per frame
	void Update () {
		ChangeLevel ();
	}

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
		
		availableTiles.Add (squareTile);
		availableTiles.Add (rightTriangleTile);

		if (currentLevel >= 2) 
		{
			availableTiles.Add (circle);
		}

		if(currentLevel >= 4 )
		{
			availableTiles.Add (isosceles);
		}

		if (currentLevel >= 6)
		{
			availableTiles.Add (twoTriangles);
		}
		
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

			correctAnswersInARow ++;
			int scoreIncrease = ( currentLevel + 1 ) * pointsForCorrect;
			score += scoreIncrease;
		} 
		else 
		{
			correctAnswersInARow = 0;
			currentLevel = Mathf.Max(minLevel, currentLevel - 1 );
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
