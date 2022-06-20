namespace Horticultist.Scripts.UI
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.UI;
    public class CutoutMaskUI : Image

    {
        public override Material materialForRendering
        {
            get
            {
                Material result = new Material(base.materialForRendering);
                result.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return result;
            }
        }
    }
}
