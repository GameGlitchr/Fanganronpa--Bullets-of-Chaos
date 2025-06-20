using UnityEngine;
using UnityEngine.EventSystems;

public class UIBlockerDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log("UI Elements under mouse:");
            foreach (var result in results)
                Debug.Log(result.gameObject.name);
        }
    }
}
