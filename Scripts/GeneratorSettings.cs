using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class GeneratorSettings
{
    public static string folderPath = @"\Resources\BiomeMapSettings\";


    public int octaves = 2;
    public int frequency = 2;
    public float scale = 70;
    public float amplitudeMod = 0.3f;
    public float frequencyMod = 1.5f;


    #region XML

    /// <summary>
    /// Outputs object to XML file to the Resources folder
    /// Code taken from: https://stackoverflow.com/questions/13266496/easily-write-a-whole-class-instance-to-xml-file-and-read-back-in
    /// </summary>
    /// <param name="name">File name</param>
    public void OutputToXML(string name)
    {
        // TODO init your garage..

        XmlSerializer xs = new XmlSerializer(typeof(GeneratorSettings));
        //TextWriter tw = new StreamWriter(@"c:\temp\garage.xml");
        string path = Application.dataPath + folderPath + name + ".xml";
        TextWriter tw = new StreamWriter(path);
        xs.Serialize(tw, this);
        tw.Close();
        Debug.Log("XML output done");
    }

    public static GeneratorSettings InputFromXML(string name)
    {
        string path = Application.dataPath + folderPath + name + ".xml";
        using (var sr = new StreamReader(@path))
        {
            XmlSerializer xs = new XmlSerializer(typeof(GeneratorSettings));
            GeneratorSettings temp = (GeneratorSettings)xs.Deserialize(sr);
            return temp;
        }
    }
    #endregion
}
