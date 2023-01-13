using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ProjectileParticle : MonoBehaviour
{
    [SerializeField] private GameObject _particle;
    [SerializeField] private GameObject _sphere;

    public void DestroyParticle()
    {
        _particle.transform.SetParent(GameManager.Instance.transform);
        _particle.transform.DOScale(Vector3.zero, .8f).OnComplete(DestroyItself);
        _sphere.transform.DOScale(Vector3.zero, .4f);
        _particle.transform.localScale = Vector3.one;
    }

    private void DestroyItself()
    {
        Destroy(gameObject);
    }
}
