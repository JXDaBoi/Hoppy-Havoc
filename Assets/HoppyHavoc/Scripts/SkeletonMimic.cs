using UnityEngine;

public class SkeletonMimic : MonoBehaviour
{
    public Transform mimickeeRoot;
    public Transform mimickerRoot;

    void Start()
    {
        if (mimickeeRoot == null || mimickerRoot == null)
        {
            Debug.LogError("Please assign both mimickee and mimicker root transforms.");
            return;
        }
    }

    void Update()
    {
        if (mimickeeRoot != null && mimickerRoot != null)
        {
            MimicTransforms(mimickeeRoot, mimickerRoot);
        }
    }

    void MimicTransforms(Transform source, Transform target)
    {
        foreach (Transform sourceChild in source)
        {
            Transform targetChild = FindChildByName(target, sourceChild.name);
            if (targetChild != null)
            {
                // Copy local transform properties
                targetChild.localPosition = sourceChild.localPosition;
                targetChild.localRotation = sourceChild.localRotation;
                targetChild.localScale = sourceChild.localScale;

                // Recursively mimic child transforms
                MimicTransforms(sourceChild, targetChild);
            }
        }
    }

    Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }

            Transform result = FindChildByName(child, name);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}