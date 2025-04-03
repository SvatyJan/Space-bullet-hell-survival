using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrizator : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject lightningFx;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float baseDamage = 5f;
    [SerializeField] private float baseRadius = 5f;
    [SerializeField] private int baseChainCount = 4;
    [SerializeField] private float lightningDelay = 0.1f;

    [SerializeField] private List<GameObject> hitTargets = new List<GameObject>();
    private float nextFireTime = 0f;

    /** Seznam tagù, se kterými projektil mùže kolidovat. */
    [SerializeField] private List<string> collisionTags;

    private SpaceEntity owner;

    private void Start()
    {
        owner = GetComponentInParent<SpaceEntity>();
        baseDamage += owner.GetComponent<ShipStats>().BaseDamage;
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;
        StartCoroutine(FireLightningSequence());
    }

    private IEnumerator FireLightningSequence()
    {
        SpaceEntity currentTarget = FindClosestEnemyFromPosition(new Vector2(transform.position.x, transform.position.y));
        int chainCount = baseChainCount;
        Vector3 previousPosition = transform.position;

        for (int i = 0; i < chainCount; i++)
        {
            if (currentTarget != null)
            {
                currentTarget.TakeDamage(baseDamage);
                Debug.Log(currentTarget.name + " has been lightning struck!");

                GameObject lightningEffect = Instantiate(lightningFx, currentTarget.transform.position, Quaternion.identity);
                Destroy(lightningEffect, 1f);

                hitTargets.Add(currentTarget.gameObject);

                yield return new WaitForSeconds(lightningDelay);

                previousPosition = currentTarget.transform.position;

                currentTarget = FindClosestEnemyFromPosition(currentTarget.transform.position);
            }
            else
            {
                hitTargets.Clear();
                break;
            }
        }
        hitTargets.Clear();
    }

    private SpaceEntity FindClosestEnemyFromPosition(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, baseRadius);
        SpaceEntity closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collisionTags.Contains(collider.tag) && !hitTargets.Contains(collider.gameObject))
            {
                float enemyDistance = Vector2.Distance(position, collider.transform.position);
                if (enemyDistance < minDistance)
                {
                    minDistance = enemyDistance;
                    closestEnemy = collider.GetComponent<SpaceEntity>();
                }
            }
        }

        return closestEnemy;
    }

    public void Upgrade()
    {
        baseChainCount++;
    }

    public void Evolve()
    {
        baseChainCount += 2;
    }
}
