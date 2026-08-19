using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// works out bounds of camera
/// and updates camera transform with a direction of movement called from another script
/// </summary>
public class Map : MonoBehaviour
{

    /// <summary>
    /// enum of possible directions of camera movement
    /// </summary>
    public enum DIRECTIONS
    {
        LEFT, RIGHT, UP, DOWN
    }

    /// <summary>
    /// bounds of the camera in world space
    /// </summary>
    private Bounds m_CameraBounds;
    /// <summary>
    /// property for m_CameraBounds so other classes can get the information
    /// </summary>
    public Bounds cameraBounds { get { return m_CameraBounds; } }

    /// <summary>
    /// storage of width and height of bounds for camera
    /// from cameraBounds.min.x to cameraBounds.max.x
    /// </summary>
    private Vector3 m_CameraSize = Vector3.zero;

    /// <summary>
    /// how sharp/snappy the lerp towards the target position is (higher = faster)
    /// </summary>
    public float m_CameraSpeed = 8.0f;
    /// <summary>
    /// flag for if the camera is moving
    /// if this is true the movement code runs
    /// </summary>
    private bool m_CameraMoving = false;
    /// <summary>
    /// which direction to move
    /// </summary>
    private DIRECTIONS m_CameraDirection = DIRECTIONS.LEFT;
    /// <summary>
    /// world position the camera is lerping towards
    /// </summary>
    private Vector3 m_CameraTargetPosition = Vector3.zero;

    /// <summary>
    /// The player has hit the edge of the screen
    /// this will set up this script to start moving the camera
    /// </summary>
    /// <param name="a_Dir">Which direction to move</param>
    public void hitEdge(DIRECTIONS a_Dir)
    {
        //if it's already moving then it's probably a mistake
        if (m_CameraMoving)
        {
            return;
        }
        //set up script
        Time.timeScale = 0.0f;//stop player/AI from moving
        m_CameraDirection = a_Dir;
        m_CameraMoving = true;

        //work out the exact screen-sized offset we want to end up at
        Vector3 direction = getDirectionFromDirection(a_Dir);
        Vector3 moveAmount = (int)a_Dir <= 1 ? new Vector3(m_CameraSize.x, 0f, 0f) : new Vector3(0f, m_CameraSize.y, 0f);
        m_CameraTargetPosition = Camera.main.transform.position + Vector3.Scale(direction, moveAmount);

        //round target off
        //this will make the camera have 12.5 instead of 12.5201510292
        m_CameraTargetPosition.x = (float)System.Math.Round(m_CameraTargetPosition.x, 1);
        m_CameraTargetPosition.y = (float)System.Math.Round(m_CameraTargetPosition.y, 1);
    }

    /// <summary>
    /// setting up the script by getting bounds of camera at the start
    /// </summary>
    void Start()
    {
        updateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        //should the camera be moving
        if (m_CameraMoving)
        {
            Transform camTransform = Camera.main.transform;

            //framerate-independent exponential smoothing towards the target, feels a lot softer than a fixed-speed translate
            float t = 1f - Mathf.Exp(-m_CameraSpeed * Time.unscaledDeltaTime);
            camTransform.position = Vector3.Lerp(camTransform.position, m_CameraTargetPosition, t);

            //close enough to the target, snap and finish
            if ((camTransform.position - m_CameraTargetPosition).sqrMagnitude < 0.0004f)
            {
                resetCameraMoving();
            }
        }
    }

    /// <summary>
    /// grab the bounds of a orthographic camera
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    private Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    /// <summary>
    /// get bounds of main camera and update camera size
    /// </summary>
    public void updateBounds()
    {
        m_CameraBounds = OrthographicBounds(Camera.main);
        m_CameraSize = cameraBounds.max - cameraBounds.min;
    }

    /// <summary>
    /// snaps the camera to the target position and re-enables player/AI movement
    /// </summary>
    private void resetCameraMoving()
    {
        //snap to the exact target so we don't drift due to the lerp threshold
        Camera.main.transform.position = m_CameraTargetPosition;

        //reset set script
        m_CameraMoving = false;

        updateBounds();

        //allow the player to move again
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// turns DIRECTIONS enum into a unit vector with the direction of the enum
    /// </summary>
    /// <param name="a_Dir">direction to transfer into a vector</param>
    /// <returns>vector of a_Dir</returns>
    public Vector2 getDirectionFromDirection(DIRECTIONS a_Dir)
    {
        Vector3 direction = Vector2.zero;
        switch (a_Dir)
        {
            case DIRECTIONS.LEFT:
                direction.x = -1;
                break;
            case DIRECTIONS.RIGHT:
                direction.x = 1;
                break;
            case DIRECTIONS.DOWN:
                direction.y = -1;
                break;
            case DIRECTIONS.UP:
                direction.y = 1;
                break;
        }
        return direction;
    }



}
