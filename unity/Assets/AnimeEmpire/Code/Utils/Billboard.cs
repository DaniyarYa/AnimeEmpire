using UnityEngine;

namespace AnimeEmpire.Utils
{
    public class Billboard : MonoBehaviour
    {
        Camera _cam;

        void Awake() => _cam = Camera.main;

        void LateUpdate()
        {
            if (_cam == null) _cam = Camera.main;
            if (_cam == null) return;
            transform.rotation = _cam.transform.rotation;
        }
    }
}
