﻿using Mantodea.Extensions;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mantodea.Content.Components
{
    public class UIText : Component
    {
        public UIText(string fontID, Vector2 size = default, string text = "", int fontSize = 20, int newLineNum = int.MaxValue,
            bool textHorizontalMiddle = false, bool textVerticalMiddle = false, Color? fontColor = null, string splitCharacter = "",
            Vector2 relativePos = default)
        {
            _font = Main.FontManager[fontID, fontSize];

            Text = text;

            NewLineNum = newLineNum;

            TextHorizontalMiddle = textHorizontalMiddle;

            TextVerticalMiddle = textVerticalMiddle;

            SplitCharacter = splitCharacter;

            FontColor = fontColor == null ? Color.White : (Color)fontColor;

            if (size == default)
                size = _font.MeasureString(text);

            _width = (int)size.X;

            _height = (int)size.Y;

            if (!string.IsNullOrEmpty(Text))
            {
                var texts = new List<string>();
                if (NewLineNum != int.MaxValue)
                    texts = Text.SplitWithCount(NewLineNum);
                if (SplitCharacter != null)
                    texts = Text.Split(SplitCharacter).ToList();

                if (_width == 0 || _height == 0)
                {
                    foreach (var txt in texts)
                        _width = Math.Max(_width, (int)_font.MeasureString(txt).X);
                    _height = (int)_font.MeasureString(texts[0]).Y * texts.Count;
                }
            }

            RelativePosition = relativePos == default ? new(0, 0) : relativePos;
        }
        internal int _width;

        internal int _height;

        public override int Width => _width;

        public override int Height => _height;

        public int NewLineNum;
        public string SplitCharacter;

        public bool TextHorizontalMiddle;
        public bool TextVerticalMiddle;

        public Color FontColor;

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!_init || !Visible) return;

            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * Alpha);

            if (!string.IsNullOrEmpty(Text))
            {
                var texts = new List<string>();
                if (NewLineNum != int.MaxValue)
                    texts = Text.SplitWithCount(NewLineNum);
                if (SplitCharacter != null)
                    texts = Text.Split(SplitCharacter).ToList();

                int x, y = 0;
                if (TextVerticalMiddle)
                    y = (Height - (int)_font.MeasureString(texts[0]).Y * texts.Count) / 2;
                foreach (var text in texts)
                {
                    var size = _font.MeasureString(text);
                    x = Rectangle.X;
                    if (TextHorizontalMiddle)
                        x += Rectangle.Width / 2 - (int)size.X / 2;
                    spriteBatch.DrawString(_font, text, new(x, y + Position.Y), FontColor * Alpha);
                    y += (int)size.Y;
                }
            }

        }
    }
}
