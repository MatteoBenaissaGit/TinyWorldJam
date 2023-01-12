﻿using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MandibuleTowerAttack : AttackBuilding
{
    private List<Projectile> _projectiles = new List<Projectile>();

    public override void Update()
    {
        base.Update();
        List<Projectile> list = _projectiles.ToList();
        foreach (Projectile projectile in list)
        {
            if (projectile.ProjectileLaunched == null)
            {
                _projectiles.Remove(projectile);
                continue;
            }
            if (projectile.EnemyToAim == null)
            {
                projectile.ProjectileLaunched.transform.DOScale(Vector3.zero, 0.3f);
                _projectiles.Remove(projectile);
                continue;
            }
            
            Vector3 position = projectile.ProjectileLaunched.transform.position;
            Vector3 enemyPosition = projectile.EnemyToAim.transform.position;
            
            projectile.ProjectileLaunched.transform.position =
                Vector3.MoveTowards(position, enemyPosition, 0.01f);
            
            if (Vector3.Distance(position, enemyPosition) < 0.01f)
            {
                projectile.EnemyToAim.SetLife(-Damage);
                _projectiles.Remove(projectile);
                Destroy(projectile.ProjectileLaunched);
            }
        }
    }

    public override void LaunchAttack(Vector3 startPosition, Enemy enemy)
    {
        base.LaunchAttack(startPosition, enemy);
        GameObject projectile = Instantiate(Projectile, startPosition, Quaternion.identity);
        _projectiles.Add(new Projectile(){ProjectileLaunched = projectile, EnemyToAim = enemy});
        
        //anim
        projectile.transform.localScale = Vector3.zero;
        projectile.transform.DOScale(Vector3.one, 0.5f);
    }
}

public struct Projectile
{
    public GameObject ProjectileLaunched;
    public Enemy EnemyToAim;
}