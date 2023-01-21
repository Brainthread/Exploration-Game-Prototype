using System.Collections;
using UnityEngine;
using Assets.UtilityScripts.EntityRelations;

namespace Assets.UtilityScripts.AI
{
    public class DetectionUtility3D
    {
        /* Evaluates whether or not an point is inside a cone around a defined vector
         * 
         * Parameters:
         * -    origin: the cone origin position
         * -    forward: the vector around which to orient the vector
         * -    target: the point to be checked
         * -    conedegrees: the angle of the outer circle of the cone relative to the forward vector
         * -    distance: the cone length
         * Return values:
         * -    true if the the target position is inside the cone, else false
         */
        public static bool TargetInVisionCone(Vector3 origin, Vector3 forward, Vector3 target, float conedegrees, float distance)
        {
            if (target == null)
                return false;

            if (!TargetIsInRange(origin, target, distance))
                return false;

            Vector3 _vectorToTarget = target - origin;

            float dot = -(Vector3.Dot(_vectorToTarget.normalized, forward) - 1) * 90;
            if (dot > conedegrees)
                return false;

            return true;
        }

        /* Evaluates whether or not a position is within a certain distance from another
         * 
         * Parameters:
         * -    origin: the origin position
         * -    target: the ending position
         * -    distance: the maximum distance between the points
         * Return values:
         * -    true if the target position is closer than [distance] units of length, else false
         */
        public static bool TargetIsInRange(Vector3 origin, Vector3 target, float distance)
        {
            Vector3 _vectorToTarget = target - origin;
            if (Vector3.Magnitude(_vectorToTarget) >= distance)
                return false;

            return true;
        }

        /* Evaluates whether or not line of sight can be established from origin to targetObject. 
         * If an object other than the targetObject intersects the line, the method returns false. 
         * 
         * Parameters:
         * -    origin: the starting position of the line
         * -    targetObject: the target object
         * -    hitMask: a layermask of layers whose gameobjects should be able to intersect the line.
         * -    mustFindCollider: specifies whether or not the line HAS to hit the targetobject to evaluate as true. 
         * Return values:
         * -    true if the line collides with the targetObject or its children, true if it collides with nothing and mustFindCollider is false.
         *          False if the line collides with any other object, or if the line intersects with nothing, and mustFindCollider is true.
         */
        public static bool TargetInLineOfSight(Vector3 origin, GameObject targetObject, LayerMask hitMask, bool mustFindCollider)
        {
            if (targetObject == null)
                return false;

            RaycastHit hit;
            if (Physics.Linecast(origin, targetObject.transform.position, out hit, hitMask))
            {
                
                return ParentingRelations.IsObjectOrAncestorTarget(targetObject, hit.collider.gameObject);
            }
            return !mustFindCollider;
        }

        /* Evaluates whether or not a line going from one point to another is obstructed
         * 
         * Parameters:
         * -    origin: the origin position
         * -    target: the ending position
         * -    hitMask: the layermask of layers whose gameobjects should be able to intersect the line
         * Return values:
         * -    true if nothing intersects the line, else false
         */
        public static bool PositionInLineOfSight(Vector3 origin, Vector3 position, LayerMask hitMask)
        {
            RaycastHit hit;
            if (Physics.Linecast(origin, position, out hit, hitMask))
            {
                return false;
            }
            return true;
        }

        /* Evaluates whether or not an object can be seen within a certain cone of vision, and is not obstructed by other objects
         * 
         * Parameters:
         * -    origin: the origin position
         * -    forward: the vector around which to orient the cone
         * -    targetObject: the target gameobject
         * -    conedegrees: the angle of the outer circle of the cone relative to the forward vector
         * -    distance: the maximum distance between the points
         * -    targetObject: the target object
         * -    hitMask: a layermask of layers whose gameobjects should be able to intersect the line.
         * -    [optional] mustFindCollider: specifies whether or not the line HAS to hit the targetobject to evaluate as true. Defaults as true.
         * Return values:
         * -    true if both the TargetInVisionCone and TargetInLineOfSight methods return true, else false
         */
        public static bool CanSeeTarget(Vector3 origin, Vector3 forward, GameObject targetObject, float conedegrees, float distance, LayerMask hitMask, bool mustFindCollider = true)
        {
            if (targetObject == null)
                return false;
            return TargetInVisionCone(origin, forward, targetObject.transform.position, conedegrees, distance)
                && TargetInLineOfSight(origin, targetObject, hitMask, mustFindCollider);
        }
    }
}