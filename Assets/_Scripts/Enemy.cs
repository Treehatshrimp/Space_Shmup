using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f; //Movement
    public float fireRate = 0.3f; //Seconds/shot
    public float health = 10; 
    public int score = 100; //Points earned for destroying

    private BoundsCheck bndCheck;
    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }
    //Property: a method that acts like a field
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
    void Update ()
    {
        Move();
        //Check whether this Enemy has gone off the bottom of the screen
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
    public virtual void Move ()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
}
