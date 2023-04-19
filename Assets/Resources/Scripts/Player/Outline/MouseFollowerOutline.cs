using UnityEngine;

public class MouseFollowerOutline : MonoBehaviour
{
    [SerializeField] private CheckingAreaUtil _checkMinDistanceItem;
    [SerializeField] private CheckingLineCast _checkGround;
    [SerializeField] private float _maxDistance;

    public GameObject CurrentOutlinedItem;

    private void Update()
    {
        if (GameManager.Instance.Player != null)
        {
            CreateOutline();
        }
    }

    private void CreateOutline()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPosition = GameManager.Instance.Player.transform.position;
        Vector2 center = transform.position;
        float mouseDistance = Mathf.Abs(playerPosition.x - center.x);
        if (mouseDistance <= _maxDistance)
        {
            var checkingResult = _checkMinDistanceItem.CheckWithMinDistance(center, transform.gameObject, center, _maxDistance);
            if (checkingResult.Item1)
            {
                Vector3 itemPosition = checkingResult.Item2.transform.position;
                float distance = Mathf.Abs(playerPosition.x - itemPosition.x);
                var checkingResult2 = _checkGround.CheckLinecast(playerPosition, itemPosition);
                if (distance <= _maxDistance && !checkingResult2.Item1)
                {
                    if (CurrentOutlinedItem == null)
                    {
                        CurrentOutlinedItem = checkingResult.Item2.gameObject;
                        CurrentOutlinedItem.GetComponent<OutlineController>().SetOutlineMaterial();
                    }
                    if (CurrentOutlinedItem != checkingResult.Item2.gameObject)
                    {
                        CurrentOutlinedItem.GetComponent<OutlineController>().SetDefaultMaterial();
                        CurrentOutlinedItem = checkingResult.Item2.gameObject;
                        CurrentOutlinedItem.GetComponent<OutlineController>().SetOutlineMaterial();
                    }
                }
                else
                {
                    DisableCurrentItemOutline();
                }
            }
            else
            {
                DisableCurrentItemOutline();
            }
        }
        else
        {
            DisableCurrentItemOutline();
        }
    }

    private void DisableCurrentItemOutline()
    {
        if (CurrentOutlinedItem != null)
        {
            CurrentOutlinedItem.GetComponent<OutlineController>().SetDefaultMaterial();
            CurrentOutlinedItem = null;
        }
    }
}
