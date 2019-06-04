namespace GameRuntime
{
    using UnityEngine;

    public class AStarColor
    {
        public Color _SolidColor;
        public Color _UnwalkableNode;
        public Color _BoundsHandles;

        public Color _ConnectionLowLerp;
        public Color _ConnectionHighLerp;

        public Color _MeshEdgeColor;
        
        public Color[] _AreaColors;

        public static Color SolidColor = new Color(30 / 255f, 102 / 255f, 201 / 255f, 0.9F);
        public static Color UnwalkableNode = new Color(1, 0, 0, 0.5F);
        public static Color BoundsHandles = new Color(0.29F, 0.454F, 0.741F, 0.9F);

        public static Color ConnectionLowLerp = new Color(0, 1, 0, 0.5F);
        public static Color ConnectionHighLerp = new Color(1, 0, 0, 0.5F);

        public static Color MeshEdgeColor = new Color(0, 0, 0, 0.5F);

        private static Color[] AreaColors = new Color[1];

        public static int ColorHash()
        {
            var hash = SolidColor.GetHashCode() ^ UnwalkableNode.GetHashCode() ^ BoundsHandles.GetHashCode() ^ ConnectionLowLerp.GetHashCode() ^ ConnectionHighLerp.GetHashCode() ^ MeshEdgeColor.GetHashCode();

            for (int i = 0; i < AreaColors.Length; i++) hash = 7 * hash ^ AreaColors[i].GetHashCode();
            return hash;
        }
        
        public static Color GetAreaColor(uint area)
        {
            if (area >= AreaColors.Length) return IntToColor((int)area, 1F);
            return AreaColors[(int)area];
        }
        
        public static Color GetTagColor(uint tag)
        {
            if (tag >= AreaColors.Length) return IntToColor((int)tag, 1F);
            return AreaColors[(int)tag];
        }
        
        public void PushToStatic(NavSystem active)
        {
            _AreaColors = _AreaColors ?? new Color[1];

            SolidColor = _SolidColor;
            UnwalkableNode = _UnwalkableNode;
            BoundsHandles = _BoundsHandles;
            ConnectionLowLerp = _ConnectionLowLerp;
            ConnectionHighLerp = _ConnectionHighLerp;
            MeshEdgeColor = _MeshEdgeColor;
            AreaColors = _AreaColors;
        }

        public AStarColor()
        {
            // Set default colors
            _SolidColor = new Color(30 / 255f, 102 / 255f, 201 / 255f, 0.9F);
            _UnwalkableNode = new Color(1, 0, 0, 0.5F);
            _BoundsHandles = new Color(0.29F, 0.454F, 0.741F, 0.9F);
            _ConnectionLowLerp = new Color(0, 1, 0, 0.5F);
            _ConnectionHighLerp = new Color(1, 0, 0, 0.5F);
            _MeshEdgeColor = new Color(0, 0, 0, 0.5F);
        }

        public static Color IntToColor(int i, float a)
        {
            int r = Bit(i, 2) + Bit(i, 3) * 2 + 1;
            int g = Bit(i, 1) + Bit(i, 4) * 2 + 1;
            int b = Bit(i, 0) + Bit(i, 5) * 2 + 1;

            return new Color(r * 0.25F, g * 0.25F, b * 0.25F, a);
        }
        private static int Bit(int a, int b)
        {
            return (a >> b) & 1;
        }
    }
}