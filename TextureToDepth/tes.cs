using UnityEngine;

public class tes : MonoBehaviour
{
    public Texture tesd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tesd=GetComponent<RunAI>().outputTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
