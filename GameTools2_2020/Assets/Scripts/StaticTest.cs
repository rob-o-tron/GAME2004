using UnityEngine;

public class StaticTest : MonoBehaviour
{
    public static float staticField=0.0f;
    public float inputField = 2.0f;
    public float resultField = 2.0f;

    void Update()
    {
        if (transform.position.y>3.0f)
            staticField = inputField;

        resultField = staticField;
    }
}
