using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public bool spawnNow;
    [SerializeField]
    public CharacterComponents components;
    [SerializeField]
    public PlayerCharacter player;
    private bool lastSpawnNow;
    [Header("shotgun")]
    [SerializeField]
    private int numShots = -1;
    [SerializeField]
    private float angleSpread;
    [SerializeField]
    private float YoffsetSpread;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnNow && !lastSpawnNow)
        {
            if (numShots > 0)
            {
                ShotgunShots(objectToSpawn);
            } else
            {
                SpawnObject(objectToSpawn);
            }
        }
        lastSpawnNow = spawnNow;
    }
    private void ShotgunShots(GameObject obj)
    {
        if (angleSpread > 0)
        {
            float angle = -((angleSpread * Mathf.Deg2Rad ) / 2);
            float angleDivision = (angleSpread * Mathf.Deg2Rad) / numShots;
            for (int i = 0; i < numShots; i++)
            {
                GameObject go = SpawnObject(obj);
                go.GetComponent<Projectile>().SetAngleOffset(angle);
                angle += angleDivision;
            }
        } else if (YoffsetSpread > 0)
        {
            float offset = -(YoffsetSpread / 2);
            float division = YoffsetSpread / numShots;
            for (int i = 0; i < numShots; i++)
            {
                GameObject go = SpawnObject(obj);
                Projectile p = go.GetComponent<Projectile>();
                p.initialVelocity = new Vector2(p.initialVelocity.x, p.initialVelocity.y + offset);
                offset += division;
            }
        }
    }
    public GameObject SpawnObject(GameObject obj)
    {
        if (obj == null) return null;
        GameObject newObj = Instantiate(obj, transform.position, Quaternion.identity);
        if (newObj.GetComponent<BaseMovement>() != null)
        {
            newObj.GetComponent<BaseMovement>().SetFacingLeft(components.MMovement.FacingLeft);
        } else
        {
            newObj.transform.localScale = new Vector3((components.MMovement.FacingLeft ? -1 : 1) * newObj.transform.localScale.x,
                newObj.transform.localScale.y, newObj.transform.localScale.z);
        }
        if (newObj.GetComponent<Projectile>() != null)
        {
            newObj.GetComponent<Projectile>().SetParent(components.MAttackConfirm);
        }
        if (newObj.GetComponent<Attackable>() != null)
        {
            newObj.GetComponent<Attackable>().MyFaction = components.MAttackable.MyFaction;
        }
        return newObj;
        //foreach (Hitbox hb in newObj.GetComponentsInChildren<Hitbox>(includeInactive: true))
        //{
        //    hb.SetParent(components.MAttackable);
        //}
    }
}
