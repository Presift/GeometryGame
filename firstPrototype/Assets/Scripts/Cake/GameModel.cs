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
	public int minLayersInTier = 0;
	public int maxLayersInTier = 4;
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

	}
	
	// Update is called once per frame
	void Update () {
		ChangeLevel ();

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			selectionManager.ShowNewProblem();
		}
	}
	
	public Vector3 CakeTierStartPosition( int tierCount, int tierIndex )
	{
		float distanceFromLeft = (horizontalWorldDistance / (tierCount + 1)) * (tierIndex + 1);
		Vector3 tierPosition = new Vector3 (distanceFromLeft, -2, 0) + leftScreen;
		return tierPosition;
	}

	public float MaxTileSize( int tilesToFit )
	{
		//get height of world from camera
		float height = Camera.main.orthographicSize * 2.0f;
		float width = height * Screen.width/Screen.height;
		float maxTileSize = width/ tilesToFit;
		
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
