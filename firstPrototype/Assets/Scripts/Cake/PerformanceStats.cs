using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerformanceStats : MonoBehaviour {

	public float problemNum = 0;			//check
	public float possibleSelections;		//check
	public float responseTime;				//check
	public float timeSinceResponse;			//check
	public float avgResponseTime;
	public float avgVolumeDiff;				//check
	public float correctAnswer;				//check
	public float volumeDiffFromCorrectAnswer; //check
	public float currentLevel;				//check
	public float totalDifferentTilesFromCorrectAnswer;
	public float differenceInSquares;  //how many more in correct answer
	public float differenceInIsos;
	public float differenceInRounds;
	public float minLayerCount;
	public float maxLayerCount;




	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTimeSinceLastInput( float time )
	{
		timeSinceResponse = time;
	}

	public void CalculateResonseTime( float timeResponded )
	{
		responseTime = timeResponded - timeSinceResponse;
	}

	public void CalculateDifferences( CakeTier correctAnswer, CakeTier answer )
	{
		if (correctAnswer == answer) 
		{
			totalDifferentTilesFromCorrectAnswer = 0;
			differenceInIsos = 0;
			differenceInSquares = 0;
			differenceInRounds = 0;	
		}
		else
		{
			float squaresInCorrectTier = Mathf.Floor( correctAnswer.isoTiles.Count / 2 );
			float isosInCorrectTier = correctAnswer.isoTiles.Count - ( squaresInCorrectTier * 2 );

			float squaresInWrongTier = Mathf.Floor( answer.isoTiles.Count / 2 );
			float isosInWrongTier = answer.isoTiles.Count - ( squaresInWrongTier * 2 );

			differenceInSquares = squaresInCorrectTier - squaresInWrongTier;
			differenceInIsos = isosInCorrectTier - isosInWrongTier;
			differenceInRounds = correctAnswer.roundTiles.Count - answer.roundTiles.Count;

			totalDifferentTilesFromCorrectAnswer = Mathf.Abs( differenceInIsos) + Mathf.Abs( differenceInRounds ) + Mathf.Abs( differenceInSquares );
		}

	}

	public void SaveStats()
	{
		string data = problemNum.ToString ();
		data += "," + possibleSelections.ToString ();
		data += "," + responseTime.ToString ();
		data += "," + avgVolumeDiff.ToString ();
		data += "," + correctAnswer.ToString();
		data += "," + volumeDiffFromCorrectAnswer.ToString();
		data += "," + currentLevel.ToString();
		data += "," + maxLayerCount.ToString ();
		data += "," + minLayerCount.ToString ();
		data += "," +totalDifferentTilesFromCorrectAnswer.ToString();
		data += "," + differenceInSquares.ToString();
		data += "," + differenceInIsos.ToString();
		data += "," + differenceInRounds.ToString();
		GameData.dataControl.SavePerformanceStats (data);

	}

	public void DetermineMaxMinLayerCounts( List<int> layerCounts )
	{
		if (layerCounts.Count == 0) 
		{
			maxLayerCount = 1;
			minLayerCount = 1;
		}
		else
		{
			int mostLayers = 0;
			int fewestLayers = 10;
			for ( int layer = 0; layer < layerCounts.Count; layer ++ )
			{
				if( layerCounts[ layer ] > mostLayers )
				{
					mostLayers = layerCounts[ layer ];
				}
				else if ( layerCounts[ layer ] < fewestLayers )
				{
					fewestLayers = layerCounts[ layer ];
				}
			}

			maxLayerCount = mostLayers;
			minLayerCount = fewestLayers;
		}
	}

	public void ResetStats()
	{

	}
}
