using UnityEngine;

namespace Buildings
{
    public class Arrival : Building
    {
        [Space(10), Header("Arrival"), SerializeField] private int _life;

        public void SetLife(int value)
        {
            _life += value;
        }
    }
}