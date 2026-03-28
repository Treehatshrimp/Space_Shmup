using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BlinkColorOnHit : MonoBehaviour
{
    private static float blinkDuration = 0.1f;  // # of seconds to show damage
    //private static Color blinkColor = Color.red;

    [Header("Dynamic")]
    public bool showingColor = false;
    public float blinkCompleteTime; // time to stop show damage
    public Color blinkColor = Color.red;

    private Material[] materials;   // time to stpo showing the color
    private Color[] originalColors;
    private BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponentInParent<BoundsCheck>();
        // get mats and colors for this go and its children
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    void Update()
    {
        if (showingColor && Time.time > blinkCompleteTime) RevertColors();
    }

    void OnCollisionEnter(Collision coll)
    {
        // check for collisons with projectile hero
        ProjectileHero p = coll.gameObject.GetComponent<ProjectileHero>();
        if (p != null)
        {
            if (bndCheck != null && !bndCheck.isOnScreen)
            {
                return; // dont show damage if this is off screen
            }
        }
        SetColors();
    }


    void SetColors()
    {
        foreach (Material m in materials)
        {
            m.color = blinkColor;
        }
        showingColor = true;
        blinkCompleteTime = Time.time + blinkDuration;
    }

    /// <summary>
    /// Revers all materials in the materials array back to their oridinal color
    /// and sets showing color to false
    /// </summary>
    void RevertColors()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingColor = false;
    }

}
