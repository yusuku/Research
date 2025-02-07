using UnityEngine;

public class Tex2Cube : MonoBehaviour
{
    public Texture Colormap;
    public Texture Depth;
    public Material mat;
    public Mesh mesh;
    public ComputeShader cs;

    TextureDepthGPUInstancing textureDepthGPUInstancing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Colormap = GetComponent<Postprocess>().Colormap;
        Depth = GetComponent<Postprocess>().PosprocessTex;
        textureDepthGPUInstancing = new TextureDepthGPUInstancing(Colormap, Depth, mat, mesh, cs);
    }

    void Update()
    {
        textureDepthGPUInstancing.Updateposition();
        textureDepthGPUInstancing.draw();
    }
    void OnDestroy()
    {

        textureDepthGPUInstancing.Release();
    }
}
