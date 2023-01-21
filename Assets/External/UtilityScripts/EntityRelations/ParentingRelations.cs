using System.Collections;
using UnityEngine;

namespace Assets.UtilityScripts.EntityRelations
{
    public class ParentingRelations : MonoBehaviour
    {
        public static bool IsObjectOrAncestorTarget(GameObject target, GameObject find)
        {
            if (find == null || target == null)
                return false;
            if (target == find)
                return true;
            else if (target != find && find.transform.parent != null)
                return IsObjectOrAncestorTarget(target, find.transform.parent.gameObject);
            else
                return false;
        }

        public static bool IsObjectOrDescendantTarget(GameObject target, GameObject find)
        {
            throw new System.NotImplementedException("This has not been implemented, you clown.");
        }
    }
}