using System.Collections.Generic;
using UnityEngine;

public class PlanesManager : MonoBehaviour {

    private Dictionary<Vector2, bool> planes;
    
    public void initPlanes()
    {
        planes = new Dictionary<Vector2, bool>();
        for(int x = 0; x < Utilities.fieldSize; x++)
        {
            for(int y = 0; y < Utilities.fieldSize; y++)
            {
                planes.Add(new Vector2(Utilities.scaleUnit * x, Utilities.scaleUnit * y), true);
            }
        }
    }
    public bool isFree(Vector2 position)
    {
        if (planes.ContainsKey(position))
        {
            return planes[position];
        }
        else
        {
            return false;
        }
    }
    public void setFree(Vector2 position, bool freeStatus)
    {
        if (planes.ContainsKey(position))
        {
            planes[position] = freeStatus;
        }
    }

}
