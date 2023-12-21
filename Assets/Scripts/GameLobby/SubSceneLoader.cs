using UnityEngine;
using UnityEngine.SceneManagement;

namespace LobbyRelaySample.ngo
{
    public class SubSceneLoader : MonoBehaviour
    {
        public string[] m_ScenesToLoad;

        public void LoadSubScenes()
        {
            foreach (string sceneName in m_ScenesToLoad)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }
    }
}