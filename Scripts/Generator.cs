using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
	
	public Tile[,] Tiles;

	public MeshRenderer HeightMapRenderer;
	public MeshRenderer HeatMapRenderer;
	public MeshRenderer MoistureMapRenderer;
	public MeshRenderer BiomeMapRenderer;

	public MapSizes mapSize = MapSizes.ExtraLarge_512;
	private int size;

	[Header("Heightmap data")]
	[Range(0, 20)]
	public int depth = 14;
	[Range(0, 10)]
	public int octaves = 6;

	public void CreateObjects(float[,] map)
	{
		TerrainData terrainData = Terrain.activeTerrain.terrainData;

		var temp = gameObject.transform.GetComponent<ObjectPlacerController>();
		temp.temp(size, terrainData,Tiles, map,seed);
	}

	[Range(0, 10)]
	public float frequency = 1.25f;
	[Range(0, 500)]
	public float scale = 250f;
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
	[Range(0, 1000)]
	public float heatScale = 600f;
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
	[Range(0, 1000)]
	public float moistureScale = 800f;
	[Range(-2, 2)]
	public float moistureAmplitudeMod = 0.25f;
	[Range(0, 5)]
	public float moistureFrequencyMod = 2f;

	[Space(10f)]
	[Header("Detail data")]
	[Range(0, 10)]
	public int detailOctaves = 3;
	[Range(0, 10)]
	public float detailFrequency = 2f;
	[Range(0, 1000)]
	public float detailScale = 50;
	[Range(-2, 2)]
	public float detailAmplitudeMod = 0.5f;
	[Range(0, 5)]
	public float detailFrequencyMod = 2f;


	[Header("Seed")]
	public long seed;
	private long seedOffset = 0;


	//detail infomation
	public Dictionary<GrassTypes, int> detailToIndex = new Dictionary<GrassTypes, int>();



	public long getSeedOffset()
	{
		seedOffset += 1;
		return seedOffset;
	}


	#region Height Classifications
	float DeepWater = 0.2f;
	float ShallowWater = 0.3f;
	float Sand = 0.4f;
	float Grass = 0.45f;
	float Forest = 0.7f;
	float Rock = 0.85f;

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

	//public static Dictionary<BiomeType, int> biomeToIndex = new Dictionary<BiomeType, int>();
	public static Dictionary<BiomeType, Biome> biomeDict = new Dictionary<BiomeType, Biome>();

	public void Start()
	{
		seed = (long)UnityEngine.Random.Range(0, 9999999f);
		Main();
	}

	public void LoadDefaultValues()
	{
		HeatValues heat = HeatValues.InputFromXML("DEFAULT_HEAT");
		BiomeInfomation.GenerateHeatDictionary(heat);


		MoistureValues moisture = MoistureValues.InputFromXML("DEFAULT_MOISTURE");
		BiomeInfomation.GenerateMoistureDictionary(moisture);

		//biomeToIndex = new Dictionary<BiomeType, int>();
		biomeDict = new Dictionary<BiomeType, Biome>();
		foreach(BiomeType b in Enum.GetValues(typeof(BiomeType)))
		{
			biomeDict[b] = new Biome(b);
			biomeDict[b].CreateHeightMap(size, seed);
		}

		detailToIndex = new Dictionary<GrassTypes, int>();

	}
	public void Run_RandomSeed()
	{
		seed = (long)UnityEngine.Random.Range(0, 9999999f);
		seedOffset = 0;
		Main();
	}
	public void Run_SetSeed()
	{
		seedOffset = 0;
		Main();
	}


	public void Main()
	{
		size = GetMapSize(mapSize);

		//Set variables and instatiate -- uses mapsize and seed so must be after.
		LoadDefaultValues();



		// Get the mesh we are rendering our output to
		FindSceneTextures();

		float[,] heightMap = GetData(size, scale, octaves, amplitudeMod, frequencyMod, seed);
		float[,] heatMap = GetData(size, heatScale, heatOctaves, heatAmplitudeMod, heatFrequencyMod, seed + getSeedOffset());
		float[,] moistureMap = GetData(size, moistureScale, moistureOctaves, moistureAmplitudeMod, moistureFrequencyMod, seed + getSeedOffset());


		// Build our final objects based on our data
		LoadTiles(heightMap,heatMap, moistureMap);

		GenerateBiomeMap();

		float[,] finalH = AddBiomesToHeight(size, heightMap, Tiles);

		finalH = GenerateSmoothNoise(size, finalH, 2);
		finalH = GenerateSmoothNoise(size, finalH, 1);

		TerrainData terrainData = Terrain.activeTerrain.terrainData;

		terrainData.heightmapResolution = size+1;

		terrainData.heightmapResolution = size + 1;
		terrainData.alphamapResolution = size;
		terrainData.SetDetailResolution(size, 8);
		terrainData.size = new Vector3(size, depth, size);
		terrainData.SetHeights(0,0, finalH);

		GenerateLayers(terrainData);
		PaintMap(size,Tiles, terrainData);


		// Render a texture representation of our map
		HeightMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(size, size, Tiles, TextureGenerator.TextureTypes.HeightMap);
		HeatMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(size, size, Tiles, TextureGenerator.TextureTypes.HeatMap);
		MoistureMapRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(size, size, Tiles, TextureGenerator.TextureTypes.MoistureMap);
		BiomeMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(size, size, Tiles);

		float[,] detailMap = GetData(size, detailScale, detailOctaves, detailAmplitudeMod, detailFrequencyMod, seed + getSeedOffset());


		GenerateDetailLayers(terrainData);
		DetailMap(terrainData, Tiles,detailMap);

		//CreateObjects(finalH);

	}

	public void GenerateDetailLayers(TerrainData terrainData)
	{
		detailToIndex = new Dictionary<GrassTypes, int>();

		string path = "GrassTexture/";
		List<DetailPrototype> temp = new List<DetailPrototype>();
		int indexCount = 0;
		foreach (GrassTypes t in Enum.GetValues(typeof(GrassTypes)))
		{
			Texture2D tex = (Texture2D)Resources.Load(path + t.ToString(), typeof(Texture2D));

			GrassSettings settings = GrassSettings.get_type(t);

			DetailPrototype detailLayer = new DetailPrototype();
			detailLayer.prototypeTexture = tex;
			detailLayer.minWidth = settings.DETAIL_MIN_WIDTH;
			detailLayer.maxWidth = settings.DETAIL_MAX_WIDTH;
			detailLayer.minHeight = settings.DETAIL_MIN_HEIGHT;
			detailLayer.maxHeight = settings.DETAIL_MAX_HEIGHT;
			detailLayer.noiseSpread = settings.DETAIL_NOISE_SPREAD;
			detailLayer.healthyColor = settings.DETAIL_HEALTHY_COLOR;
			detailLayer.dryColor = settings.DETAIL_DRY_COLOR;
			detailLayer.renderMode = settings.DETAIl_RENDER_MODE;

			detailToIndex[t] = indexCount;
			temp.Add(detailLayer);
			indexCount += 1;
		}
		terrainData.detailPrototypes = temp.ToArray();
	}



	public void DetailMap(TerrainData terrainData, Tile[,] tiles, float[,] detailMap)
	{

		int numDetails = terrainData.detailPrototypes.Length;
		int[,,] detailMapData = new int[terrainData.detailWidth, terrainData.detailHeight, numDetails];


		for (int i = 0; i < numDetails; i++)
		{
			//int[,] map = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i);
			int[,] map = new int[terrainData.detailWidth, terrainData.detailHeight];
			// For each pixel in the detail map...
			for (int y = 0; y < terrainData.detailHeight; y++)
			{
				for (int x = 0; x < terrainData.detailWidth; x++)
				{
					Tile tile = tiles[x, y];
					BiomeType close = tile.primaryBiomeType;
					List<grassInfomation> grassinfo = biomeDict[close].grassList;

					


					if (grassinfo.Count == 0) { continue; }
					for (int g = 0; g < grassinfo.Count;g++)
					{
						if (detailToIndex[grassinfo[g].grassEnum] != i) { continue; }

						if (grassinfo[g].grassWeight >= detailMap[x,y])
						{
							map[x, y] = 1;
						}
					}
				}
			}
			terrainData.SetDetailLayer(0, 0, i, map);
		}













					//[x,y,detailLayer]
					int[,,] map2 = new int[terrainData.detailWidth, terrainData.detailHeight, numDetails];

		for (int y = 0; y < terrainData.detailHeight; y++)
		{
			for (int x = 0; x < terrainData.detailWidth; x++)
			{
				Tile tile = Tiles[x, y];
				BiomeType close = tile.primaryBiomeType;
				Biome data = biomeDict[close];
				List<grassInfomation> grassinfo = data.grassList;
				if (grassinfo.Count == 0) { continue; }

				for (int g = 0;g < grassinfo.Count;g++)
				{
					var w = grassinfo[g].grassWeight;
					if (w==1)
					{
						map2[x, y, g] = 1;
					}
				}

			}
		}











			}

	/// <summary>
	/// https://gamedev.stackexchange.com/questions/54300/smoothing-heightmap-data
	/// </summary>
	/// <param name="size"></param>
	/// <param name="baseNoise"></param>
	/// <param name="octave"></param>
	/// <returns></returns>
	public static float[,] GenerateSmoothNoise(int size,float[,] baseNoise, int octave)
	{

		float[,] smoothNoise = new float[size, size];

		int samplePeriod = 1 << octave; // calculates 2 ^ k
		float sampleFrequency = 1.0f / samplePeriod;

		for (int i = 0; i < size; i++)
		{
			//calculate the horizontal sampling indices
			int sample_i0 = (i / samplePeriod) * samplePeriod;
			int sample_i1 = (sample_i0 + samplePeriod) % size; //wrap around
			float horizontal_blend = (i - sample_i0) * sampleFrequency;

			for (int j = 0; j < size; j++)
			{
				//calculate the vertical sampling indices
				int sample_j0 = (j / samplePeriod) * samplePeriod;
				int sample_j1 = (sample_j0 + samplePeriod) % size; //wrap around
				float vertical_blend = (j - sample_j0) * sampleFrequency;

				//blend the top two corners
				float top = Mathf.Lerp(baseNoise[sample_i0,sample_j0],
					baseNoise[sample_i1,sample_j0], horizontal_blend);

				//blend the bottom two corners
				float bottom = Mathf.Lerp(baseNoise[sample_i0,sample_j1],
					baseNoise[sample_i1,sample_j1], horizontal_blend);

				//final blend
				smoothNoise[i,j] = Mathf.Lerp(top, bottom, vertical_blend);
			}
		}
		return smoothNoise;
	}



	private float[,] AddBiomesToHeight(int mSize,float[,] hMap, Tile[,] tiles)
	{
		float BASE_TERRAIN_RATIO = 0.5f;
		float BIOME_TERRAIN_RATIO = 1 - BASE_TERRAIN_RATIO;

		//Generate heightmaps for the biomes.
		//For every point determine the amount needed to add.
		//add it as a percentage like the blending.
		//have some ratio values
		//apply smoothing.
		for (int x=0;x<mSize;x++)
		{
			for (int y = 0; y < mSize; y++)
			{
				//
				float baseHeight = hMap[x, y];
				BiomeType primaryBiome = tiles[x, y].primaryBiomeType;
				BiomeType secondaryBiome = tiles[x, y].SecondaryBiomeType;
				BiomeType thirdBiome = tiles[x, y].ThirdBiomeType;

				var d1 = tiles[x, y].primaryBiomeDistance;
				var d2 = tiles[x, y].secondaryBiomeDistance;
				var d3 = tiles[x, y].thirdBiomeDistance;


				var h1 = biomeDict[primaryBiome].heightMap[x, y];
				var h2 = biomeDict[secondaryBiome].heightMap[x, y];
				var h3 = biomeDict[thirdBiome].heightMap[x, y];

				float biomeHeight = (1 - d1) * h1 + (1 - d2) * h2 + (1 - d3) * h3;
				biomeHeight = biomeHeight * 0.5f;

				hMap[x, y] = BASE_TERRAIN_RATIO * baseHeight + BIOME_TERRAIN_RATIO * biomeHeight;
			}
		}
		return hMap;
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
				var index1 = biomeDict[biomeMap[x, y].primaryBiomeType].texInfo.index;
				var index2 = biomeDict[biomeMap[x, y].SecondaryBiomeType].texInfo.index;
				var index3 = biomeDict[biomeMap[x, y].ThirdBiomeType].texInfo.index;


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
		string path = "TerrainLayers/";
		List<TerrainLayer> temp = new List<TerrainLayer>();
		int indexCount = 0;
		foreach (BiomeType t in Enum.GetValues(typeof(BiomeType)))
		{
			TerrainLayer tempLayer = (TerrainLayer)Resources.Load(path + t.ToString(), typeof(TerrainLayer));
			temp.Add(tempLayer);

			biomeDict[t].texInfo.index = indexCount;
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
					//float xCord = (float)x / mapSize * scale * frequency;
					//float yCord = (float)y / mapSize * scale * frequency;
					
					float xCord = (float)x / scale * frequency;
					float yCord = (float)y / scale * frequency;


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
