using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectPlacerController))]
public class ObjectPlacerControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
	}
}
