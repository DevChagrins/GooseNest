using System;
using System.Collections.Generic;
using UnityEngine;

namespace GooseNest
{
    class ScreenTextDisplay
    {
        List<TextData> _screenText;
        Vector2 _startPosition = Vector2.zero;
        Vector2 _stepTextOffset = new Vector2(0f, 20f);
        Vector2 _labelSize = new Vector2(2000f, 200f);

        Vector2 CurrentPosition;

        public ScreenTextDisplay()
        {
            _screenText = new List<TextData>();
            CurrentPosition = _startPosition;
        }

        public void SetStartPosition(Vector2 position)
        {
            _startPosition = position;
        }

        public void Clear()
        {
            _screenText.Clear();
            CurrentPosition = _startPosition;
        }

        public void AddText(string text)
        {
            _screenText.Add(new TextData(text, CurrentPosition));
            StepOffset();
        }

        public void AddTextAtPosition(string text, Vector2 position)
        {
            CurrentPosition = position;
            AddText(text);
        }

        private void StepOffset()
        {
            CurrentPosition += _stepTextOffset;
        }

        public void DrawTextToGUI()
        {
            int textCount = _screenText.Count;
            for (int textIndex = 0; textIndex < textCount; textIndex++)
            {
                GUI.Label(new Rect(_screenText[textIndex].Position, _labelSize), _screenText[textIndex].Text);
            }
        }
    }

    struct TextData
    {
        public string Text;
        public Vector2 Position;

        public TextData(string text, Vector2 position)
        {
            Text = text;
            Position = position;
        }
    }
}
