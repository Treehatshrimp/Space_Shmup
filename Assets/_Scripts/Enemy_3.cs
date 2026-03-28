using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
    [Header("Enemy_3 Inscribed Feilds")]
    public float lifeTime = 5;
    public Vector2 midpointYRange = new Vector2(1.5f, 3);
    [Tooltip("If true, the bezier points & path are drawn in the Scene pane.")]
    public bool drawDebugInfo = true;

    [Header("Enemy_3 Private Fields")]
    [SerializeField] private Vector3[] points; //the three points for the bezier curve
    [SerializeField] private float birthTime;

    // start
    void Start()
    {
        points = new Vector3[3];    // ini pts
        // start pos has been set by main spawn
        points[0] = pos;
        // set xmin and xmax the same way that main spawn does
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        // pick and rand middle pos in the bott half of screen
        points[1] = Vector3.zero;
        points[1].x = Random.Range(xMin, xMax);
        float midYMult = Random.Range(midpointYRange[0], midpointYRange[1]);
        points[1].y = -bndCheck.camHeight * midYMult;

        // pick a rand final pos above the top of the screen
        points[2] = Vector3.zero;
        points[2].y = pos.y;
        points[2].x = Random.Range(xMin, xMax);

        // set birth time to curr time
        birthTime = Time.time;

        if (drawDebugInfo) DrawDebug();

    }

    public override void Move()
    {
        // bezier curves work based on a u value between 0 & 1
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1)
        {
            // this enemy_# has finished its life
            Destroy(this.gameObject);
            return;
        }

        transform.rotation = Quaternion.Euler(u * 180, 0, 0);
        // interpolate the 3 bezier curve pts
        u = u - 0.1f * Mathf.Sin(u * Mathf.PI * 2);
        pos = Utils.Bezier(u, points);
        // enemy_3 does not call base.Move()
    }

    void DrawDebug()
    {
        // draw the three points
        Debug.DrawLine(points[0], points[1], Color.cyan, lifeTime);
        Debug.DrawLine(points[1], points[2], Color.yellow, lifeTime);

        // draw the curve
        float numSections = 20;
        Vector3 prevPoint = points[0];
        Color col;
        Vector3 pt;
        for (int i = 1; i < numSections; i++)
        {
            float u = i / numSections;
            pt = Utils.Bezier(u, points);
            col = Color.Lerp(Color.cyan, Color.yellow, u);
            Debug.DrawLine(prevPoint, pt, col, lifeTime);
            prevPoint = pt;

        }
    }
}
