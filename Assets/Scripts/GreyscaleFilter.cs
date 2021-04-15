using deVoid.Utils;
using UnityEngine;

[ExecuteInEditMode]
public class GreyscaleFilter : MonoBehaviour
{
    [SerializeField] private Texture _texture;

    private Material _mat;
    private static readonly int Mask = Shader.PropertyToID("_Mask");
    private bool _on;

    private void OnEnable()
    {
        Signals.Get<GreyscaleOn>().AddListener(GreyscaleOn);
    }

    private void OnDisable()
    {
        if (_mat)
        {
            DestroyImmediate(_mat);
        }

        Signals.Get<GreyscaleOn>().RemoveListener(GreyscaleOn);
    }

    private void GreyscaleOn()
    {
        _on = true;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!_mat)
        {
            _mat = new Material(Shader.Find("Custom/GSF"));
            _mat.SetTexture(Mask, _texture);
        }

        if (_on)
        {
            Graphics.Blit(src, dest, _mat);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}