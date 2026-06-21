using System.Collections;
using AnimeEmpire.Core;
using TMPro;
using UnityEngine;

namespace AnimeEmpire.UI
{
    public class BootController : MonoBehaviour
    {
        [SerializeField] TMP_Text _statusLabel;

        IEnumerator Start()
        {
            if (_statusLabel != null) _statusLabel.text = "Загрузка...";
            yield return null;
            yield return new WaitForSeconds(0.5f);
            if (_statusLabel != null) _statusLabel.text = "Запуск мира...";
            yield return new WaitForSeconds(0.3f);
            if (SceneRouter.Instance != null) SceneRouter.Instance.Replace(SceneRouter.SceneWorld);
            else UnityEngine.SceneManagement.SceneManager.LoadScene(SceneRouter.SceneWorld);
        }
    }
}
