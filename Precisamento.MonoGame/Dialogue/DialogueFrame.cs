using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using Precisamento.MonoGame.Dialogue.AttributeProcessors;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueFrame
    {
        private static List<IDialogueProcessor> _processorStack = new List<IDialogueProcessor>();

        public int LineStart { get; set; }
        public List<DialogueLine> Lines { get; } = new List<DialogueLine>();
        public List<IDialogueProcessor> Processors { get; } = new List<IDialogueProcessor>();
        public bool HasCustomProcessors { get; private set; }
        public Size TotalSize { get; private set; }
        public bool IsOption { get; set; }
        public string[] Metadata { get; private set; }

        public DialogueFrame()
        {
        }

        public DialogueFrame(
            LocalizedLine line,
            DialogueProcessorFactory processorFactory,
            DialogueState state,
            ISentenceSplitter sentenceSplitter,
            int lineStart)
        {
            Init(line, processorFactory, state, sentenceSplitter, lineStart);
        }

        public void Init(
            LocalizedLine line,
            DialogueProcessorFactory processorFactory,
            DialogueState state,
            ISentenceSplitter sentenceSplitter,
            int lineStart)
        {
            Metadata = line.Metadata;
            GenerateLines(line, processorFactory, state, sentenceSplitter);
            LineStart = lineStart;
        }

        private void GenerateLines(
            LocalizedLine line,
            DialogueProcessorFactory processorFactory,
            DialogueState state,
            ISentenceSplitter sentenceSplitter)
        {
            var text = line.Text.Text;
            IDialogueCustomDraw? customProcessor = null;
            var currentLine = new StringBuilder();
            var letterBuffer = new StringBuilder(1);
            foreach (var attribute in line.Text.Attributes)
                Processors.Add(processorFactory.Get(attribute));

            var width = 0;
            var maxHeight = 0;
            var lastSplitIndex = -1;
            var lastSplitWidth = 0;
            var totalSize = new Size();

            for (var letter = 0; letter < text.Length; letter++)
            {
                foreach (var processor in Processors.Where(
                    p => p.Position == letter
                    || p.Position + p.Length == letter))
                {
                    if (processor.Position == letter)
                    {
                        processor.Push(state);
                        _processorStack.Add(processor);
                        if (processor.CustomDraw && processor is IDialogueCustomDraw customDraw)
                        {
                            customProcessor = customDraw;
                            HasCustomProcessors = true;
                        }
                    }

                    if (processor.Position + processor.Length == letter)
                    {
                        processor.Pop(state);
                        _processorStack.Remove(processor);
                        if (processor.CustomDraw)
                        {
                            customProcessor = _processorStack.LastOrDefault(p => p.CustomDraw) as IDialogueCustomDraw;
                        }
                    }
                }

                Vector2 size;

                if (customProcessor != null)
                {
                    size = customProcessor.Measure(text[letter], state);
                }
                else
                {
                    letterBuffer.Clear();
                    letterBuffer.Append(text[letter]);
                    size = state.Fonts.Peek().MeasureString(letterBuffer);
                }

                var measuredWidth = size.X;
                maxHeight = (int)MathF.Ceiling(Math.Max(maxHeight, size.Y));

                if (width + measuredWidth > state.DrawArea.Width && lastSplitIndex != -1)
                {
                    letterBuffer.Clear();
                    letterBuffer.Append(currentLine, lastSplitIndex, currentLine.Length - lastSplitIndex);
                    var dialogueLine = new DialogueLine()
                    {
                        Line = currentLine.ToString(0, lastSplitIndex),
                        Width = lastSplitWidth,
                        Height = maxHeight,
                        InnerJustification = state.InnerJustification,
                        HorizontalJustification = state.HorizontalJustification
                    };
                    totalSize.Width = Math.Max(totalSize.Width, lastSplitWidth);
                    totalSize.Height += maxHeight;
                    Lines.Add(dialogueLine);
                    currentLine.Clear();
                    var temp = currentLine;
                    currentLine = letterBuffer;
                    letterBuffer = temp;
                    width -= lastSplitWidth;
                    lastSplitIndex = -1;
                    lastSplitWidth = 0;
                    maxHeight = 0;
                }

                width += (int)measuredWidth;
                currentLine.Append(text[letter]);

                if (sentenceSplitter.CanSplit(text, letter))
                {
                    lastSplitIndex = currentLine.Length;
                    lastSplitWidth = width;
                }
            }

            var finalLine = new DialogueLine()
            {
                Line = currentLine.ToString(),
                Width = width,
                Height = maxHeight,
                InnerJustification = state.InnerJustification,
                HorizontalJustification = state.HorizontalJustification
            };

            totalSize.Width = Math.Max(totalSize.Width, width);
            totalSize.Height += maxHeight;

            TotalSize = totalSize;

            Lines.Add(finalLine);
        }

        public bool Draw(SpriteBatchState spriteBatchState, DialogueState dialogueState, int startingLine, int endingLine, int column)
        {
            var startingLineIndex = Math.Max(startingLine - LineStart, 0);
            var maxDrawHeight = dialogueState.DrawArea.Y + dialogueState.DrawArea.Height;

            if (dialogueState.CurrentPosition.Y + Lines[startingLineIndex].Height > maxDrawHeight)
                return false;

            var startX = dialogueState.CurrentPosition.X;
            var doneDrawing = false;
            var attributeStart = Lines.Take(startingLineIndex).Select(l => l.Line.Length).Sum();

            foreach (var processor in Processors)
            {
                if (processor.Position < attributeStart && processor.Position + processor.Length >= attributeStart)
                {
                    processor.Push(dialogueState);
                    _processorStack.Add(processor);
                }
            }

            for (var i = startingLineIndex; i <= endingLine; i++)
            {
                var segment = Lines[i];
                var length = segment.Line.Length;
                var lastLetter = 0;
                var position = dialogueState.CurrentPosition;

                switch (segment.HorizontalJustification)
                {
                    case DialogueJustification.Center:
                        position.X += MathF.Round((dialogueState.DrawArea.Width - segment.Width) * 0.5f);
                        break;
                    case DialogueJustification.Right:
                        position.X += dialogueState.DrawArea.Width - segment.Width;
                        break;
                }

                dialogueState.CurrentPosition = position;

                if (i == endingLine)
                {
                    length = column;
                }

                for (var letter = 0; letter < length; letter++)
                {
                    foreach (var processor in Processors
                        .Where(p => p.Position == attributeStart + letter ||
                            p.Position + p.Length == attributeStart + letter))
                    {
                        if (lastLetter != letter)
                        {
                            DrawSegment(spriteBatchState, dialogueState, segment, lastLetter, letter);
                            lastLetter = letter;
                        }

                        if (processor.Position == attributeStart + letter)
                        {
                            processor.Push(dialogueState);
                            _processorStack.Add(processor);
                        }
                        else
                        {
                            processor.Pop(dialogueState);
                            _processorStack.Remove(processor);
                        }
                    }
                }

                if (lastLetter != length)
                {
                    DrawSegment(spriteBatchState, dialogueState, segment, lastLetter, length);
                }

                attributeStart += length;
                var newX = startX;

                dialogueState.CurrentPosition = new Vector2(
                    startX,
                    dialogueState.CurrentPosition.Y + segment.Height);

                int nextLineHeight = 0;

                if (i < endingLine)
                {
                    nextLineHeight = Lines[i + 1].Height;
                }

                if (dialogueState.CurrentPosition.Y + nextLineHeight > maxDrawHeight)
                {
                    doneDrawing = true;
                    break;
                }
            }

            for (var j = _processorStack.Count - 1; j >= 0; j--)
            {
                var last = _processorStack[j];
                _processorStack.RemoveAt(j);
                last.Pop(dialogueState);
            }

            return doneDrawing;
        }

        private void DrawSegment(SpriteBatchState spriteBatchState, DialogueState dialogueState, DialogueLine line, int start, int end)
        {
            var font = dialogueState.Fonts.Peek();
            var text = line.Line;
            IDialogueCustomDraw? customDraw = HasCustomProcessors
                ? _processorStack.LastOrDefault(p => p.CustomDraw) as IDialogueCustomDraw
                : null;

            if (customDraw != null)
            {
                for (; start < end; start++)
                {

                    customDraw.Draw(spriteBatchState, dialogueState);
                    dialogueState.CurrentPosition = new Vector2(
                        dialogueState.CurrentPosition.X + customDraw.Measure(text[start], dialogueState).X,
                        dialogueState.CurrentPosition.Y);
                }
            }
            else
            {
                var subString = text[start..end];
                var size = font.MeasureString(subString);
                var position = dialogueState.CurrentPosition;
                switch (line.InnerJustification)
                {
                    case DialogueJustification.Center:
                        position.Y += MathF.Round((line.Height - size.Y) * 0.5f);
                        break;
                    case DialogueJustification.Bottom:
                        position.Y += line.Height - size.Y;
                        break;
                }
                spriteBatchState.SpriteBatch.DrawString(
                    font,
                    subString,
                    position,
                    dialogueState.Colors.Peek());

                dialogueState.CurrentPosition = new Vector2(
                    dialogueState.CurrentPosition.X + size.X,
                    dialogueState.CurrentPosition.Y);
            }
        }

        public void Reset()
        {
            foreach(var processor in Processors)
                processor.Reset();
            Processors.Clear();
            Lines.Clear();
        }
    }
}
