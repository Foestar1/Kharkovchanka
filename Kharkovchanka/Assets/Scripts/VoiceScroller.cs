using UnityEngine;

public class VoiceScroller : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private AudioClip[] differentLines;

    private void Update()
    {
        float offset = Time.time * scrollSpeed;
        this.GetComponent<Renderer>().material.SetTextureOffset("_BaseMap", new Vector2(offset, 0));
    }
}
