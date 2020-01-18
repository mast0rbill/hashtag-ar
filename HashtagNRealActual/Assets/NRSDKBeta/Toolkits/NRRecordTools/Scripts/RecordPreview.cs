namespace NRKernal.Record
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RawImage))]
    public class RecordPreview : MonoBehaviour
    {
        private RecordController RecordController { get; set; }
        public RawImage m_PreviewImage;

        void Start()
        {
            RecordController = FindObjectOfType<RecordController>();
            m_PreviewImage = gameObject.GetComponent<RawImage>();
        }

        void Update()
        {
            if (RecordController != null)
            {
                m_PreviewImage.texture = RecordController.PreviewTexture;
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                m_PreviewImage.enabled = !m_PreviewImage.enabled;
            }
        }
    }
}
