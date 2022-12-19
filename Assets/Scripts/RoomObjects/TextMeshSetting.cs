using System;
using UnityEngine;

namespace RoomObjects
{
    public class TextMeshSetting : MonoBehaviour
    {
        private Renderer _renderer;

        [ContextMenu("SetMaxRenderPriority")]
        private void SetMaxRenderPriority(){
            _renderer = GetComponent<Renderer>();
            _renderer.sortingOrder = Int32.MinValue;
            _renderer.sortingLayerName = "Effect";
        }
    }
}
