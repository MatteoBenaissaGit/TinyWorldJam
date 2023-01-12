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
        }
    }
}