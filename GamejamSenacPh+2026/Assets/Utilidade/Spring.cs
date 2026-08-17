using UnityEngine;


[System.Serializable]
public class SpringFloat
{
    public float value;
    public float velocity;
    public float stiffness = 200f;
    public float damping = 15f;

    public SpringFloat(float initial = 0f)
    {
        value = initial;
        velocity = 0f;
    }

    public float Update(float target, float dt)
    {
        float force = (target - value) * stiffness - velocity * damping;
        velocity += force * dt;
        value += velocity * dt;
        return value;
    }

    public void SetInstant(float v)
    {
        value = v;
        velocity = 0f;
    }
}


[System.Serializable]
public class SpringVector2
{
    public Vector2 value;
    public Vector2 velocity;
    public float stiffness = 200f;
    public float damping = 15f;

    public SpringVector2(Vector2 initial = default)
    {
        value = initial;
        velocity = Vector2.zero;
    }

    public Vector2 Update(Vector2 target, float dt)
    {
        Vector2 force = (target - value) * stiffness - velocity * damping;
        velocity += force * dt;
        value += velocity * dt;
        return value;
    }

    public void SetInstant(Vector2 v)
    {
        value = v;
        velocity = Vector2.zero;
    }
}
