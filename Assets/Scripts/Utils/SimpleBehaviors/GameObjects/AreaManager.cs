using MarkusSecundus.Utils.Extensions;
using System;
using UnityEngine;

namespace MarkusSecundus.Utils.Behaviors.GameObjects
{
    /// <summary>
    /// Componant that takes care of part of the scene and allows it to be reloaded.
    /// </summary>
    public class AreaManager : MonoBehaviour
    {
        /// <summary>
        /// Prototype of the scene segment
        /// </summary>
        public GameObject Prototype;
        /// <summary>
        /// Currently active instance of the scene segment
        /// </summary>
        public GameObject CurrentInstance {  get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            Prototype.SetActive(false);
            Reload();
        }

        /// <summary>
        /// Reload the scene segment - discard all changes and start it anew
        /// </summary>
        public void Reload()
        {
            if (CurrentInstance != null) Unload(); 

            CurrentInstance = Prototype.InstantiateWithTransform(copyScale: true);
            CurrentInstance.SetActive(true);
        }
        /// <summary>
        /// Destroy current instance of the scene segment
        /// </summary>
        public void Unload()
        {
            if (CurrentInstance != null)
            {
                Destroy(CurrentInstance);
                CurrentInstance = null;
            }
        }
    }
    
}