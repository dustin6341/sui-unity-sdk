using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VectorGraphics;

public class SpriteSVGNftLoader : MonoBehaviour {
    public string NftObjectId;
    
    private async void Start()
    {
        var objectRpcResult = await SuiApi.Client.GetObjectAsync(NftObjectId);
        
        if (objectRpcResult.IsSuccess)
        {
            var url =  objectRpcResult.Result.Object.Data.Fields["url"] as string;
            var sceneInfo = await LoadSVGAsync(url);
            DrawSVG(sceneInfo);
        }
    }
 
    public void DrawSVG(SVGParser.SceneInfo sceneInfo) {
        // Dynamically import the SVG data, and tessellate the resulting vector scene.
        var tesselationOptions = new VectorUtils.TessellationOptions()
        {
            StepDistance = 10f,
            MaxCordDeviation = 2f,
            SamplingStepSize = 0.5f,
            MaxTanAngleDeviation = float.MaxValue
        };
        var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tesselationOptions);
 
        // Build a sprite with the tessellated geometry.
        var sprite = VectorUtils.BuildSprite(geoms, 100.0f, VectorUtils.Alignment.SVGOrigin, Vector2.zero, 256, true);
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
 
    private async Task<SVGParser.SceneInfo> LoadSVGAsync(string url)
    {
        var svgText = await DownladSVGTextAsync(url);
        using (var reader = new StringReader(svgText))
        {
            return SVGParser.ImportSVG(reader);
        }
    }
    
    public async Task<string> DownladSVGTextAsync(string url)
    {
        var req = await UnityWebRequests.GetAsync(url);
        return req.downloadHandler.text;
    }
}