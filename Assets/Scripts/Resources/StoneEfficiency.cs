using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Resources
{
    public class StoneEfficiency : MonoBehaviour
    {
        public static StoneEfficiency instance;

        public int size = 1000;

        public int startX = -500;
        public int startY = -500;

        public int[,] efficiencyArray;

        public int lastEfficiency;

        public Text efficiencyText;

        public List<LineRenderer> radiusCircleList = new List<LineRenderer>();

        [HideInInspector] public List<Quarry> quarryList = new List<Quarry>();

        private void Awake()
        {
            if (instance == null) instance = this;
            if (instance != this) Destroy(gameObject);

            CreateStoneMap();
        }

        private void CreateStoneMap()
        {
            efficiencyArray = new int[size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    efficiencyArray[x, y] = Random.Range(30, 100);
                }
            }
        }

        public int GetEfficiencyInPosition(Vector3 position)
        {
            int xIndex = (int)position.x + (size / 2);
            int yIndex = (int)position.y + (size / 2);

            return efficiencyArray[xIndex, yIndex];
        }

        public void SetCirclesState(bool state)
        {
            foreach (LineRenderer line in radiusCircleList)
            {
                line.enabled = state;
            }
        }
    }
}
