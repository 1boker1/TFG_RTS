using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.ObstacleAvoidance
{
    public static class ObstacleAvoidance
    {
        public static void FindClearDestination(Unit.Unit unit, ref Vector3 destination, NavMeshAgent navMeshAgent)
        {
            var target = unit.Target.transform.GetComponent<BoxCollider>();

            if (target == null) return;

            var min = new Vector3(target.bounds.min.x, 0, target.bounds.min.z);
            var max = new Vector3(target.bounds.max.x, 0, target.bounds.max.z);
            var maxZ = new Vector3(target.bounds.min.x, 0, target.bounds.max.z);
            var minZ = new Vector3(target.bounds.max.x, 0, target.bounds.min.z);

            var points = new List<Vector3>
            {
                Utils.ReturnLinePoint(min, maxZ),
                Utils.ReturnLinePoint(min, minZ),
                Utils.ReturnLinePoint(max, maxZ),
                Utils.ReturnLinePoint(max, minZ)
            };

            var final = destination;

            foreach (var point in points)
            {
                if (!DestinationBlocked(unit.transform, point, unit.Target.transform))
                {
                    final = point;

                    break;
                }
            }

            destination = target.ClosestPoint(new Vector3(final.x, destination.y, final.z));

            navMeshAgent.SetDestination(destination);
        }

        public static bool DestinationBlocked(Transform transform, Vector3 point, Transform target)
        {
            foreach (var collider in Physics.OverlapSphere(point, 1.5f))
            {
                if (collider.transform != transform &&
                    collider.transform != target &&
                    collider.GetComponent<Unit.Unit>())
                {
                    return true;
                }
            }

            return false;
        }
    }
}