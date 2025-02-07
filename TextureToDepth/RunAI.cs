using System.Collections;
using System.Collections.Generic;
using Unity.Sentis;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Sentis.Model;
using static Unity.VisualScripting.Member;


public class RunAI : MonoBehaviour
{
    public Texture InputTexture;
    public Material outputMaterial;

    Tensor<float> inputTensor;
    public RenderTexture outputTexture;
    Worker m_engineEstimation;

    public enum Modelname {dpt_swin2_base_384,dpt_swin2_large_384, dpt_swin2_tiny_256}

    public Modelname modelname;
     string[] modelnames = { "dpt_swin2_base_384", "dpt_swin2_large_384", "dpt_swin2_tiny_256" };
    
    void Awake()
    {
     
        Model model = ModelLoader.Load(Application.streamingAssetsPath +"/"+ modelnames[(int)modelname]+".sentis");

        // Post process
        var graph = new FunctionalGraph();
        var inputs = graph.AddInputs(model);
        var outputs = Functional.Forward(model, inputs);
        var output = outputs[0];

        var max0 = Functional.ReduceMax(output, new[] { 1, 2 }, false);
        var min0 = Functional.ReduceMin(output, new[] { 1, 2 }, false);
        output = (output - min0) / (max0 - min0);

        model = graph.Compile(output);


        m_engineEstimation = new Worker(model, BackendType.GPUCompute);

        outputTexture = new RenderTexture(InputTexture.width, InputTexture.height, 0, RenderTextureFormat.ARGBFloat);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();
        if((int)modelname == 2)
        {
            int inputwidth = 256,inputheight = 256;
            inputTensor = new Tensor<float>(new TensorShape(1, 3, inputwidth,inputheight));
        }
        else
        {
            int inputwidth = 384, inputheight = 384;
            inputTensor = new Tensor<float>(new TensorShape(1, 3, inputwidth, inputheight));
        }
        
    }


    void Update()
    {

        TextureConverter.ToTensor(InputTexture, inputTensor, new TextureTransform());
        m_engineEstimation.Schedule(inputTensor);
        var output = m_engineEstimation.PeekOutput() as Tensor<float>;
        Debug.Log(output.shape);
        output.Reshape(output.shape.Unsqueeze(0));
        Debug.Log(output.shape);
        TextureConverter.RenderToTexture(output, outputTexture, new TextureTransform().SetCoordOrigin(CoordOrigin.TopLeft));
        outputMaterial.mainTexture = outputTexture;

    }


    void OnDestroy()
    {
        m_engineEstimation.Dispose();
        inputTensor.Dispose();
        outputTexture.Release();
    }



}