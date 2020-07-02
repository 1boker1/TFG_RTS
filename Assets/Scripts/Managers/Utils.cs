using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Resources;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Managers
{
    public static class Utils
    {
        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static T GetFromRay<T>()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.GetComponent<T>() != null)
                {
                    return hit.transform.GetComponent<T>();
                }
            }
            Vector3 a = Vector3.back;

            return default;
        }

        public static T GetFromRay<T>(string layerMaskName)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int layer_mask = LayerMask.GetMask(layerMaskName);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
            {
                if (hit.transform.GetComponent<T>() != null)
                {
                    return hit.transform.GetComponent<T>();
                }
            }

            return default;
        }

        public static T GetFromRay<T>(LayerMask layerMask)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.GetComponent<T>() != null)
                {
                    return hit.transform.GetComponent<T>();
                }
            }

            return default;
        }

        public static bool InRange(ISelectable first, ISelectable second, float range)
        {
            return ((ClosestPoint(first, second) - ClosestPoint(second, first)).magnitude < range);
        }

        public static bool InRange(Vector3 origin, Vector3 destination, float range)
        {
            return ((destination - origin).magnitude < range);
        }

        public static Vector3 ClosestPoint(ISelectable first, ISelectable second)
        {
            return second.transform.GetComponent<Collider>().ClosestPoint(first.transform.position);
        }

        public static void LookTarget(ISelectable first, ISelectable second)
        {
            first.transform.LookAt(second.transform.position.With(y: first.transform.position.y));
        }

        public static Rect DragRectangle(Vector3 startClick)
        {
            var minX = startClick.x;
            var minY = MathExtension.InvertMouseY(startClick.y);
            var maxX = Input.mousePosition.x - startClick.x;
            var maxY = MathExtension.InvertMouseY(Input.mousePosition.y) - MathExtension.InvertMouseY(startClick.y);

            var rectangle = new Rect(minX, minY, maxX, maxY);

            if (rectangle.width < 0)
            {
                rectangle.x += rectangle.width;
                rectangle.width = -rectangle.width;
            }

            if (rectangle.height < 0)
            {
                rectangle.y += rectangle.height;
                rectangle.height = -rectangle.height;
            }

            return rectangle;
        }

        public static Vector3 CenterOfMass(BindingList<ISelectable> Entities)
        {
			if(Entities.Count==0)
				return Vector3.zero;

            Vector3 centerOfMass = new Vector3();

            foreach (var unit in Entities)
            {
                centerOfMass += unit.transform.position;
            }

            return centerOfMass / (Entities.Count);
        }

        public static List<Vector3> GetPositions(List<Unit.Unit> units, Vector3 worldPoint, Vector3 forwardVector, Vector3 perpendicularVector)
        {
            List<Vector3> positions = new List<Vector3>();

            if (units.Count <= 5)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    positions.Add(worldPoint - (perpendicularVector * units.Count * 1.375f) + (perpendicularVector * i * 3f));
                }
            }
            else
            {
                int num = (units.Count / 10);
                float dividend = (10 - (float)(units.Count - (10 * num)));

                for (int x = 0; x < num; x++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector3 position = (perpendicularVector * 13.75f) +
                                           (perpendicularVector * i * 3) -
                                           (forwardVector * x * 4);

                        positions.Add(worldPoint - position);
                    }
                }

                for (int i = 0; i < units.Count - (10 * num); i++)
                {
                    Vector3 position = (perpendicularVector * (10 - dividend) * 1.375f) +
                                       (perpendicularVector * i * 3f) -
                                       (forwardVector * (num) * 4);

                    positions.Add(worldPoint - position);
                }
            }

            return positions;
        }

        public static bool HaveEnoughResources([CanBeNull] Wood wood, [CanBeNull] Food food, [CanBeNull] Gold gold, [CanBeNull] Rock rock)
        {
            if (wood != null && !HaveEnoughResources(wood)) return false;
            if (food != null && !HaveEnoughResources(food)) return false;
            if (gold != null && !HaveEnoughResources(gold)) return false;
            if (rock != null && !HaveEnoughResources(rock)) return false;

            ResourceManager.Instance.AddResource(typeof(Wood), -wood?.amount ?? 0);
            ResourceManager.Instance.AddResource(typeof(Food), -gold?.amount ?? 0);
            ResourceManager.Instance.AddResource(typeof(Rock), -food?.amount ?? 0);
            ResourceManager.Instance.AddResource(typeof(Gold), -food?.amount ?? 0);

            return true;
        }

        public static bool HaveEnoughResources(ResourceType resource)
        {
            return !(resource.amount > ResourceManager.Instance.GetResourceAmount(resource.GetType()));
        }

        public static Vector3 ReturnLinePoint(Vector3 first, Vector3 second)
        {
            var vector = second - first;

            return first + (vector * Random.Range(0f, 1f));
        }
    }
}