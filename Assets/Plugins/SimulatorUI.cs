using Gtec.Chain.Common.Nodes.FilterNodes;
using UnityEngine;
namespace Gtec.Bandpower
{
    public class SimulatorUI : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The alpha level in µV.")]
        [Range(0, 100)]

        public float AlphaLevelUv = 0;
        private Device _device = null;
        // Start is called before the first frame update
        void Start()
        {
            _device = GetComponentInParent<Device>();
        }

        // Update is called once per frame
        void Update()
        {
            try
            {
                _device.SetAlphaLevel(AlphaLevelUv);
            }
            catch 
            { 
                //do nothing
            }
        }
    }
}