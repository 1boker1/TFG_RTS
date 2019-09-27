using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Building
{
    public class Stockpile : MonoBehaviour
    {
        [SerializeField] private float increaseAmount = 1000;

        public void IncreaseCapacity() => ResourceManager.Instance.ModifyMaxCapacity(increaseAmount);

        private void OnDestroy()
        {
            if (!Application.isEditor) ResourceManager.Instance.ModifyMaxCapacity(-increaseAmount);
        }
    }
}
