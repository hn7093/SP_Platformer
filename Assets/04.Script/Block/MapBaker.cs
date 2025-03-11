using Unity.AI.Navigation;
using UnityEngine;
public class MapBaker : MonoBehaviour
{
    public NavMeshSurface[] surfaces;
    public GameObject[] gameObjects;
    public void ReBake(bool hide)
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(!hide);
        }
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }
}
