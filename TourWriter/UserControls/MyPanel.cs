using System.Drawing;
using System.Windows.Forms;

namespace TourWriter.UserControls
{
    /// <summary>
    /// Custom panel with Border settings, inherits from Windows.Forms.Panel
    /// </summary>
    public class MyPanel : Panel
    {
        private int borderWidth = 1;
        private Color borderColor = Color.Red;
        private ButtonBorderStyle borderStyle = ButtonBorderStyle.None;
        
        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        public int BorderWidth
        {
            get { return borderWidth;  }
            set { borderWidth = value; }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        /// <summary>
        /// Gets or sets the style of the border.
        /// </summary>
        public new ButtonBorderStyle BorderStyle
        {
            get { return borderStyle; }
            set { borderStyle = value; }
        }

        /// <summary>
        /// Overrides the base OnPaint event, to draw a border around the base panel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            ControlPaint.DrawBorder(
              e.Graphics, e.ClipRectangle,
              BorderColor, BorderWidth, borderStyle,
              BorderColor, BorderWidth, borderStyle,
              BorderColor, BorderWidth, borderStyle,
              BorderColor, BorderWidth, borderStyle);
        }
    }
}