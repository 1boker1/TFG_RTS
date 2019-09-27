using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Prefabs.ClickArrow
{
    public class ClickArrow : MonoBehaviour
    {
        [SerializeField] private Animation animationComponent;
        [SerializeField] private List<GameObject> childObjects = new List<GameObject>();

        public IEnumerator PlayAnimation()
        {
            foreach (var child in childObjects) child.SetActive(true);

            animationComponent.Play();

            yield return new WaitForSeconds(animationComponent.clip.length);

            foreach (var child in childObjects) child.SetActive(false);
        }

        public void Enable(Vector3 position)
        {
            StopAllCoroutines();

            animationComponent.Stop();
            animationComponent.Rewind();

            transform.position = position;

            StartCoroutine(PlayAnimation());
        }
    }
}
