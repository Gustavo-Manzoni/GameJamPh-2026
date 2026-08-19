using UnityEngine;
using System.Collections;

/// <summary>
/// checks whether a transform is past the camera bounds
/// and tells map to move the camera
/// </summary>
public class BoundsChecker : MonoBehaviour
{

    /// <summary>
    /// amount to take or add from camera edges when checking against m_Check position
    /// EG.
    /// when checking the camera left
    /// the check position will need to be this value more to the left then that value
    /// </summary>
    public float padding = 0.2f;

    /// <summary>
    /// reference to transform that will be moving to the end of the bounds
    /// </summary>
    public Transform m_Check;

    /// <summary>
    /// reference to the camera bounds
    /// </summary>
    private Map m_Map;

    // Use this for initialization
    void Start()
    {
        m_Check = ServiceLocator.Get<PlayerController>().transform;
        m_Map = GetComponent<Map>();
    }

    // Update is called once per frame
    void Update()
    {
        //right
        //if x is larger
        if (m_Check.position.x > m_Map.cameraBounds.max.x + padding)
        {
            outOfBounds(Map.DIRECTIONS.RIGHT);
            return;
        }

        //left
        //if x is smaller
        if (m_Check.position.x < m_Map.cameraBounds.min.x - padding)
        {
            outOfBounds(Map.DIRECTIONS.LEFT);
            return;
        }

        //up
        //if y is larger
        if (m_Check.position.y > m_Map.cameraBounds.max.y + padding)
        {
            outOfBounds(Map.DIRECTIONS.UP);
            return;
        }

        //down
        //if y is smaller
        if (m_Check.position.y < m_Map.cameraBounds.min.y - padding)
        {
            outOfBounds(Map.DIRECTIONS.DOWN);
            return;
        }
    }

    /// <summary>
    /// just tells map which direction to move
    /// </summary>
    /// <param name="a_Dir">Direction to move camera</param>
    private void outOfBounds(Map.DIRECTIONS a_Dir)
    {
        m_Map.hitEdge(a_Dir);
    }


}
