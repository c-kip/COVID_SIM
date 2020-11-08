using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plane
{
    private Vector3 start;
    private Vector3 end;
    private Quaternion rot;
    private Image plane;
    private double speed;

    public Plane(Vector3 start, Vector3 end, Quaternion rot, Image plane, double speed)
    {
        this.start = start;
        this.end = end;
        this.rot = rot;
        this.plane = plane;
        this.speed = speed;
    }

    public void movePlane()
    {
        //If the plane reached it's destination, turn it around
        if (plane.transform.position == end)
        {
            Vector3 temp = start;
            start = end;
            end = temp;
            Vector3 vectToTarget = end - start;
            float angle = Mathf.Atan2(vectToTarget.y, vectToTarget.x) * Mathf.Rad2Deg + 30; //IDK i just guessed a bit and found 30 for the offset
            rot = Quaternion.AngleAxis(angle, Vector3.forward);
            plane.transform.rotation = rot; //Look from new start to end (flip direction)
        } else
        {
            plane.transform.position = Vector3.MoveTowards(plane.transform.position, end, (float)speed * Time.deltaTime);
        }
    }

    public Vector3 getStart()
    {
        return start;
    }

    public Vector3 getEnd()
    {
        return end;
    }

    public Quaternion getRot()
    {
        return rot;
    }

    public void setRot(Quaternion rot)
    {
        this.rot = rot;
    }

    public Image getPlane()
    {
        return plane;
    }
}
