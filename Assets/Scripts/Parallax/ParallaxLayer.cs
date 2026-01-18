using UnityEngine;

namespace HoloJam.Parallax
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        [Range(-1f, 1f), Tooltip("The factor of the parallax, 0 for closest, 1 for farthest.")]
        public float ParallaxFactor;

        public void Move(float delta)
        {
            Vector3 newPos = transform.localPosition;
            newPos.x -= delta * ParallaxFactor;

            transform.localPosition = newPos;
        }
    }
}
