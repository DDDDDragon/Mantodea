using Mantodea.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mantodea.Content.Components;
using Mantodea.Content;
using FontStashSharp;

namespace Mantodea
{
    public class TextBox : SizeContainer
    {
        public TextBox(int width, int height, int leftOffset = 5, int charSpacing = 1) : base(width, height)
        {
            Text = "adad";

            _font = Main.FontManager["JetBrainsMono-Regular", 25];

            BorderWidth.Set(1);

            Cursor = new(Color.White, Color.White);
            Cursor.TextBox = this;

            UserInput.CharPressed += CharPressed;
            UserInput.KeyJustPress += KeyJustPress;

            LeftOffset = leftOffset;
            CharSpacing = charSpacing;

            BackgroundColor = Color.White;
        }

        public Cursor Cursor;

        public bool Active { get; set; }

        public int LeftOffset;

        public int CharSpacing;

        private void KeyJustPress(object sender, KeyEventArgs e)
        {
            if (Active)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        if (UserInput.Ctrl)
                        {
                            if (Text.Length > 0)
                                Cursor.SelectFrom(0, Text.Length);
                        }
                        break;
                    case Keys.C:
                        if (UserInput.Ctrl)
                        {
                            if (Cursor.SelectBegin < Cursor.CursorIndex)
                                Clipboard.SetClipboardText(Text.Substring(Cursor.SelectBegin, Cursor.CursorIndex - Cursor.SelectBegin));
                            if (Cursor.SelectBegin > Cursor.CursorIndex)
                                Clipboard.SetClipboardText(Text.Substring(Cursor.CursorIndex, Cursor.SelectBegin - Cursor.CursorIndex));
                        }
                        break;
                    case Keys.V:
                        Cursor.DeleteSelection();
                        var clip = Clipboard.GetClipboardText();
                        Text = Text.Insert(Cursor.CursorIndex, clip);
                        Cursor.CursorIndex += clip.Length;
                        Cursor.SelectBegin = Cursor.CursorIndex;
                        break;
                    case Keys.X:
                        if (UserInput.Ctrl)
                        {
                            if (Cursor.SelectBegin < Cursor.CursorIndex)
                                Clipboard.SetClipboardText(Text.Substring(Cursor.SelectBegin, Cursor.CursorIndex - Cursor.SelectBegin));
                            if (Cursor.SelectBegin > Cursor.CursorIndex)
                                Clipboard.SetClipboardText(Text.Substring(Cursor.CursorIndex, Cursor.SelectBegin - Cursor.CursorIndex));
                            Cursor.DeleteSelection();
                        }
                        break;
                    case Keys.Left:
                        if (UserInput.Shift)
                            Cursor.CursorIndex--;
                        else if (UserInput.Ctrl)
                        {

                        }
                        else
                        {
                            Cursor.CursorIndex--;
                            Cursor.SelectBegin = Cursor.CursorIndex;
                        }
                        break;
                    case Keys.Right:
                        if (UserInput.Shift)
                            Cursor.CursorIndex++;
                        else if (UserInput.Ctrl)
                        {

                        }
                        else
                        {
                            Cursor.CursorIndex++;
                            Cursor.SelectBegin = Cursor.CursorIndex;
                        }
                        break;
                    case Keys.Back:
                        if (Cursor.DeleteSelection() && Cursor.CursorIndex > 0)
                        {
                            Text = Text.Remove(Cursor.CursorIndex - 1, 1);
                            Cursor.SelectBegin--;
                            Cursor.CursorIndex--;
                        }
                        break;
                }
            }
        }

        private void CharPressed(object sender, CharacterEventArgs e)
        {
            if (Active && !UserInput.Ctrl)
            {
                if (!e.Character.Equals('\r') && !e.Character.Equals('\n'))
                {
                    Cursor.DeleteSelection();
                    Text = Text.Insert(Cursor.CursorIndex, e.Character.ToString());
                    Cursor.CursorIndex++;
                    Cursor.SelectBegin = Cursor.CursorIndex;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            Cursor.Draw(spriteBatch);

            if (string.IsNullOrEmpty(Text)) return;

            var x = Rectangle.X + LeftOffset;
            var y = Rectangle.Y + Rectangle.Height / 2 - _font.MeasureString(Text).Y / 2;

            spriteBatch.DrawString(_font, Text, new(x, y), Color.Black * _alpha, characterSpacing: CharSpacing);

            spriteBatch.DrawString(_font, "CursorIndex: " + Cursor.CursorIndex, UserInput.CurrentMouseState.Position.ToVector2() + new Vector2(20, 10), Color.Black * _alpha, characterSpacing: 1);
            spriteBatch.DrawString(_font, "SelectBegin: " + Cursor.SelectBegin, UserInput.CurrentMouseState.Position.ToVector2() + new Vector2(20, 40), Color.Black * _alpha, characterSpacing: 1);
        }

        public override void LeftClick(object sender, int pressTime, Vector2 mouseStart)
        {
            base.LeftClick(sender, pressTime, mouseStart);

            if (Clicked)
            {
                if (!Active) Active = true;
                else
                {
                    var mouse = UserInput.GetMouseRectangle().Location.ToVector2();
                    var index = (int)((mouse.X - LeftOffset) / (TextSize(1).X + CharSpacing) + 0.5);
                    Cursor.CursorIndex = index;
                    Cursor.SelectBegin = index;
                }
            }

            if (!UserInput.GetMouseRectangle().Intersects(Rectangle))
            {
                Active = false;
                Cursor.SelectBegin = Cursor.CursorIndex;
            }
        }

        public override void KeepPressLeft(object sender, int pressTime, Vector2 mouseStart)
        {
            if (!Active) return;

            var mouse = UserInput.GetMouseRectangle().Location.ToVector2();

            var index = (int)((mouse.X - LeftOffset) / (TextSize(1).X + CharSpacing) + 0.5);

            var select = (int)((mouseStart.X - LeftOffset) / (TextSize(1).X + CharSpacing) + 0.5);

            Cursor.CursorIndex = index;
            Cursor.SelectBegin = select;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Cursor.Update(gameTime);

            BorderColor = Color.Black;

            if (Active) BorderColor = Color.LightBlue;
        }

        public Vector2 TextSize(int length)
        {
            return _font.MeasureString("A").Multiply((length, 1)) + new Vector2(CharSpacing * (length - 1), 0);
        }
    }

    public class Cursor
    {
        public Cursor(Color cursorColor, Color selectionColor)
        {
            Color = cursorColor;
            SelectionColor = selectionColor;
            Visible = false;
            Timer = new();
        }

        public Color Color { get; set; }

        public Color SelectionColor { get; set; }

        public bool Visible;

        public Timer Timer;

        public TextBox TextBox;

        public bool Active => TextBox.Active;

        public int CursorIndex
        {
            get => _cursorIndex;
            set => _cursorIndex = value.Clamp(0, TextBox.Text.Length);
        }

        private int _cursorIndex;

        public int SelectBegin
        {
            get => _selectBegin;
            set => _selectBegin = value.Clamp(0, TextBox.Text.Length);
        }

        private int _selectBegin;

        public void SelectFrom(int begin, int length)
        {
            SelectBegin = begin;
            CursorIndex = begin + length;
        }

        public bool DeleteSelection()
        {
            var res = CursorIndex == SelectBegin;
            if (SelectBegin < CursorIndex)
            {
                TextBox.Text = TextBox.Text.Remove(SelectBegin, CursorIndex - SelectBegin);
                CursorIndex = SelectBegin;
            }
            else
            {
                TextBox.Text = TextBox.Text.Remove(CursorIndex, SelectBegin - CursorIndex);
                SelectBegin = CursorIndex;
            }
            return res;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active) return;

            Timer[0]++;

            if (Timer[0] >= 40)
            {
                Visible = !Visible;

                Timer[0] = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var cursorPos = TextBox.TextSize(CursorIndex);
            var selectBegin = (int)TextBox.TextSize(SelectBegin).X;

            var x = (int)cursorPos.X + TextBox.LeftOffset;
            var y = (int)(TextBox.Rectangle.Y + TextBox.Rectangle.Height / 2 - cursorPos.Y / 2);

            if (selectBegin < cursorPos.X)
                spriteBatch.Draw(SpriteBatchExt.pixel, new(selectBegin + TextBox.LeftOffset, y, (int)cursorPos.X - selectBegin, (int)TextBox._font.FontSize), null, new Color(153, 201, 239), 0, Vector2.Zero, SpriteEffects.None, 1);
            else if (selectBegin > cursorPos.X)
                spriteBatch.Draw(SpriteBatchExt.pixel, new(x, y, selectBegin - (int)cursorPos.X, (int)TextBox._font.FontSize), null, new Color(153, 201, 239), 0, Vector2.Zero, SpriteEffects.None, 1);

            if (Visible)
                spriteBatch.Draw(SpriteBatchExt.pixel, new(x, y, TextBox.CharSpacing, (int)TextBox._font.FontSize), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
    }
}
