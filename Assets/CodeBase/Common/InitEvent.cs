using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    public class InitEvent : MonoBehaviour
    {
        public UnityEvent OnClick;

        private void OnMouseDown()
        {
            OnClick?.Invoke();
        }
    }
}