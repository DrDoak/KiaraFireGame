using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public GameObject chargedSpawn;
    public GameObject largeChargeSpawn;
    public bool spawnNow;
    [SerializeField]
    public CharacterComponents components;
    [SerializeField]
    public PlayerCharacter player;
    private bool lastSpawnNow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnNow && !lastSpawnNow)
        {
            if (chargedSpawn != null || largeChargeSpawn != null)
            {
                ChargeSpawn();
            } else
            {
                SpawnObject(objectToSpawn);
            }
            
        }
        lastSpawnNow = spawnNow;
    }
    private void ChargeSpawn()
    {
        if (player.HasLargeCharge() && largeChargeSpawn != null)
        {
            SpawnObject(largeChargeSpawn);
        } else if (player.HasSmallCharge() && chargedSpawn != null)
        {
            SpawnObject(chargedSpawn);
        } else
        {
            SpawnObject(objectToSpawn);
        }
    }
    private void SpawnObject(GameObject obj)
    {
        if (obj == null) return;
        GameObject newObj = Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        if (newObj.GetComponent<BaseMovement>() != null)
        {
            newObj.GetComponent<BaseMovement>().SetFacingLeft(components.MMovement.FacingLeft);
        }
        if (newObj.GetComponent<Projectile>() != null)
        {
            newObj.GetComponent<Projectile>().SetParent(components.MAttackConfirm);
        }
    }
}
