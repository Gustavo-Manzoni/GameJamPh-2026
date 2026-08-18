using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// works out bounds of camera
/// and updates camera transform with a direction of movement called from another script
/// </summary>
public class CameraTest : MonoBehaviour
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
    /// speed of the movement
    /// </summary>
    public float m_CameraSpeed = 5.0f;
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
    /// how far the camera has moved since it was asked
    /// </summary>
    private float m_CameraMovedAmount = 0.0f;

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
            //how far the camera should move
            float dist = m_CameraSpeed * Time.unscaledDeltaTime;
            //what direction the camera should move
            Vector3 direction = getDirectionFromDirection(m_CameraDirection);

            //total distance/direction the camera is moving
            direction *= dist;
            //move camera
            Camera.main.transform.Translate(direction);
            //add
            m_CameraMovedAmount += dist;

            //if camera movement is left or right
            if ((int)m_CameraDirection <= 1)
            {
                //check to see if we have moved the camera far enough
                if (m_CameraMovedAmount > Mathf.Abs(m_CameraSize.x))
                {
                    //reset it
                    resetCameraMoving();
                }
            }
            else
            { // camera movement is up or down
                if (m_CameraMovedAmount > Mathf.Abs(m_CameraSize.y))
                {
                    resetCameraMoving();
                }
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
    /// works out difference between how far the camera has overshot the position we want
    /// this is good for minor fixes are slow speeds(not common), and makes quick speeds possible (very common)
    /// </summary>
    private void resetCameraMoving()
    {

        //get opposite direction since we want to move the way we came
        Vector3 direction = getDirectionFromDirection(m_CameraDirection);
        direction *= -1; //flip direction

        Vector3 cameraSize = m_CameraSize;

        //remove the other directions data
        // EG. if we move left, this will remove up/down's data from cameraSize
        if ((int)m_CameraDirection <= 1)
        {
            cameraSize.y = 0;
        }
        else
        {
            cameraSize.x = 0;
        }

        //get how far we should move
        Vector3 offset;
        if (m_CameraDirection == DIRECTIONS.RIGHT || m_CameraDirection == DIRECTIONS.UP)
        {
            offset = (direction * m_CameraMovedAmount) + cameraSize;
        }
        else
        {
            offset = (direction * m_CameraMovedAmount) - cameraSize;
        }

        //final location of camera
        Vector3 camPos = Camera.main.transform.position + offset;

        //round camera position off
        //this will make the camera have 12.5 instead of 12.5201510292
        camPos.x = (float)System.Math.Round(camPos.x, 1);
        camPos.y = (float)System.Math.Round(camPos.y, 1);

        //set camera pos
        Camera.main.transform.position = camPos;

        //reset set script
        m_CameraMoving = false;
        m_CameraMovedAmount = 0.0f;

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
