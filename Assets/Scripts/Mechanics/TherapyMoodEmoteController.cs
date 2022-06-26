namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class TherapyMoodEmoteController : MonoBehaviour
    {
        [SerializeField] private Vector2 offset;
        private Camera mainCamera;
        private float baseXScale;

        private void Awake()
        {
            mainCamera = Camera.main;
            baseXScale = transform.localScale.x;
        }

        public void ShowEmote(Vector2 screenPos)
        {
            var worldPos = mainCamera.ScreenToWorldPoint(screenPos);

            var mousePos = Mouse.current.position.ReadValue();
            // Flip on x axis if the object is on the left side of the screen
            if (mousePos.x < Screen.width / 2f)
            {
                transform.localScale = new Vector3(
                    transform.localScale.x * -1,
                    transform.localScale.y,
                    transform.localScale.z
                );
                transform.position = (Vector2)worldPos - offset;
            }
            else
            {
                transform.position = (Vector2)worldPos + offset;
            }
            StartCoroutine(ShowEmoteCoroutine());
        }

        private IEnumerator ShowEmoteCoroutine()
        {
            yield return new WaitForSeconds(0.75f);
            gameObject.SetActive(false);

            // Reset flip
            transform.localScale = new Vector3(
                transform.localScale.x * -1,
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
}