using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    
    private Transform selectedCharacter;
    private bool characterSelected;

    private List<Node> path = new List<Node>();

    private GridScript gridScript;
    private Pathfinding pathFinder;
    // Start is called before the first frame update
    void Start()
    {
        gridScript = FindObjectOfType<GridScript>();
        pathFinder = FindObjectOfType<Pathfinding>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit)
            {
                //Debug.Log("Has Hit");
                if (hit.transform.tag == "Tile")
                {
                    if (!characterSelected)
                    {
                        return;
                    }
                    
                    Vector2Int targetCords = hit.transform.GetComponent<Tile>().cords;
                    bool isBlocked = hit.transform.GetComponent<Tile>().blocked;
                    
                    if (isBlocked)
                    {
                        Debug.Log("Nuh uh");
                        return;
                    }
                    
                    //Debug.Log(targetCords.x+", "+targetCords.y);
                    Vector2Int startCords = new Vector2Int((int)selectedCharacter.transform.position.x,
                        (int)selectedCharacter.transform.position.z);
                    pathFinder.SetNewDestination(startCords,targetCords);
                    RecalculatePath(true);
                }

                if (hit.transform.tag == "Character")
                {
                    if (selectedCharacter == hit.transform)
                    {
                        selectedCharacter = null;
                        characterSelected = false;
                        Debug.Log("Deselect");
                    }
                    else
                    {
                        selectedCharacter = hit.transform;
                        characterSelected = true;
                    }
                }
            }
        }
    }

    private void RecalculatePath(bool resetPath)
    {
        Vector2Int coordinates = new Vector2Int();
        if (resetPath)
        {
            coordinates = pathFinder.StartCords;
        }
        else
        {
            coordinates = gridScript.GetCoordinatesFromPosition(transform.position);
        }
        StopAllCoroutines();
        path.Clear();
        path = pathFinder.GetNewPath(coordinates);
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 startPosition = selectedCharacter.position;
            Vector3 endPosition = gridScript.GetPositionFromCoordinates(path[i].cords);
            float travelPercent = 0f;
            
            Vector3 moveDirection = -(endPosition - startPosition).normalized;
            moveDirection.y = 0;

            if (Mathf.Abs(moveDirection.x) > 0.1f)
            {
                bool movingLeft = moveDirection.x < 0;
            
                Vector3 currentScale = selectedCharacter.localScale;
            
                currentScale.x = movingLeft ? -Mathf.Abs(currentScale.x) : Mathf.Abs(currentScale.x);
            
                selectedCharacter.localScale = currentScale;
            }

            while (travelPercent < 1f)
            {
                travelPercent += Time.deltaTime * movementSpeed;
                selectedCharacter.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
            }
        }
        selectedCharacter = null;
        characterSelected = false;
    }
}
