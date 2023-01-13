using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Referencing")] [SerializeField] private GameObject _lifeUI;
    [SerializeField] private Image _lifeImage;
    [SerializeField] private ParticleSystem _deathParticle;

    [Header("Enemy")] [SerializeField] private float _life;
    public float Damage;
    [SerializeField] private float _offsetY = 1.2f;

    [Header("Debug"), ReadOnly] public float Timer;

    private float _currentLife;
    [HideInInspector] public List<Tile> TilePath = new List<Tile>();
    private int _currentTile;
    [HideInInspector] public float MoveTime;
    [HideInInspector] public Sequence TweenSequence;
    [ReadOnly] public bool CanBeTargeted = true;

    private void Start()
    {
        _currentLife = _life;
        if (_lifeImage != null && _lifeUI != null)
        {
            _lifeImage.fillAmount = 1;
            _lifeUI.SetActive(false);
        }

        //sequence tile
        TweenSequence = DOTween.Sequence();
        for (int i = 0; i < TilePath.Count - 1; i++)
        {
            TweenSequence.Append(
                    transform.DOMove(TilePath[i + 1].transform.position + new Vector3(0, _offsetY, 0), MoveTime).SetEase(Ease.Linear));
        }

        TweenSequence.OnComplete(AttainedArrival);
        TweenSequence.Play();
    }

    private void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            AttainedArrival();
        }
    }

    public void SetLife(float value)
    {
        _currentLife += value;
        _lifeImage.DOComplete();
        _lifeImage.DOFillAmount(_currentLife / _life, 0.1f);
        
        if (_currentLife < _life && _lifeUI.activeInHierarchy == false)
        {
            _lifeUI.SetActive(true);
            Vector3 scale = _lifeUI.transform.localScale;
            _lifeUI.transform.localScale = Vector3.zero;
            _lifeUI.transform.DOScale(scale, 0.3f);
        }

        if (_currentLife <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (CanBeTargeted == false)
        {
            return;    
        }
        
        transform.DOKill();
        TweenSequence.Kill();
        transform.DOScale(Vector3.zero, 0.3f).OnComplete(DestroyItself);
        _deathParticle.Play(); 
        _deathParticle.transform.SetParent(GameManager.Instance.transform);
        _deathParticle.transform.localScale = Vector3.one * 0.3f;
        CanBeTargeted = false;
        if (FindObjectsOfType<Enemy>().Length <= 1)
        {
            GameManager.Instance.RoundsAndDefenseManager.EndOfWave();
        }
    }

    private void DestroyItself()
    {
        Destroy(gameObject);
    }

    private void AttainedArrival()
    {
        if (CanBeTargeted == false || _currentLife <= 0)
        {
            return;    
        }
        
        GameManager.Instance.EnemyAttainArrival(this);
        Die();
    }
}