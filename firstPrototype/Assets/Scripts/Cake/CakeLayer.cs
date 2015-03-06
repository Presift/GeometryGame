using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakeLayer : MonoBehaviour {
	
	float scaleChange;

	float tileSize;

	GameObject defaultTile;

	public List<GameObject> tilesUsed;

	public float area;

	public Selection selectionManager;

	public List<GameObject> squareTiles;
	public List<GameObject> isoTiles;
	public List<GameObject> roundTiles;

	public Color frostingColor;

	float frostingHeight;
	float frostingHeightMultiplier;
	public Material cakeColor;

	private float originalTileHeight;

	// Use this for initialization
	void Awake () {

		roundTiles = new List<GameObject> ();
		isoTiles = new List<GameObject> ();
		squareTiles = new List<GameObject> ();

		Transform parent = gameObject.transform;

		//if object already has children
		if (transform.childCount > 0) 
		{
			foreach (Transform child in parent) 
			{
				UpdateTileTypeLists( child.gameObject );
			}
		}

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void UpdateTileTypeLists( GameObject tile )
	{
		if (tile.name.Contains( "round" )) 
		{
			roundTiles.Add ( tile );
		}
		else if( tile.name.Contains( "cube"))
		{

			//get iso child and add to iso list
			Transform parent = tile.transform;
			
			foreach (Transform child in parent) 
			{
					isoTiles.Add (child.gameObject);
				
			}

		}
		else if( tile.name.Contains("iso"))
		{
			isoTiles.Add ( tile );
		}
	}

	public float SetUpPieceAndStats( int[,] pieceCoordinates, float newTileSize, float newScaleChange, GameObject newDefaultTile, 
	                                Vector3 centerPosition, List<GameObject> availableTiles, CakeTier tierScript, Material cakeMaterial, 
	                                int maxRoundTiles, float tileHeight, float frostingMultiplier)

	{
		squareTiles = new List<GameObject> ();
		roundTiles = new List<GameObject> ();
		isoTiles = new List<GameObject> ();

		originalTileHeight = tileHeight;
		frostingHeightMultiplier = frostingMultiplier;
		frostingHeight = originalTileHeight * frostingMultiplier;
		cakeColor = cakeMaterial;
		scaleChange = newScaleChange;
		tileSize = newTileSize;
		defaultTile = newDefaultTile;

		CreateTetrisPiece( pieceCoordinates, centerPosition, availableTiles, tierScript, maxRoundTiles );
		area = CalculateArea();

		return area;
	}

	public void WipeCurrentLayer()
	{
		Transform parent = gameObject.transform;
		
		foreach (Transform child in parent) 
		{
			Destroy(child.gameObject);
		}
		
		gameObject.transform.DetachChildren ();
		roundTiles = new List<GameObject> ();
		isoTiles = new List<GameObject> ();
		squareTiles = new List<GameObject> ();
	}


	void CreateTetrisPiece( int[,] coordinates, Vector3 centerPosition, List<GameObject> availableTiles, CakeTier tierScript, int maxRoundTiles )
	{

		for (int coordinateIndex = 0; coordinateIndex < ( coordinates.Length / 2 ); coordinateIndex++) 
		
		{
			List < GameObject > refreshedTiles = new List < GameObject > ( availableTiles );

			//if max round tiles used
			if( roundTiles.Count >= maxRoundTiles )
			{
				for( int tileIndex = 0; tileIndex < availableTiles.Count; tileIndex++ )
				{
					GameObject tile = availableTiles[ tileIndex ];
//					Debug.Log ( tile.name );
					if( tile.name == "round" )
					{
						availableTiles.Remove( tile );
						Debug.Log (" round removed from available tiles ");
					}
				}
			}

			int column = coordinates [ coordinateIndex, 0 ];
			int row = coordinates [ coordinateIndex, 1 ];

			List<bool> sideExposures = getTilePieceSideExposure( coordinates, coordinateIndex );

			GameObject newTile = makeNewFittedTile( refreshedTiles, sideExposures, column, row, centerPosition );
			//instantiate tile at scaled size at coordinate

			UpdateTileTypeLists (newTile);

			newTile.transform.localScale *= scaleChange;
			//set side Exposure bools on this tile
			CakeTile tileScript = (CakeTile)newTile.GetComponent(typeof(CakeTile));
			tileScript.SetGrandparentScript( tierScript );
			tileScript.sideExposures = sideExposures;

			//attach tile to parent piece
			newTile.transform.parent = this.gameObject.transform;
		}
	}

	void CreateFrosting( GameObject tile, Vector3 startPosition )  //add a layer of frosting to tile
	{
		Vector3 frostingPosition;

		if (tile.name.Contains ("cube")) 
		{
			//for first two children of tile
			for( int childIndex = 0; childIndex < 2; childIndex ++ )
			{
				//create frosting layer of child
				GameObject child = tile.transform.GetChild( childIndex ).gameObject;

				frostingPosition = startPosition + new Vector3( 0, 0, ( originalTileHeight + frostingHeight ) / 2  );
				GameObject frostingLayer = ( GameObject )Instantiate( child, frostingPosition, child.transform.rotation );

				//parent frosting to cake layer
				frostingLayer.transform.parent = child.transform;

				frostingLayer.transform.localScale = new Vector3( frostingLayer.transform.localScale.x, frostingLayer.transform.localScale.y, frostingLayer.transform.localScale.z * frostingHeightMultiplier );
				//change color to frosting color
				frostingLayer.renderer.material.color = frostingColor;
				frostingLayer.name = "Frosting";
				
				//remove script and collider
//				CakeTile tileScript = (CakeTile)frostingLayer.GetComponent(typeof(CakeTile));
//				BoxCollider collider = (BoxCollider) frostingLayer.GetComponent(typeof( BoxCollider ));
//				Destroy( collider );
//				Destroy(tileScript);
			}
				
		}
		else
		{
			frostingPosition = startPosition + new Vector3( 0, 0, ( originalTileHeight + frostingHeight ) / 2  );
			GameObject frostingLayer = ( GameObject )Instantiate( tile, frostingPosition, tile.transform.rotation );
			
			//parent frosting to cake layer
			frostingLayer.transform.parent = tile.transform;
			frostingLayer.transform.localScale = new Vector3( frostingLayer.transform.localScale.x, frostingLayer.transform.localScale.y, frostingLayer.transform.localScale.z * frostingHeightMultiplier );
			//change color to frosting color
			frostingLayer.renderer.material.color = frostingColor;
			frostingLayer.name = "Frosting";
			
			//remove script and collider
			CakeTile tileScript = (CakeTile)frostingLayer.GetComponent(typeof(CakeTile));
			BoxCollider collider = (BoxCollider) frostingLayer.GetComponent(typeof( BoxCollider ));
			Destroy( collider );
			Destroy(tileScript);
		}

	}
	

	float CalculateArea()
	{
		float area = 0;
		Transform parent = gameObject.transform;
		
		//for each child 
		foreach (Transform child in parent) 
		{
			//add child's area to total area
			CakeTile tileScript = (CakeTile)child.GetComponent(typeof(CakeTile));
			area += tileScript.area;

		}
		return area;

	}
	

	int getTileAndRotation( CakeTile tileScript, List<bool> sideExposures )
	{
		for (int rotation = 0; rotation < 4; rotation ++) 
		{
			List<float> sidePerimeters = tileScript.GetNewPerimetersAfterRotation ( 90 * rotation );
			if( tileFitsInCoordinates( sidePerimeters, sideExposures ))
			{
				//return side perimeters
				return ( 90 * rotation );
			}
		}

		return -1;
	}

	GameObject makeNewFittedTile( List<GameObject> availableTiles, List<bool> sideExposures, int column, int row, Vector3 centerPosition )
	{
		GameObject fittedTile = null;

		while ( fittedTile == null)
		{

			//randomly choose a tile from available tile and remove it from list of available tiles
			int random = Random.Range( 0, availableTiles.Count );

			GameObject possibleTile = availableTiles[ random ];
//			Debug.Log (possibleTile.name);

			availableTiles.RemoveAt( random );

			//get tile script
			CakeTile tileScript = ( CakeTile ) possibleTile.GetComponent(typeof(CakeTile));

			//get successful rotation for possible tile
			int rotation = getTileAndRotation( tileScript, sideExposures);

			if(rotation >= 0 )
			{
				return CreateNewTile( possibleTile, column, row, rotation, fittedTile, centerPosition );
			}

		}

		return CreateNewTile( defaultTile, column, row, 0, fittedTile, centerPosition );
	}

	GameObject CreateNewTile( GameObject newTile, int column, int row, int rotation, GameObject fittedTile, Vector3 centerPosition )
	{


		//instantiate new tile at rotation
		Vector3 startPosition = new Vector3 (column * tileSize + centerPosition.x, row * tileSize + centerPosition.y, centerPosition.z);
		fittedTile = (GameObject)Instantiate( newTile, startPosition, Quaternion.identity);
		CakeTile fittedTileScript = ( CakeTile ) fittedTile.GetComponent(typeof(CakeTile));
		fittedTileScript.SetNewPerimetersAfterRotation( rotation );

		if (fittedTile.name.Contains ("cube")) 
		{
			
			foreach (Transform child in fittedTile.transform ) 
			{
				child.gameObject.renderer.material = cakeColor;
				
			}
		}
		else
		{
			fittedTile.renderer.material = cakeColor;
		}


		CreateFrosting (fittedTile, startPosition );

		return fittedTile;
	}



	bool tileFitsInCoordinates ( List<float> sidePerimeters, List<bool> sideExposures ) //side exposures is a list of left, top, right, bottom sides of a tile
	{
		for (int side = 0; side < 4; side++) 
		{
			if(!sideExposures[ side ])
			{
				//check to see if corresponding side on tileScript is a full side
				if(sidePerimeters [ side ] != 1 )
				{
					return false;
				}
			}

		}
		return true;
	}
	

	List<int> GetNeighborCoordinateOffsets( int sideIndex )
	{

		int xOffset;
		int yOffset;
		List<int> offsets = new List<int> ();

		switch( sideIndex )
		{
		case 0: //left side
			xOffset = -1;
			yOffset = 0;
			break;
		case 1: //top side
			xOffset = 0;
			yOffset = 1;
			break;
		case 2: //right side
			xOffset = 1;
			yOffset = 0;
			break;
		case 3: //bottom side
			xOffset = 0;
			yOffset = -1;
			break;
		default: 	
			Debug.Log ("not a valid sideIndex");
			xOffset = 0;
			yOffset = 0;
			break;
		}

		offsets.Add (xOffset);
		offsets.Add (yOffset);
		return offsets;
	}

	List<bool> getTilePieceSideExposure ( int[,] coordinates, int coordinateIndex )
	{
		int currentSideX = coordinates [coordinateIndex, 0 ];
		int currentSideY = coordinates [coordinateIndex, 1 ];
		List<bool> sideExposures = new List<bool>();


		//iterate 4 times to get left, top, top and right side
		for( int sideIndex = 0; sideIndex < 4; sideIndex ++ )
		{
			bool exposed = true;
			List<int> coordinateOffsets = GetNeighborCoordinateOffsets( sideIndex );
			int xOffset = coordinateOffsets[ 0 ];
			int yOffset = coordinateOffsets[ 1 ];

			for( int coordinate = 0; coordinate < ( coordinates.Length / 2 ); coordinate ++ )
			{
				//if there is another tile found in the coordinate list with the above offset
				if( (( currentSideX + xOffset ) == coordinates[ coordinate, 0 ]) && (( currentSideY + yOffset ) == coordinates[ coordinate, 1 ]))
				{
					exposed = false;

				}
			}

			sideExposures.Add ( exposed );

		}

		return sideExposures;
	}

//	public void ChangeChildrenColor()
//	{
//		Transform parent = gameObject.transform;
//
//		foreach (Transform child in parent) 
//		{
//			//set child color
//			child.renderer.material.color = frostingColor;
//		}
//	}

}

