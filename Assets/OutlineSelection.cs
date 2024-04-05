using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLineSelection : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    void Update()
    {
        // Highlight
        if (highlight != null)
        {
            DisableHighlight(highlight);
            highlight = null;
        }

        // Use raycasting from the center of the VR camera
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        // Highlight hovered object
        if (Physics.Raycast(ray, out raycastHit))
        {
            Transform currentHighlight = raycastHit.transform;
            if (currentHighlight.CompareTag("Block") && currentHighlight != selection)
            {
                GameObject hitObject = currentHighlight.gameObject;
                if (hitObject.GetComponent<Outline>() != null)
                {
                    hitObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = hitObject.AddComponent<Outline>();
                    outline.enabled = true;
                    outline.OutlineColor = Color.magenta;
                    outline.OutlineWidth = 7.0f;
                }
                highlight = currentHighlight;

                // Highlight all objects in the group if the hit object is part of a group
                if (ObjectManipulator.parentOfGroup != null && hitObject.transform.IsChildOf(ObjectManipulator.parentOfGroup.transform))
                {
                    HighlightGroupedObjects();
                }
            }
            else
            {
                highlight = null;
            }
        }

        // This section remains for handling selection, which might involve different logic for grouped objects
    }

    void HighlightGroupedObjects()
    {
        foreach (Transform child in ObjectManipulator.parentOfGroup.transform)
        {
            Outline outline = child.gameObject.GetComponent<Outline>();
            if (outline == null)
            {
                outline = child.gameObject.AddComponent<Outline>();
            }
            outline.enabled = true;
            outline.OutlineColor = Color.magenta;
            outline.OutlineWidth = 7.0f;
        }
    }

    void DisableHighlight(Transform objectTransform)
    {
        Outline outline = objectTransform.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
        // Additionally, disable highlight for all objects in a group if applicable
        if (ObjectManipulator.parentOfGroup != null && objectTransform.IsChildOf(ObjectManipulator.parentOfGroup.transform))
        {
            foreach (Transform child in ObjectManipulator.parentOfGroup.transform)
            {
                outline = child.gameObject.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }
        }
    }
}
