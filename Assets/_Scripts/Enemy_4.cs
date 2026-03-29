using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyShield))]
public class Enemy_4 : Enemy
{
    [Header("Enemy_4 Inscribed Fields")]
    public float duration = 4;  // duration of interpolation movement
    private EnemyShield[] allShields;
    private EnemyShield thisShield;

    private Vector3 p0, p1;  // the two points to interpolate
    private float timeStart;    // birth time for this enemy_4

    void Start()
    {
        allShields = GetComponentsInChildren<EnemyShield>();
        thisShield = GetComponent<EnemyShield>();

        // initaly set p0 & p1 to current position from main spawn emeny
        p0 = p1 = pos;
        InitMovement();
    }

    void InitMovement()
    {
        p0 = p1;    // set p0 ot the old p1
        //assign a new on screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // make sure that it moves to a diffrent quadrant of the screen
        if (p0.x * p1.x > 0 && p0.y * p1.y > 0)
        {
            if (Mathf.Abs(p0.x) > Mathf.Abs(p0.y))
            {
                p1.x *= -1;
            }
            else
            {
                p1.y *= -1;
            }
        }

        // reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        // this completly overrides emeny move with a linear interpolation
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = u - 0.15f * Mathf.Sin(u * 2 * Mathf.PI); //easing sine -0.15
        pos = (1 - u) * p0 + u * p1;  // simple linear interpolation

    }

    /// <summary>
    /// Enemy_4 Collisions are handled diffrently from other Enemy subclasses
    /// to enable protection by Enemy Shields.
    /// </summary>
    /// <param name="coll"></param>
    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        // amke sure this was hit by a ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            // destroy the ProjectileHero reguardless of bndCheck.isOnScreen
            Destroy(otherGO);

            // only damage this enemy if it is on screen
            if (bndCheck.isOnScreen)
            {
                // find the GO of this enemy4 that was hit
                GameObject hitGO = coll.contacts[0].thisCollider.gameObject;
                if (hitGO == otherGO)
                {
                    hitGO = coll.contacts[0].otherCollider.gameObject;
                }

                // get the damage ammount from the main weapon dict
                float dmg = Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;

                // find the EnemyShield that was hit (if there was one)
                bool shieldFound = false;
                foreach (EnemyShield es in allShields)
                {
                    if (es.gameObject == hitGO)
                    {
                        es.TakeDamage(dmg);
                        shieldFound = true;
                    }
                }
                if (!shieldFound) thisShield.TakeDamage(dmg);

                //  if thisShield is still active, then it has not been destroyed
                if (thisShield.isActive) return;

                // this ship was destrows so tell main about it
                if (!calledShipDestroyed)
                {
                    Main.SHIP_DESTROYED(this);
                    calledShipDestroyed = true;
                }

                // destroy this enemy4
                Destroy(gameObject);
            }
        }
        else Debug.Log("Enemy_4 hit by non-ProjectileHero: " + otherGO.name);
    }
}
