using UnityEngine;
using UnityEngine.EventSystems;

public class DraggingControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum DragDirection
    {
        Left, Right, Up, Down
    }

    public RectTransform prevPointerDrag;
    public RectTransform pointerDrag;
    public bool showUI;

    DragDirection dragDirection;

    bool isDragging = false;
    Vector3 prevPos;
    Vector3 curPos;
    float dragDelta;

    public void SetDragDirection(DragDirection dragDirection)
    {
        this.dragDirection = dragDirection;
    }

    private void Awake()
    {
        prevPointerDrag.gameObject.SetActive(false);
        pointerDrag.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDragging)
        {
            prevPointerDrag.position = pointerDrag.position;
            prevPos = prevPointerDrag.anchoredPosition;

            pointerDrag.position = Input.mousePosition;
            curPos = pointerDrag.anchoredPosition;

            EvaluateDragDelta();
        }
    }

    private void EvaluateDragDelta()
    {
        switch (dragDirection)
        {
            case DragDirection.Left:
                dragDelta = Mathf.Max(0f, curPos.x - prevPos.x);
                break;
            case DragDirection.Right:
                dragDelta = Mathf.Max(0f, prevPos.x - curPos.x);
                break;
            case DragDirection.Up:
                dragDelta = Mathf.Max(0f, curPos.y - prevPos.y);
                break;
            case DragDirection.Down:
                dragDelta = Mathf.Max(0f, prevPos.y - curPos.y);
                break;
        }
    }

    public float GetDragDelta()
    {
        return dragDelta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        prevPointerDrag.position = pointerDrag.position = Input.mousePosition;

        if (showUI)
        {
            prevPointerDrag.gameObject.SetActive(true);
            pointerDrag.gameObject.SetActive(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (showUI)
        {
            prevPointerDrag.gameObject.SetActive(false);
            pointerDrag.gameObject.SetActive(false);
        }
    }
}
