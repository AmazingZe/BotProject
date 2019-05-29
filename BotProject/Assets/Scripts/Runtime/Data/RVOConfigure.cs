namespace GameRuntime
{
    using UnityEditor;
    using UnityEngine;

    public class RVOConfigure : Configure
    {
        public int SimulationFPS = 30;

        protected override void OnDraw()
        {
            SimulationFPS = EditorUtils.IntFieldWithLabel("SimulationFPS", SimulationFPS);
        }
    }
}