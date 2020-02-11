using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExampleSO", order = 100)]
public class ExampleScriptableObject : ScriptableObject
{

    public int sampleInt = 3;
    public float sampleFloat = 20.0f;
	public Color floorColor;
    public Mesh[] meshArray;
}
