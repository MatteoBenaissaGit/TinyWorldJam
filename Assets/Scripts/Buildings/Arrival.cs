using DG.Tweening;
using UnityEngine;

namespace Buildings
{
    public class Arrival : Building
    {
        [Space(10), Header("Arrival"), SerializeField] public int Life;

        [ReadOnly] public float CurrentLife;

        public override void Start()
        {
            base.Start();
            CurrentLife = Life;
        }

        public void SetLife(float value)
        {
            CurrentLife += value;
            if (CurrentLife <= 0)
            {
                Die();
            }
            else
            {
                transform.DOComplete();
                transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
            }
        }

        private void Die()
        {
            transform.DOComplete();
            transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InExpo);
            GameManager.Instance.ChangeState(GameState.Lose);
        }
    }
}