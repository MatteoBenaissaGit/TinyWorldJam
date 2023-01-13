using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackBuilding : MonoBehaviour
{
    [Header("Referencing"), SerializeField] private List<Transform> _attackOrigins = new List<Transform>();
    [SerializeField] private Building _building;
    [SerializeField] protected GameObject Projectile;
    
    [Header("Attack"), SerializeField] private float _range;
    [SerializeField] protected float Damage;
    [SerializeField] private float _cooldown;
    [SerializeField, Range(0,10)] protected float ProjectileSpeed = 0.03f;
    [SerializeField] private bool _showGizmos;
    [SerializeField] private LayerMask _layerMask;

    [ReadOnly, SerializeField] private List<float> _currentCooldownsList = new List<float>();

    public void Launch()
    {
        _currentCooldownsList.Clear();
        for (int i = 1; i <= _building.CurrentAntsInBuilding; i++)
        {
            _currentCooldownsList.Add(_cooldown/i);
        }
    }

    public virtual void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Attack)
        {
            CooldownManagement();
        }
    }

    private void CooldownManagement()
    {
        for (int i = 0; i < _currentCooldownsList.Count; i++)
        {
            _currentCooldownsList[i] -= Time.deltaTime;
            if (_currentCooldownsList[i] <= 0)
            {
                Attack(i);
                _currentCooldownsList[i] = _cooldown;
            }
        }
    }

    public virtual void Attack(int origin)
    {
        //setup
        Vector3 startPosition = _attackOrigins[origin].transform.position;
        List<Enemy> enemiesInRange = new List<Enemy>();

        //detection 
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, 
            new Vector3(_range/2,100,_range/2), Vector3.up, Quaternion.identity, 0.01f, _layerMask);
        
        foreach (RaycastHit ray in hits)
        {
            Enemy enemy = ray.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null && enemy.CanBeTargeted)
            {
                enemiesInRange.Add(enemy);
            }
        }
        
        //attack
        if (enemiesInRange.Count > 0)
        {
            Enemy enemy = enemiesInRange.ToList().OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
            LaunchAttack(startPosition, enemy);
            
            //anim
            _attackOrigins[origin].DOComplete();
            _attackOrigins[origin].DOPunchScale(Vector3.one * 0.15f, 0.2f);
        }
    }

    public virtual void LaunchAttack(Vector3 startPosition, Enemy enemy)
    {
        
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (_showGizmos == false)
        {
            return;
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_range,200,_range));
    }

#endif
}
