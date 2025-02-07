using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngineInternal;


public class DepthMeshIndirect : MonoBehaviour
{
    public Texture Colormap;
    public Texture Input;
    public Material mat;
    public Mesh mesh;
    public ComputeShader cs;

    TextureDepthGPUInstancing textureDepthGPUInstancing;
    void Start()
    {
        textureDepthGPUInstancing = new TextureDepthGPUInstancing(Colormap,Input, mat, mesh, cs);
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

public class TextureDepthGPUInstancing
{
    // Initialize
    Texture DepthTex;
    Material material;
    Mesh mesh;
    ComputeShader cs;
    // const
    ComputeBuffer positionsBuffer;
    ComputeBuffer colorsBuffer;
    static readonly int
        widthId = Shader.PropertyToID("width"),
        heightId = Shader.PropertyToID("height"),
        positionId = Shader.PropertyToID("_Positions"),
        colorsId= Shader.PropertyToID("_Colors"),
        colormapId=Shader.PropertyToID("Colormap"),
        DepthId = Shader.PropertyToID("DepthTexture");
    int groupX,groupY;
    Bounds bounds;

    public TextureDepthGPUInstancing(Texture Colormap,Texture DepthTex, Material material,Mesh mesh,ComputeShader cs)
    {
        this.DepthTex = DepthTex;
        this.material = material;
        this.mesh = mesh;
        this.cs = cs;
        this.groupX = Mathf.CeilToInt(DepthTex.width / 8f);
        this.groupY = Mathf.CeilToInt(DepthTex.height / 8f);

        positionsBuffer = new ComputeBuffer(DepthTex.width * DepthTex.height, Marshal.SizeOf<Vector3>());
        colorsBuffer = new ComputeBuffer(Colormap.width * Colormap.height, Marshal.SizeOf<Color>());

        cs.SetInt(widthId, DepthTex.width);
        cs.SetInt(heightId, DepthTex.height);
        cs.SetBuffer(0,positionId,positionsBuffer);
        cs.SetBuffer(0,colorsId,colorsBuffer);
        cs.SetTexture(0, colormapId, Colormap);
        cs.SetTexture(0, DepthId, DepthTex);
        this.bounds = new Bounds(Vector3.zero, 5 * Vector3.one);


        material.SetBuffer(positionId, positionsBuffer);
        material.SetBuffer(colorsId,colorsBuffer);
    }

    public void  Updateposition()
    {
        cs.Dispatch(0, groupX, groupY, 1);
    }
    public void draw()
    {
       
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }

    public void Release()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }
}