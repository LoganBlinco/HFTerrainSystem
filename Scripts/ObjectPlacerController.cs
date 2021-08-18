using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacerController : MonoBehaviour
{
	public float radiusForObjects = 32;
	public Vector2 regionSize;
	public int rejectionSamples = 30;
	public float displayRadius = 8;

	List<Vector2> validPoints;

	public float defaultheight = 80;
	public float maxRayCastDistance = 100;
	public Transform parant;

	public Dictionary<BiomeType, List<weightedPrefab>> biomeToObject = new Dictionary<BiomeType, List<weightedPrefab>>();

	[SerializeField]
	public List<weightedPrefab> desertList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> savannaList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> tropicalList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> grasslandList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> woodlandList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> seasonalForestList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> temperateList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> borealList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> tundraList = new List<weightedPrefab>();
	[SerializeField]
	public List<weightedPrefab> iceList = new List<weightedPrefab>();

	public void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(gameObject.transform.position,new Vector3(regionSize.x,0,regionSize.y));
	}


	public void temp(int mapSize, TerrainData data, Tile[,] tiles, float[,] hMap, long seed)
	{
		biomeToObject = new Dictionary<BiomeType, List<weightedPrefab>>()
		{
			{BiomeType.Desert, desertList },
			{BiomeType.Savanna, savannaList },
			{BiomeType.TropicalRainforest, tropicalList },
			{BiomeType.Grassland, grasslandList },
			{BiomeType.Woodland, woodlandList },
			{BiomeType.SeasonalForest, temperateList },
			{BiomeType.TemperateRainforest, desertList },
			{BiomeType.BorealForest, borealList },
			{BiomeType.Tundra, tundraList },
			{BiomeType.Ice, iceList }
		};

		regionSize = new Vector2(mapSize, mapSize);

		validPoints = ObjectPlacer.GeneratePoints(radiusForObjects, regionSize, rejectionSamples);
		Debug.Log("valid points: " + validPoints.Count);
		PlaceObjects(validPoints,data, tiles, hMap, seed);
	}
	private void PlaceObjects(List<Vector2> points, TerrainData terrainData,Tile[,] Tiles, float[,] finalHMap, long seed)
	{
		OpenSimplexNoise generator = new OpenSimplexNoise(seed);
		foreach (Vector2 point in points)
		{
			int x = (int)point.x;
			int y = (int)point.y;
			var biome1 = Tiles[x, y].primaryBiomeType;
			Debug.Log(biome1);
			double random = generator.Evaluate(x, y);
			var objectList = biomeToObject[biome1];
			if (objectList.Count == 0) { continue; }

			var prefabObject = objectList[0].prefab;
			Vector3 pos = new Vector3(x, terrainData.size.y * finalHMap[x,y], y);
			Instantiate(prefabObject, pos, Quaternion.identity,parant);


		}
		//Instantiate(prefabForInstantiate, testPosition, Quaternion.identity, parant);
	}

	private void LoadObjects()
	{

	}
}

[System.Serializable]
public struct weightedPrefab
{
	[SerializeField]
	[Range(0, 1)]
	public float relativeWeight;
	[SerializeField]
	public GameObject prefab;
}

//List of biomes
//Each biome has a list of objects