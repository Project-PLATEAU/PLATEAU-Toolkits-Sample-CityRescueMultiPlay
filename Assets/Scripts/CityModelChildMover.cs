using UnityEngine;
using UnityEngine.SceneManagement;
using PLATEAU.CityInfo;

public class CityModelChildMover : MonoBehaviour
{
    void Start()
    {
        MoveToTargetNode();
    }

    private void MoveToTargetNode()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded)
                continue;

            foreach (GameObject rootObj in scene.GetRootGameObjects())
            {
                PLATEAUInstancedCityModel plateauModel = rootObj.GetComponentInChildren<PLATEAUInstancedCityModel>();
                if (plateauModel != null)
                {
                    Transform targetParent = plateauModel.transform;
                    transform.SetParent(targetParent, true);
                    break;
                }
            }
        }
    }
}