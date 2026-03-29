using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f; // Movement
    public float fireRate = 0.3f; // Seconds/shot
    public float health = 10;
    public int score = 100; // Points earned for destroying
    public float powerUpDropChance = 1f; //chance to drop a PowerUp
    protected bool calledShipDestroyed = false;

    protected BoundsCheck bndCheck;
    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }
    // Property: a method that acts like a field
    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }
    void Update()
    {
        Move();
        // Check whether this Enemy has gone off the bottom of the screen
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
        {
            Destroy(gameObject);
        }
        /*if(!bndCheck.isOnScreen)
        {
            if(pos.y < bndCheck.camHeight - bndCheck.radius)
            {
                //Off the bottom, destroy object
                Destroy(gameObject);
            }
        }
        */
    }
    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
    // private void OnCollisionEnter(Collision coll)
    // {
    //     GameObject otherGO = coll.gameObject;
    //     if(otherGO.GetComponent<ProjectileHero>() != null)
    //     {
    //         Destroy (otherGO); //Destroy Projectile
    //         Destroy(gameObject); //Destory this Enemy GameObject
    //     }
    //     else
    //     {
    //         Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name);
    //     }
    // }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        // check for collision with projectile hero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            // only damage this enemy if its on screen
            if (bndCheck.isOnScreen)
            {
                // get the damage ammount from the main weap dict
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health <= 0)
                {
                    if(!calledShipDestroyed)
                    {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                    // destory this enemy
                    Destroy(this.gameObject);
                }
            }
            // destory the porjectilehero reguardless
            Destroy(otherGO);
        }
        else
        {
            print("Enemy hit by non-ProjectileHero " + otherGO.name);
        }
    }
}
