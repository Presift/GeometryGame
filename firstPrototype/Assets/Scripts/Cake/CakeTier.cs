using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakeTier : MonoBehaviour {
	
	public GameModel manager;
	public Selection selectionManager;
	public float volume;
	public Vector3 centerPosition;
	public bool inputLocked;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public float NewCakeTier ( int numberOfLayers, float newTileSize, float newTileHeight, float scaleChange, GameObject defaultTile, Vector3 position, List<string> tetrisShapes, CakeTier tierScript )
	{
		centerPosition = position;
		List<GameObject> availableTiles = manager.GetAvailableTiles ();
		//instantiate a cake layer
		GameObject newCakeLayer = (GameObject)Resources.Load("Cake Layer");
		GameObject newCakeLayerObject = (GameObject)Instantiate( newCakeLayer, position, Quaternion.identity);

		//add it as child of cake tier
		newCakeLayerObject.transform.parent = this.transform;
		
		//create cake tile pieces on layer
		CakeLayer layerScript = ( CakeLayer )newCakeLayerObject.GetComponent( typeof( CakeLayer ));
		float area = layerScript.SetUpPieceAndStats( 0, newTileSize, scaleChange, defaultTile, position, tetrisShapes, availableTiles, tierScript );

		float frostingHeight = newTileHeight/4;

		//add a layer of frosting
		position += new Vector3( 0, 0, ( newTileHeight + frostingHeight ) / 2  );
		GameObject frostingLayer = ( GameObject )Instantiate( newCakeLayerObject, position, Quaternion.identity );
		//parent frosting to cake layer
		frostingLayer.transform.parent = layerScript.transform;
		//change color to frosting color
		CakeLayer frostingScript = ( CakeLayer )frostingLayer.GetComponent( typeof( CakeLayer ));
		frostingScript.ChangeChildrenColor ();
//		frostingLayer.transform.scale.z = frostingHeight;
		frostingLayer.transform.localScale = new Vector3( frostingLayer.transform.localScale.x, frostingLayer.transform.localScale.y, frostingLayer.transform.localScale.z / 4 );

		//for each additional layer over 1
		for (int layer = 1; layer < numberOfLayers; layer++) 
		{
			//add a cake layer
			position += new Vector3(0, 0, newTileHeight/ 2);
			//duplicate cakeObject
			GameObject additionalLayer = ( GameObject )Instantiate( newCakeLayerObject, position, Quaternion.identity );
			additionalLayer.transform.parent = this.transform;

			//add a layer of frosting
			position += new Vector3( 0, 0, ( newTileHeight + frostingHeight ) / 2  );
			GameObject additionalFrostingLayer = ( GameObject )Instantiate( frostingLayer, position, Quaternion.identity );
			//parent frosting to cake layer
			frostingLayer.transform.parent = additionalLayer.transform;

		}

		volume = area * numberOfLayers;

		return volume;
	}

	public void Selected()
	{
		selectionManager.IsSelectionCorrect ( this );
	}

	public void LockInputOnTier()
	{

	}
}
