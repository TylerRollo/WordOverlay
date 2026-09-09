namespace WordOverlay.Models
{
    public class WordEntry
    {
        public string Word { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
    }

    public class OverlayConfig
    {
        public double Left { get; set; } = -1;
        public double Top { get; set; } = -1;
        public double Width { get; set; } = 320;
        public double Height { get; set; } = 140;
        public int RotationSeconds { get; set; } = 120;
    }
}
