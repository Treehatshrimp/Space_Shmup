using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S { get; private set; }//Singleton
    [Header("Inscribed")]
    //Control movement of ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    [Header("Dynamic")]
    [Range(0, 4)]
    [SerializeField]
    private float _shieldLevel = 1;
    //public float shieldLevel = 1;
    [Tooltip("This field holds a referenece to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;

    private void Awake()
    {
        if(S == null)
        {
            S = this; //Set Singleton only if null
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Pull info from the Input class
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        //Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        //Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        // Debug.Log("Shield trigger hit by: " +go.gameObject.name);
        //Make sure it's not the same triggering go as last time
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy == null)
        { //If the shield was triggered by an enemy
            shieldLevel--; //Decrease the level of the shield by 1
            Destroy(go);
        }
        else 
        {
            Debug.LogWarning("Shield trigger by non-Enemy: "+go.name);
        }
    }
    public float shieldLevel
    {
        get { return shieldLevel; }
        private set 
        {
            _shieldLevel = Mathf.Min(value,4); 
            //If the shield is going to be set to less than zero...
            if(value <0)
            {
                Destroy(this.gameObject);//Destoy the Hero
            }
        }
    }
}
