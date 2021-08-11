using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
	
	Tile[,] Tiles;

	public MeshRenderer HeightMapRenderer;
	public MeshRenderer HeatMapRenderer;
	public MeshRenderer MoistureMapRenderer;
	public MeshRenderer BiomeMapRenderer;

	public MapSizes mapSize = MapSizes.ExtraLarge_512;
	private int size;

	[Header("Heightmap data")]
	[Range(0, 10)]
	public float depth = 14;
	public int octaves = 6;
	[Range(0, 10)]
	public float frequency = 1.25f;
	[Range(0, 30)]
	public float scale = 10.5f;
	[Range(-2, 2)]
	public float amplitudeMod = 0.5f;
	[Range(0, 5)]
	public float frequencyMod = 2f;
	[Space(10f)]
	[Header("Heatmap data")]
	[Range(0,10)]
	public int heatOctaves = 3;
	[Range(0, 10)]
	public float heatFrequency = 1.25f;
	[Range(0, 30)]
	public float heatScale = 3f;
	[Range(-2, 2)]
	public float heatAmplitudeMod = 0.25f;
	[Range(0, 5)]
	public float heatFrequencyMod = 2f;

	[Space(10f)]
	[Header("Moisturemap data")]
	[Range(0, 10)]
	public int moistureOctaves = 3;
	[Range(0, 10)]
	public float moistureFrequency = 1.25f;
	[Range(0, 30)]
	public float moistureScale = 3f;
	[Range(-2, 2)]
	public float moistureAmplitudeMod = 0.25f;
	[Range(0, 5)]
	public float moistureFrequencyMod = 2f;


	[Header("Seed")]
	public long seed;
	private long seedOffset = 0;


	public long getSeedOffset()
	{
		seedOffset += 1;
		return seedOffset;
	}


	#region Height Classifications
	float DeepWater = 0.2f;
	float ShallowWater = 0.4f;
	float Sand = 0.5f;
	float Grass = 0.7f;
	float Forest = 0.8f;
	float Rock = 0.9f;

	#endregion


	public static BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
};

	public static Dictionary<BiomeType, int> biomeToIndex = new Dictionary<BiomeType, int>();

	public void Start()
	{
		Main();
	}

	public void LoadDefaultValues()
	{
		HeatValues heat = HeatValues.InputFromXML("DEFAULT_HEAT");
		Biome.GenerateHeatDictionary(heat);


		MoistureValues moisture = MoistureValues.InputFromXML("DEFAULT_MOISTURE");
		Biome.GenerateMoistureDictionary(moisture);

		biomeToIndex = new Dictionary<BiomeType, int>();
	}

	public void Main()
	{
		//Lots Heat and Moisture settings from XML file. 
		//Sets vairables Biome.MoistureToValue and Biome.HeatToValue [Dictionaries]
		LoadDefaultValues();
		size = GetMapSize(mapSize);


		seed = (long)UnityEngine.Random.Range(0, 9999999f);

		// Get the mesh we are rendering our output to
		FindSceneTextures();

		float[,] heightMap = GetData(size, scale, octaves, amplitudeMod, frequencyMod, seed);
		float[,] heatMap = GetData(size, heatScale, heatOctaves, heatAmplitudeMod, heatFrequencyMod, seed + getSeedOffset());
		float[,] moistureMap = GetData(size, moistureScale, moistureOctaves, moistureAmplitudeMod, moistureFrequencyMod, seed + getSeedOffset());


		// Build our final objects based on our data
		LoadTiles(heightMap,heatMap, moistureMap);

		//float[,] biomeMapArray = NewGenerateBiomeMap(heatMap, moistureMap);

		//flood fill
		System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
		stopWatch.Start();

		GenerateBiomeMap();
		stopWatch.Stop();
		Debug.Log("Time Taken: "+stopWatch.ElapsedMilliseconds);


			// Render a texture representation of our map
		HeightMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(size, size, Tiles,TextureGenerator.TextureTypes.HeightMap);
		HeatMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(size, size, Tiles, TextureGenerator.TextureTypes.HeatMap);
		MoistureMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(size, size, Tiles, TextureGenerator.TextureTypes.MoistureMap);
		BiomeMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(size, size, Tiles);

		float[,] finalH = new float[size, size];
		for (int x=0;x<size;x++)
		{
			for (int y=0;y<size;y++)
			{
				finalH[x,y] = Tiles[x, y].Height;
			}
		}
		TerrainData terrainData = Terrain.activeTerrain.terrainData;

		terrainData.heightmapResolution = size+1;

		terrainData.heightmapResolution = size + 1;
		terrainData.alphamapResolution = size;
		terrainData.SetDetailResolution(size, 8);
		terrainData.size = new Vector3(size, depth, size);
		terrainData.SetHeights(0,0, finalH);

		GenerateLayers(terrainData);

		PaintMap(size,Tiles, terrainData);


	}
	private void FindSceneTextures()
	{
		HeightMapRenderer = transform.Find("HeightTexture").GetComponent<MeshRenderer>();
		HeatMapRenderer = transform.Find("HeatTexture").GetComponent<MeshRenderer>();
		MoistureMapRenderer = transform.Find("MoistureTexture").GetComponent<MeshRenderer>();
		BiomeMapRenderer = transform.Find("BiomeTexture").GetComponent<MeshRenderer>();
	}

	private int GetMapSize(MapSizes mapSize)
	{
		switch (mapSize)
		{
			case MapSizes.Debug_16:
				return 16;
			case MapSizes.Tiny_32:
				return 32;
			case MapSizes.Small_64:
				return 64;
			case MapSizes.Medium_128:
				return 128;
			case MapSizes.Large_256:
				return 256;
			case MapSizes.ExtraLarge_512:
				return 512;
			case MapSizes.RolePlay_1024:
				return 1024;
			default:
				return 128;
		}
	}

	private void PaintMap(int tempSize, Tile[,] biomeMap, TerrainData terrainData)
	{
		int counter = 15;

		float[,,] splatMap = new float[tempSize, tempSize, terrainData.alphamapLayers];
		for (int x = 0; x < tempSize; x++)
		{
			for (int y = 0; y < tempSize; y++)
			{
				var d1 = biomeMap[x, y].primaryBiomeDistance;
				var d2 = biomeMap[x, y].secondaryBiomeDistance;
				var d3 = biomeMap[x, y].thirdBiomeDistance;
				float totalDistance = d1 + d2 + d3;
				var index1 = biomeToIndex[biomeMap[x, y].primaryBiomeType];
				var index2 = biomeToIndex[biomeMap[x, y].SecondaryBiomeType];
				var index3 = biomeToIndex[biomeMap[x, y].ThirdBiomeType];



				splatMap[x, y, index1] = 0.5f * (1 - d1 / totalDistance);
				splatMap[x, y, index2] = 0.5f * (1 - d2 / totalDistance);
				splatMap[x, y, index3] = 0.5f * (1 - d3 / totalDistance);

				if (counter > 0)
				{
					//Debug.Log(biomeMap[x, y].primaryBiomeType);
					//Debug.Log(biomeMap[x, y].SecondaryBiomeType);
					//Debug.Log(biomeMap[x, y].ThirdBiomeType);
					//Debug.Log(splatMap[x, y, index1]);
					//Debug.Log(splatMap[x, y, index2]);
					//Debug.Log(splatMap[x, y, index3]);
				}

				counter -= 1;
			}
		}
		terrainData.SetAlphamaps(0, 0, splatMap);
	}

	private void GenerateLayers(TerrainData terrainData)
	{
		List<TerrainLayer> temp = new List<TerrainLayer>();
		int indexCount = 0;
		foreach (BiomeType t in Enum.GetValues(typeof(BiomeType)))
		{
			TerrainLayer tempLayer = new TerrainLayer();
			tempLayer.diffuseTexture = Resources.Load(t.ToString()) as Texture2D;
			tempLayer.name = t.ToString();
			tempLayer.tileSize = new Vector2(2, 2);
			tempLayer.metallic = 0;
			temp.Add(tempLayer);

			biomeToIndex.Add(t, indexCount);
			indexCount += 1;
		}
		terrainData.terrainLayers = temp.ToArray();
	}

	private void GenerateBiomeMap()
	{
		for (var x = 0; x < size; x++)
		{
			for (var y = 0; y < size; y++)
			{
				Tiles[x, y].CalculateClosestHeatTypes();
				Tiles[x, y].CalculateClosestMoistureTypes();
				Tiles[x, y].CalculateBiomeTypes(BiomeTable);
			}
		}
	}


	private float[,] NewGenerateBiomeMap(float[,] heatMap, float[,] moistureMap, int size)
	{
		float[,] tempArray = new float[size, size];
		for (var x = 0; x < size; x++)
		{
			for (var y = 0; y < size; y++)
			{

			}
		}

				throw new NotImplementedException();
	}














	// Build a Tile array from our data
	private void LoadTiles(float[,] heightMap, float[,] heatMap, float[,] moiustureMap)
	{
		Tiles = new Tile[size, size];
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				Tile t = new Tile();
				t.X = x;
				t.Y = y;
				t.Height = heightMap[x, y];
				t.HeatData = heatMap[x, y];
				t.MoistureData = moiustureMap[x, y];
				float heightValue = t.Height;

				if (heightValue < DeepWater)
				{
					t._HeightType = HeightType.DeepWater;
				}
				else if (heightValue < ShallowWater)
				{
					t._HeightType = HeightType.ShallowWater;
				}
				else if (heightValue < Sand)
				{
					t._HeightType = HeightType.Sand;
				}
				else if (heightValue < Grass)
				{
					t._HeightType = HeightType.Grass;
				}
				else if (heightValue < Forest)
				{
					t._HeightType = HeightType.Forest;
				}
				else if (heightValue < Rock)
				{
					t._HeightType = HeightType.Rock;
				}
				else
				{
					t._HeightType = HeightType.Snow;
				}
				Tiles[x, y] = t;
			}
		}
	}



	public static float[,] GetData(int mapSize, float scale, int octaves, float amplitudeMod, float frequencyMod, long seed)
	{
		float[,] heights = new float[mapSize, mapSize];

		float minNoiseHeight = float.MaxValue;
		float maxNoiseHeight = float.MinValue;

		OpenSimplexNoise simplex = new OpenSimplexNoise(seed);
		for (int x = 0; x < mapSize; x++)
		{
			for (int y = 0; y < mapSize; y++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;
				for (int i = 0; i < octaves; i++)
				{
					float xCord = (float)x / mapSize * scale * frequency;
					float yCord = (float)y / mapSize * scale * frequency;


					float perlinValue = (float)(simplex.Evaluate(xCord, yCord) + 1) / 2;
					noiseHeight += perlinValue * amplitude;

					amplitude *= amplitudeMod;
					frequency *= frequencyMod;
				}

				if (noiseHeight > maxNoiseHeight)
				{
					maxNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight)
				{
					minNoiseHeight = noiseHeight;
				}
				//Debug.Log(noiseHeight);
				heights[x, y] = noiseHeight;
			}
		}

		for (int x=0;x < mapSize;x++)
		{
			for (int y=0;y < mapSize;y++)
			{
				//normalize our value between 0 and 1
				heights[x,y] = (heights[x, y] - minNoiseHeight) / (maxNoiseHeight - minNoiseHeight);
			}
		}
		return heights;
	}





	/// <summary>
	/// Returns the value of X under the linear map:
	/// [domainMin,domainMax] -> [rangeMin,rangeMax]
	/// </summary>
	/// <param name="domainMin">Minimum input value</param>
	/// <param name="domainMax">Maximum input value</param>
	/// <param name="rangeMin">Minimum output value</param>
	/// <param name="rangeMax">Maximum output value</param>
	/// <param name="x">Value to be mapped</param>
	/// <returns>value x mapped from [domainMin,domainMax] -> [rangeMin,rangeMax]</returns>
	public static float GeneralLinearMap(float domainMin, float domainMax, float rangeMin, float rangeMax, float x)
	{
		float quotient = (rangeMax - rangeMin) / (domainMax - domainMin);
		float temp = rangeMin + (x - domainMin) * quotient;
		return temp;
	}


}
