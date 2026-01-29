using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowController : MonoBehaviour
{
    public void Load(SceneDefinition scene)
    {
        SceneManager.LoadScene(scene.sceneName);
    }
}