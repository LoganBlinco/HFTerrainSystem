using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeightType
{
	DeepWater = 1,
	ShallowWater = 2,
	Shore = 3,
	Sand = 4,
	Grass = 5,
	Forest = 6,
	Rock = 7,
	Snow = 8,
}



public class Tile
{
	public static MoistureType DEFAULT_MOISTURE_TYPE = MoistureType.Dryest;
	public static HeatType DEFAULT_HEAT_TYPE = HeatType.Coldest;
	public static BiomeType DEFAULT_BIOME_TYPE = BiomeType.Ice;


	public float Height;
	public float HeatData;
	public float MoistureData;
	public int X;
	public int Y;

	public HeightType _HeightType;
	//public HeatType HeatType;
	//public MoistureType MoistureType;

	public BiomeType primaryBiomeType;
	public BiomeType SecondaryBiomeType;
	public BiomeType ThirdBiomeType;
	public float primaryBiomeDistance;
	public float secondaryBiomeDistance;
	public float thirdBiomeDistance;

	//NEW MOISTURE/HEAT SYSTEM
	public HeatType[] NewHeatTypes = new HeatType[3];
	public MoistureType[] NewMoistureTypes = new MoistureType[3];

	public void CalculateBiomeTypes(BiomeType[,] BiomeTable)
	{
		//distance function d(x,y)
		primaryBiomeType = getBiomeFromEnums(NewMoistureTypes[0], NewHeatTypes[0], BiomeTable);
		float primaryD = CustomMaths.distance(BiomeInfomation.MoistureToValue[NewMoistureTypes[0]], BiomeInfomation.HeatToValue[NewHeatTypes[0]], MoistureData, HeatData);

		SecondaryBiomeType  = DEFAULT_BIOME_TYPE;
		float secondaryD = float.MaxValue;
		ThirdBiomeType = DEFAULT_BIOME_TYPE;
		float thirdD = float.MaxValue;
		foreach(HeatType h in NewHeatTypes)
		{
			foreach(MoistureType m in NewMoistureTypes)
			{
				BiomeType currentBiome = getBiomeFromEnums(m, h, BiomeTable);
				if (currentBiome != primaryBiomeType)
				{
					float currentD = CustomMaths.distance(BiomeInfomation.MoistureToValue[m], BiomeInfomation.HeatToValue[h], MoistureData, HeatData);
					if (currentD < secondaryD)
					{
						thirdD = secondaryD;
						ThirdBiomeType = SecondaryBiomeType;
						secondaryD = currentD;
						SecondaryBiomeType = currentBiome;
					}
					else if(currentD < thirdD)
					{
						thirdD = currentD;
						ThirdBiomeType = currentBiome;
					}
				}
			}
		}
		if (secondaryD == float.MaxValue)
		{
			secondaryD = primaryD;
			SecondaryBiomeType = primaryBiomeType;
		}
		if (thirdD == float.MaxValue)
		{
			thirdD = secondaryD;
			ThirdBiomeType = SecondaryBiomeType;
		}
		float sum = primaryD + secondaryD + thirdD;
		primaryBiomeDistance = primaryD / sum;
		secondaryBiomeDistance = secondaryD / sum;
		thirdBiomeDistance = thirdD / sum;
	}

	public BiomeType getBiomeFromEnums(MoistureType m, HeatType h, BiomeType[,] BiomeTable)
	{
		return BiomeTable[(int)m, (int)h]; ;
	}

	#region Moisture and Heat types
	public void CalculateClosestMoistureTypes()
	{
		int correctIndex = -1;

		MoistureType[] enumArray = (MoistureType[])System.Enum.GetValues(typeof(MoistureType));
		for (int i = 0; i < enumArray.Length; i++)
		{
			MoistureType type = enumArray[i];
			if (BiomeInfomation.MoistureToValue[type] <= HeatData)
			{
				correctIndex = i;
			}
			else
			{
				//The start value is bigger than the value so we can end.
				break;
			}
		}
		if (correctIndex == -1)
		{
			Debug.Log("Failed to find a correct biome Value: "+HeatData);
			return;
		}
		NewMoistureTypes[0] = enumArray[correctIndex];
		NewMoistureTypes[1] = enumArray[correctIndex];
		NewMoistureTypes[2] = enumArray[correctIndex];
		if (correctIndex + 1 < enumArray.Length)
		{
			NewMoistureTypes[1] = enumArray[correctIndex + 1];
		}
		if (correctIndex - 1 >= 0)
		{
			NewMoistureTypes[2] = enumArray[correctIndex - 1];
		}
	}




	public void CalculateClosestHeatTypes()
	{
		int correctIndex = -1;

		HeatType[] enumArray = (HeatType[])System.Enum.GetValues(typeof(HeatType));
		for (int i = 0; i < enumArray.Length; i++)
		{
			HeatType type = enumArray[i];
			if (BiomeInfomation.HeatToValue[type] <= HeatData)
			{
				correctIndex = i;
			}
			else
			{
				//The start value is bigger than the value so we can end.
				break;
			}
		}
		if (correctIndex == -1)
		{
			Debug.Log("Failed to find a correct biome");
			return;
		}
		NewHeatTypes[0] = enumArray[correctIndex];
		NewHeatTypes[1] = enumArray[correctIndex];
		NewHeatTypes[2] = enumArray[correctIndex];
		if (correctIndex + 1 < enumArray.Length)
		{
			NewHeatTypes[1] = enumArray[correctIndex + 1];
		}
		if (correctIndex - 1 >= 0)
		{
			NewHeatTypes[2] = enumArray[correctIndex - 1];
		}
	}
	#endregion

	public Tile()
	{

	}

	
}
