namespace Precisamento.MonoGame.Dialogue
{
    public readonly struct DialogueLine
    {
        public string Line { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public DialogueJustification InnerJustification { get; init; }
        public DialogueJustification HorizontalJustification { get; init; }

        public DialogueLine(
            string line,
            int width,
            int height,
            DialogueJustification innerJustification,
            DialogueJustification horizontalJustification)
        {
            Line = line;
            Width = width;
            Height = height;
            InnerJustification = innerJustification;
            HorizontalJustification = horizontalJustification;
        }

        public override string ToString()
        {
            return Line;
        }
    }
}
