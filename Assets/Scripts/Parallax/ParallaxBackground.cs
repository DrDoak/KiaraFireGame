using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace HoloJam.Parallax
{
    [ExecuteInEditMode]
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField]
        private ParallaxCamera parallaxCamera;
        public List<ParallaxLayer> layers = new List<ParallaxLayer>();

        private void Start()
        {
            if (parallaxCamera == null)
            {
                // get component, if none is found add it to the camera
                if (!Camera.main.TryGetComponent<ParallaxCamera>(out parallaxCamera))
                    parallaxCamera = Camera.main.AddComponent<ParallaxCamera>();
            }

            if (parallaxCamera != null)
                parallaxCamera.OnCameraTranslate += Move;

            SetLayers();
        }

        private void OnDisable()
        {
            if (parallaxCamera != null)
            {
                parallaxCamera.OnCameraTranslate -= Move;
            }
        }

        private void OnDestroy()
        {
            if (parallaxCamera != null)
            {
                parallaxCamera.OnCameraTranslate -= Move;
            }
        }

        public void SetLayers()
        {
            // clear out the layes
            layers.Clear();

            // initialize the layers
            for (int i = 0; i < transform.childCount; i++)
            {
                // get the layer from the child object
                ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();
                
                // add the layer and rename it accordingly
                if (layer != null)
                {
                    layer.name = "Layer - " + i;
                    layers.Add(layer);
                }
            }
        }

        private void Move(float delta)
        {
            foreach(ParallaxLayer layer in layers)
            {
                layer.Move(delta);
            }
        }
    }
}
