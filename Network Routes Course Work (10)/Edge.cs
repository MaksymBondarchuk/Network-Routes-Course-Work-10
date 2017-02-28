using System.Windows;

namespace Network_Routes_Course_Work_10
{
    public class Edge
    {
        public const int EdgeFontSize = 15;

        public const int StrokeThickness = 30;
        
        /// <summary>
        /// First point coordinates
        /// </summary>
        public Point P1;

        /// <summary>
        /// Second point coordinates
        /// </summary>
        public Point P2;

        /// <summary>
        /// First connected vertex
        /// </summary>
        public int Vertex1;

        /// <summary>
        /// Second connected vertex
        /// </summary>
        public int Vertex2;

        /// <summary>
        /// Index in Canvas.Children
        /// </summary>
        public int CanvasIdx;
    }
}
