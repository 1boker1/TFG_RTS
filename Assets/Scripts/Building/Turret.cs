using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Building
{
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public class Turret : MonoBehaviour, IGuardable
    {
        [SerializeField] private Building building;
        [SerializeField] private Button[] unitButton = new Button[6];

        public Unit.Unit enemy;

        public GameObject arrowPrefab;
        public GameObject destinationMarker;

        private Unit.Unit[] guardedUnits = new Unit.Unit[6];

        public float attackSpeed;
        public float radius;

        private float time = 0;

        public int damage;

        private void Awake()
        {
            GetComponent<SphereCollider>().radius = radius;
        }

        private void Update()
        {
            Attack();

            if (building.Selected && Input.GetMouseButtonDown(1)) SetGuardedUnitsDestination();

            destinationMarker.SetActive(building.Selected);
        }

        private void Attack()
        {
            if (enemy == null) return;

            time += Time.deltaTime;

            if (time >= attackSpeed)
            {
                SpawnArrow();

                enemy.GetComponent<IHealth>().ModifyHealth(damage, building);

                time = 0;
            }
        }

        private void SpawnArrow()
        {
            var arrowInstance = Instantiate(arrowPrefab);

            arrowInstance.transform.position = transform.position + new Vector3(0, 20, 0);
            arrowInstance.transform.LookAt(enemy.transform);

            arrowInstance.GetComponent<Arrow>().direction = (enemy.transform.position - arrowInstance.transform.position).normalized;
        }

        #region IGuardable

        public void GuardUnit(Unit.Unit unit)
        {
            for (int i = 0; i < guardedUnits.Length; i++)
            {
                if (guardedUnits[i] == null)
                {
                    guardedUnits[i] = unit;
                    guardedUnits[i].Deselect();
                    guardedUnits[i].gameObject.SetActive(false);

                    unitButton[i].image.sprite = unit.unitData.Image;
                    unitButton[i].gameObject.SetActive(true);

                    break;
                }
            }
        }

        public void RestoreUnit(int index)
        {
            if (guardedUnits[index] != null)
            {
                guardedUnits[index].gameObject.SetActive(true);
                guardedUnits[index].Target = null;
                guardedUnits[index].Select(building.Team);
                guardedUnits[index] = null;

                unitButton[index].image.sprite = null;
                unitButton[index].gameObject.SetActive(false);

                if (destinationMarker.activeSelf)
                {
                    SelectionManager.Instance.Group(destinationMarker.transform.position, SelectionManager.SelectedUnits);
                }

                ArrangeButtons(index);
            }
        }

        public void RestoreAllUnits()
        {
            for (int index = 0; index < guardedUnits.Length; index++)
            {
                if (guardedUnits[index] == null) continue;

                guardedUnits[index].gameObject.SetActive(true);
                guardedUnits[index].Target = null;
                guardedUnits[index].Select(building.Team);
                guardedUnits[index] = null;

                unitButton[index].image.sprite = null;
                unitButton[index].gameObject.SetActive(false);
            }

            if (destinationMarker.activeSelf && SelectionManager.SelectedUnits.Count > 0)
                SelectionManager.Instance.Group(destinationMarker.transform.position, SelectionManager.SelectedUnits);
        }

        public void SetGuardedUnitsDestination()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 9000))
            {
                if (hit.transform.CompareTag("Floor"))
                {
                    destinationMarker.transform.position = hit.point;
                    destinationMarker.SetActive(true);
                    destinationMarker.GetComponent<PathRenderer>().SetPositions(transform.position);
                }
                else if (hit.transform.gameObject == this.gameObject)
                {
                    destinationMarker.transform.localPosition = Vector3.zero;
                    destinationMarker.SetActive(false);
                    destinationMarker.GetComponent<PathRenderer>().SetPositions(transform.position);
                }
            }
        }

        #endregion

        private void ArrangeButtons(int emptyIndex)
        {
            for (int i = emptyIndex; i < guardedUnits.Length; i++)
            {
                if (i + 1 >= guardedUnits.Length || guardedUnits[i + 1] == null) continue;

                guardedUnits[i] = guardedUnits[i + 1];
                guardedUnits[i + 1] = null;

                unitButton[i].image.sprite = guardedUnits[i].unitData.Image;
                unitButton[i].gameObject.SetActive(true);
            }

            for (int i = guardedUnits.Length - 1; i >= 0; i--)
            {
                if (guardedUnits[i] != null) continue;

                unitButton[i].image.sprite = null;
                unitButton[i].gameObject.SetActive(false);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (enemy != null) return;

            enemy = other.GetComponent<ISelectable>() as Unit.Unit;

            if (enemy != null)
                enemy = enemy.Team == building.Team ? null : enemy;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform == enemy) { enemy = null; }
        }
    }
}
