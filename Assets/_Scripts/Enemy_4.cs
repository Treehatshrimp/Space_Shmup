using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyShield))]
public class Enemy_4 : Enemy
{
    private EnemyShield[] allShields;
    private EnemyShield thisShield;

    void Start()
    {
        allShields = GetComponentsInChildren<EnemyShield>();
        thisShield = GetComponent<EnemyShield>();
    }

    public override void Move()
    {
        //  we will add much more here later for now we will test
        // without enemny movement
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
