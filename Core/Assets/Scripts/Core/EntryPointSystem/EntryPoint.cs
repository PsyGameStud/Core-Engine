using UnityEngine;

namespace Core.EntryPointSystem
{
    [DefaultExecutionOrder(0)]
    public abstract class EntryPoint : MonoBehaviour
    {
        protected void Awake()
        {
            Setup();
        }

        protected abstract void Setup();
    }
}