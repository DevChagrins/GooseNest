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
        StepDirection _stepDirection = StepDirection.Normal;
        GUIStyle _style;

        Vector2 _currentPosition;

        public ScreenTextDisplay()
        {
            _screenText = new List<TextData>();
            _currentPosition = _startPosition;

            _style = new GUIStyle();
            _style.fontSize = 18;
            _style.normal.textColor = HexToColor("FFFFFFFF");
        }

        public void SetStartPosition(Vector2 position)
        {
            _startPosition = position;
        }

        public void Clear()
        {
            _screenText.Clear();
            _currentPosition = _startPosition;
        }

        public void AddText(string text)
        {
            _screenText.Add(new TextData(text, _currentPosition));
            StepOffset();
        }

        public void AddTextAtPosition(string text, Vector2 position)
        {
            _currentPosition = position;
            AddText(text);
        }

        public Vector2 GetCurrentPosition()
        {
            return _currentPosition;
        }

        public void SetPosition(Vector2 position)
        {
            _currentPosition = position;
        }

        public void SetStepDirection(StepDirection direction)
        {
            _stepDirection = direction;
        }

        public void DrawTextToGUI()
        {
            int textCount = _screenText.Count;
            for (int textIndex = 0; textIndex < textCount; textIndex++)
            {
                GUI.Label(new Rect(_screenText[textIndex].Position, _labelSize), _screenText[textIndex].Text, _style);
            }
        }

        private void StepOffset()
        {
            if (_stepDirection == StepDirection.Normal)
            {
                _currentPosition += _stepTextOffset;
            }
            else
            {
                _currentPosition -= _stepTextOffset;
            }
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }
    }

    enum StepDirection
    {
        Normal,
        Reverse
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
