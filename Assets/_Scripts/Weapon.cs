using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is an enum of the various possible weapon types.
/// it also includes a "Shield" type to allow a shield powerup
/// items marked [NI] are not implemeted in the book
/// </summary>
public enum eWeaponType
{
    none, // the defualt / no weapon
    blaster, // a simple blaster
    spread, // mutple shots simultaneously
    phaser, // [NI] shots that move in waves
    missile, // [NI] homing missiles
    laser, // [NI]f Damge over time
    shield // raise shieldLevel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties
/// of a specific weapon in the inspector. the main class has
/// an array of weapon defintions that makes this posible
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;
    [Tooltip("Letter to show on the PowerupCube")]
    public string letter;
    [Tooltip("Color of PowerupCube")]
    public Color powerUpColor = Color.white;
    [Tooltip("Prefab of the model that is attached to the player ship")]
    public GameObject weaponModelPrefab;
    [Tooltip("Prefab of prohectile that is fired")]
    public GameObject projectilePrefab;
    [Tooltip("Color of the projectile that is fired")]
    public Color projectileColor = Color.white;
    [Tooltip("Damage cuasedwhen a single projectile hits an enemy")]
    public float damageOnHit = 0;


    [Tooltip("Damage cuased per second by the laser  [Not Implemented]")]
    public float damagePerSec = 0;
    [Tooltip("Seconds of Delat between shots")]
    public float delayBetweenShots = 0;
    [Tooltip("Velocity of indiviudal projectiles")]
    public float velocity = 50;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Dynamic")]
    [SerializeField]
    [Tooltip("Setting this manually while playing does not work properly.")]
    private eWeaponType _type = eWeaponType.none;
    public WeaponDefinition def;
    public float nextShotTime;     // tiume the weapon will fire next

    private GameObject weaponModel;
    private Transform shotPointTrans;

    void Start()
    {
        // Set up PROJECTILE_ANCHOR if it has not already been done
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjetileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        shotPointTrans = transform.GetChild(0);

        // call SetType() for the default _type set in the Inspector
        SetType(_type);

        // find the fireEvent of a Hero Component in the parent hierarchy
        Hero hero = GetComponentInParent<Hero>();
        if (hero != null) hero.fireEvent += Fire;
    }

    public eWeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt)
    {
        _type = wt;
        if (type == eWeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        // get the weapon def for this type from main
        def = Main.GET_WEAPON_DEFINITION(_type);
        //Destroy any old model and then attach a model for this weapon
        if (weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0;   // you can fire immediatly after _type is set.

    }

    private void Fire()
    {
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        // if it hasn't been enough time between shots, return
        if (Time.time < nextShotTime) return;

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;

        switch (type)
        {
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.vel = vel;
                break;

            case eWeaponType.spread:
                p = MakeProjectile();
                p.vel = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;


        }
    }

    private ProjectileHero MakeProjectile()
    {
        GameObject go;
        go = Instantiate<GameObject>(def.projectilePrefab, PROJECTILE_ANCHOR);
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return (p);
    }
}
