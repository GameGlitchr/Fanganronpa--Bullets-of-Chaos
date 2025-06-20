using UnityEngine;

public class TabletController : MonoBehaviour
{
    public Transform tabletViewPosition;
    public Transform tabletRestPosition;
    public float moveSpeed = 5f;

    private Transform tabletTransform;
    private bool isTabletOpen = false;

    void Start()
    {
        tabletTransform = this.transform;
        // Immediately move to rest position
        MoveToPosition(tabletRestPosition);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTabletOpen = !isTabletOpen;
        }

        Transform target = isTabletOpen ? tabletViewPosition : tabletRestPosition;
        MoveToPosition(target);
    }

    private void MoveToPosition(Transform target)
    {
        tabletTransform.position = Vector3.Lerp(tabletTransform.position, target.position, Time.deltaTime * moveSpeed);
        tabletTransform.rotation = Quaternion.Lerp(tabletTransform.rotation, target.rotation, Time.deltaTime * moveSpeed);
    }
}
