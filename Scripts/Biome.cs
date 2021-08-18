using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
	public static string XML_PREFIX = "DEFAULT_BIOME_";

	public BiomeType bType;
	public GeneratorSettings generateSettings;

	public TextureInfomation texInfo;
	public float[,] heightMap;

	//Grass values
	//public List<grassInfomation> grassList = new List<grassInfomation>();
	public Dictionary<int, grassInfomation> grassDict = new Dictionary<int, grassInfomation>();

	public float totalW = 0;

	public Biome(BiomeType b)
	{
		bType = b;
		updateGrassInfo(b);
		generateSettings = GeneratorSettings.InputFromXML(XML_PREFIX + b.ToString());

		float total = 0;
		foreach(var v in grassList)
		{
			if (v.grassWeight == 1) { continue; }
			total += v.grassWeight;
		}
		for(int i =0;i<grassList.Count;i++)
		{
			if (grassList[i].grassWeight == 1) { continue; }
			var temp = grassList[i];
			temp.grassWeight = grassList[i].grassWeight / total;
			grassList[i] = temp;
		}
	}

	public void CreateHeightMap(int size,long seed)
	{
		heightMap = Generator.GetData(size, 
			generateSettings.scale,
			generateSettings.octaves,
			generateSettings.amplitudeMod,
			generateSettings.frequencyMod, 
			seed);
	}

	public void updateGrassInfo(BiomeType b)
	{
		switch(b)
		{
			case BiomeType.Desert:
				details_Desert();
				break;
			case BiomeType.Savanna:
				details_Savanna();
				break;
			case BiomeType.TropicalRainforest:
				details_TropicalRainforest();
				break;
			case BiomeType.Grassland:
				details_Grassland();
				break;
			case BiomeType.Woodland:
				details_Woodland();
				break;
			case BiomeType.SeasonalForest:
				details_SeasonalForest();
				break;
			case BiomeType.TemperateRainforest:
				details_TemperateRainforest();
				break;
			case BiomeType.BorealForest:
				details_BorealForest();
				break;
			case BiomeType.Tundra:
				details_Tundra();
				break;
			case BiomeType.Ice:
				details_Ice();
				break;
			default:
				Debug.Log("Did not find biome: "+b.ToString());
				break;
		}
	}


	public void details_Desert()
	{
		//grassList.Add(new grassInfomation(GrassTypes.DesertGrass, 0.2f));
		//grassList.Add(new grassInfomation(GrassTypes.DeadGrass, 0.2f));
	}
	public void details_Savanna()
	{
		//grassList.Add(new grassInfomation(GrassTypes.DesertGrass, 0.25f));
		//grassList.Add(new grassInfomation(GrassTypes.DeadGrass, 0.75f));
		//grassList.Add(new grassInfomation(GrassTypes.Straw, 0.25f));
	}
	public void details_TropicalRainforest()
	{
		grassDict[]


		grassList.Add(new grassInfomation(GrassTypes.RainforestGrass, 1));
		grassList.Add(new grassInfomation(GrassTypes.WoodlandGrass, 0.5f));
	}
	public void details_Grassland()
	{
		grassList.Add(new grassInfomation(GrassTypes.GrasslandGrass, 1));
		grassList.Add(new grassInfomation(GrassTypes.WoodlandGrass, 0.25f));
		grassList.Add(new grassInfomation(GrassTypes.Flower, 0.5f));
	}
	public void details_Woodland()
	{
		grassList.Add(new grassInfomation(GrassTypes.GrasslandGrass, 0.5f));
		grassList.Add(new grassInfomation(GrassTypes.WoodlandGrass, 1f));
		grassList.Add(new grassInfomation(GrassTypes.Flower, 0.5f));
	}
	public void details_SeasonalForest()
	{
		grassList.Add(new grassInfomation(GrassTypes.GrasslandGrass, 0.5f));
		grassList.Add(new grassInfomation(GrassTypes.WoodlandGrass, 0.5f));
		grassList.Add(new grassInfomation(GrassTypes.RainforestGrass, 0.25f));
		grassList.Add(new grassInfomation(GrassTypes.DeadGrass, 0.25f));
	}
	public void details_TemperateRainforest()
	{
		grassList.Add(new grassInfomation(GrassTypes.RainforestGrass, 1));
		grassList.Add(new grassInfomation(GrassTypes.WoodlandGrass, 0.5f));
		grassList.Add(new grassInfomation(GrassTypes.GrasslandGrass, 0.5f));
	}
	public void details_BorealForest()
	{
		grassList.Add(new grassInfomation(GrassTypes.GrasslandGrass, 0.25f));
		grassList.Add(new grassInfomation(GrassTypes.WoodlandGrass, 0.25f));
		grassList.Add(new grassInfomation(GrassTypes.SnowGrass, 0.5f));
	}
	public void details_Tundra()
	{
		grassList.Add(new grassInfomation(GrassTypes.SnowGrass, 1f));
		grassList.Add(new grassInfomation(GrassTypes.Flower, 0.25f));
	}
	public void details_Ice()
	{
		//ice shouldnt have grass
	}
}

public struct TextureInfomation
{
	public Texture2D tex;
	public int index;

}

public struct grassInfomation
{
	public GrassTypes grassEnum;
	public float grassWeight;

	public grassInfomation(GrassTypes g, float w)
	{
		grassEnum = g;
		grassWeight = w;
	}
}
