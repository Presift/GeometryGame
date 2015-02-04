using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public float leftPerimeter;
	public float rightPerimeter;
	public float topPerimeter;
	public float bottomPerimeter;

	public List<bool> sideExposures = new List<bool> ();
	public List<float> sidePerimeters = new List<float>();

	public float area;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown()
	{
		//get parent script Tetris Piece
		Transform parent = gameObject.transform.parent;

		TetrisPiece pieceScript = (TetrisPiece)parent.GetComponent (typeof(TetrisPiece));
		pieceScript.Selected ();
	}
	

	public List<float> GetNewPerimetersAfterRotation( int rotation)
	{
		int numberOf90DegreeRotations = rotation / 90;

		float newLeftPerimeter = leftPerimeter;
		float newTopPerimeter = topPerimeter;
		float newRightPerimeter = rightPerimeter;
		float newBottomPerimeter = bottomPerimeter;


		for (int rotationCount = 0; rotationCount < numberOf90DegreeRotations; rotationCount++) 
		{
			float left = newLeftPerimeter;
			float bottom = newBottomPerimeter;
			float right = newRightPerimeter;

			newLeftPerimeter = newTopPerimeter;
			newBottomPerimeter = left;
			newRightPerimeter = bottom;
			newTopPerimeter = right;

		}

		return new List< float > ( new float[] { newLeftPerimeter, newTopPerimeter, newRightPerimeter, newBottomPerimeter } );
	}

	public void SetNewPerimetersAfterRotation( int rotation )
	{
		float newRotation = (float)transform.eulerAngles.z + rotation;

		transform.eulerAngles = new Vector3( transform.eulerAngles.x, transform.eulerAngles.y, newRotation);

		List<float> newPerimeters = GetNewPerimetersAfterRotation (rotation);
		leftPerimeter = newPerimeters [ 0 ];
		topPerimeter = newPerimeters [ 1 ];
		rightPerimeter = newPerimeters [ 2 ];
		bottomPerimeter = newPerimeters [ 3 ];

		sidePerimeters = new List<float>();
		sidePerimeters.Add (leftPerimeter);
		sidePerimeters.Add (topPerimeter);
		sidePerimeters.Add (rightPerimeter);
		sidePerimeters.Add (bottomPerimeter);
	}


}
