using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSettings 
{
    public float DETAIL_MIN_WIDTH = 1;
    public float DETAIL_MAX_WIDTH = 1.2f;
    public float DETAIL_MIN_HEIGHT = 0.4f;
    public float DETAIL_MAX_HEIGHT = 0.5f;
    public float DETAIL_NOISE_SPREAD = 10;
    public Color DETAIL_HEALTHY_COLOR = new Color(59f / 255, 115f / 255, 27f / 255);
    public Color DETAIL_DRY_COLOR = new Color(107f / 255, 209f / 255, 76f / 255);
    public DetailRenderMode DETAIl_RENDER_MODE = DetailRenderMode.Grass;

    public int detailID = -1;


    public static GrassSettings get_type(GrassTypes type)
    {
        switch(type)
        {
            case GrassTypes.DesertGrass:
                return get_DesertGrass();
            case GrassTypes.GrasslandGrass:
                return get_GrasslandGrass();
            case GrassTypes.WoodlandGrass:
                return get_WoodlandGrass();
            case GrassTypes.RainforestGrass:
                return get_RainforestGrass();
            case GrassTypes.DeadGrass:
                return get_DeadGrass();
            case GrassTypes.SnowGrass:
                return get_SnowGrass();
            case GrassTypes.Straw:
                return get_Straw();
            case GrassTypes.Flower:
                return get_Flower();
            default:
                Debug.Log("did not find right type: " + type.ToString());
                return new GrassSettings();
        }
    }

    private static GrassSettings get_GrasslandGrass()
    {
        GrassSettings temp = new GrassSettings();
        temp.DETAIL_MIN_WIDTH = 1;
        temp.DETAIL_MAX_WIDTH = 1.2f;
        temp.DETAIL_MIN_HEIGHT = 0.4f;
        temp.DETAIL_MAX_HEIGHT = 0.5f;
        temp.DETAIL_NOISE_SPREAD = 10;
        temp.DETAIL_HEALTHY_COLOR = new Color(82f / 255, 175f / 255, 63f / 255);
        temp.DETAIL_DRY_COLOR = new Color(142f / 255, 191f / 255, 58f / 255);
        temp.DETAIl_RENDER_MODE = DetailRenderMode.Grass;
        return temp;
    }

    private static GrassSettings get_WoodlandGrass()
    {
        GrassSettings temp = new GrassSettings();
        temp.DETAIL_MIN_WIDTH = 1;
        temp.DETAIL_MAX_WIDTH = 1.2f;
        temp.DETAIL_MIN_HEIGHT = 0.4f;
        temp.DETAIL_MAX_HEIGHT = 0.5f;
        temp.DETAIL_NOISE_SPREAD = 10;
        temp.DETAIL_HEALTHY_COLOR = new Color(59f / 255, 115f / 255, 27f / 255);
        temp.DETAIL_DRY_COLOR = new Color(107f / 255, 209f / 255, 76f / 255);
        temp.DETAIl_RENDER_MODE = DetailRenderMode.Grass;
        return temp;
    }

    private static GrassSettings get_RainforestGrass()
    {
        GrassSettings temp = new GrassSettings();
        temp.DETAIL_MIN_WIDTH = 1;
        temp.DETAIL_MAX_WIDTH = 1.2f;
        temp.DETAIL_MIN_HEIGHT = 0.4f;
        temp.DETAIL_MAX_HEIGHT = 0.5f;
        temp.DETAIL_NOISE_SPREAD = 10;
        temp.DETAIL_HEALTHY_COLOR = new Color(11f / 255, 218f / 255, 18f / 255);
        temp.DETAIL_DRY_COLOR = new Color(142f / 255, 218f / 255, 11f / 255);
        temp.DETAIl_RENDER_MODE = DetailRenderMode.Grass;
        return temp;
    }

    private static GrassSettings get_DeadGrass()
    {
        GrassSettings temp = new GrassSettings();
        temp.DETAIL_MIN_WIDTH = 1;
        temp.DETAIL_MAX_WIDTH = 1.2f;
        temp.DETAIL_MIN_HEIGHT = 0.4f;
        temp.DETAIL_MAX_HEIGHT = 0.5f;
        temp.DETAIL_NOISE_SPREAD = 10;
        temp.DETAIL_HEALTHY_COLOR = new Color(170f / 255, 233f / 255, 61f / 255);
        temp.DETAIL_DRY_COLOR = new Color(205f / 255, 233f / 255, 61f / 255);
        temp.DETAIl_RENDER_MODE = DetailRenderMode.Grass;
        return temp;
    }

    private static GrassSettings get_SnowGrass()
    {
        GrassSettings temp = new GrassSettings();
        temp.DETAIL_MIN_WIDTH = 1;
        temp.DETAIL_MAX_WIDTH = 1.2f;
        temp.DETAIL_MIN_HEIGHT = 0.4f;
        temp.DETAIL_MAX_HEIGHT = 0.5f;
        temp.DETAIL_NOISE_SPREAD = 10;
        temp.DETAIL_HEALTHY_COLOR = new Color(245f / 255, 246f / 255, 236f / 255);
        temp.DETAIL_DRY_COLOR = new Color(229f / 255, 230f / 255, 219f / 255);
        temp.DETAIl_RENDER_MODE = DetailRenderMode.Grass;
        return temp;
    }

    private static GrassSettings get_Straw()
    {
        GrassSettings temp = new GrassSettings();
        temp.DETAIL_MIN_WIDTH = 1;
        temp.DETAIL_MAX_WIDTH = 1.2f;
        temp.DETAIL_MIN_HEIGHT = 0.4f;
        temp.DETAIL_MAX_HEIGHT = 0.5f;
        temp.DETAIL_NOISE_SPREAD = 10;
        temp.DETAIL_HEALTHY_COLOR = new Color(228f / 255, 217f / 255, 111f / 255);
        temp.DETAIL_DRY_COLOR = new Color(227f / 255, 200f / 255, 129f / 255);
        temp.DETAIl_RENDER_MODE = DetailRenderMode.Grass;
        return temp;
    }

    private static GrassSettings get_Flower()
    {
        GrassSettings temp = new GrassSettings();
        temp.DETAIL_MIN_WIDTH = 1;
        temp.DETAIL_MAX_WIDTH = 1.2f;
        temp.DETAIL_MIN_HEIGHT = 0.4f;
        temp.DETAIL_MAX_HEIGHT = 0.5f;
        temp.DETAIL_NOISE_SPREAD = 10;
        temp.DETAIL_HEALTHY_COLOR = new Color(59f / 255, 115f / 255, 27f / 255);
        temp.DETAIL_DRY_COLOR = new Color(107f / 255, 209f / 255, 76f / 255);
        temp.DETAIl_RENDER_MODE = DetailRenderMode.Grass;
        return temp;
    }

    private static GrassSettings get_DesertGrass()
    {
        GrassSettings temp = new GrassSettings();
        temp.DETAIL_MIN_WIDTH = 1;
        temp.DETAIL_MAX_WIDTH = 1.2f;
        temp.DETAIL_MIN_HEIGHT = 0.4f;
        temp.DETAIL_MAX_HEIGHT = 0.5f;
        temp.DETAIL_NOISE_SPREAD = 10;
        temp.DETAIL_HEALTHY_COLOR = new Color(202f / 255, 189f / 255, 102f / 255);
        temp.DETAIL_DRY_COLOR = new Color(221f / 255, 215f / 255, 195f / 255);
        temp.DETAIl_RENDER_MODE = DetailRenderMode.Grass;
        return temp;
    }

}
