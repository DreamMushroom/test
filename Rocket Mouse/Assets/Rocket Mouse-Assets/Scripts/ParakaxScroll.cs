using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParakaxScroll : MonoBehaviour
{
    // Start is called before the first frame update
    public Renderer background;
    public Renderer foreground;
    public float backgroundspeed;
    public float foregroundspeed;
    public float offset;

    private void Update()
    {
        float backgroundOffset = offset * backgroundspeed;
        float foregroundOffset = offset * foregroundspeed;

        background.material.mainTextureOffset = new Vector2(backgroundOffset, 0);
        foreground.material.mainTextureOffset = new Vector2(foregroundOffset, 0);
    }
}
