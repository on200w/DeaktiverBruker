using System;
using System.Drawing;
using System.Windows.Forms;

namespace DeaktiverBruker.Controls
{
    public class ThemedCalendar : Control
    {
        public event Action<object, DateTime>? DateSelected;

        private DateTime month;
        private DateTime selected;

        public ThemedCalendar()
        {
            this.DoubleBuffered = true;
            this.Width = 260;
            this.Height = 200;
            month = DateTime.Today;
            selected = DateTime.Today;
            this.BackColor = Theme.Surface;
            this.ForeColor = Theme.TextPrimary;
        }

        public DateTime SelectionStart
        {
            get => selected;
            set { selected = value; Invalidate(); }
        }

        public void SetMonth(DateTime dt)
        {
            month = new DateTime(dt.Year, dt.Month, 1);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(Theme.Surface);

            var headerFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            var dayFont = new Font("Segoe UI", 9F);

            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            // draw title area
            var titleRect = new Rectangle(0, 0, w, 28);
            using (var b = new SolidBrush(Theme.HeaderBackground)) g.FillRectangle(b, titleRect);
            var title = month.ToString("MMMM yyyy");
            TextRenderer.DrawText(g, title, headerFont, titleRect, Theme.TextPrimary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // nav arrows
            var leftRect = new Rectangle(6, 4, 20, 20);
            var rightRect = new Rectangle(w - 26, 4, 20, 20);
            TextRenderer.DrawText(g, "◀", headerFont, leftRect, Theme.TextPrimary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(g, "▶", headerFont, rightRect, Theme.TextPrimary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // weekday headers
            string[] days = new[] { "man.", "tir.", "ons.", "tor.", "fre.", "lør.", "søn." };
            int gridY = 28;
            int cellW = w / 7;
            for (int i = 0; i < 7; i++)
            {
                var r = new Rectangle(i * cellW, gridY, cellW, 20);
                TextRenderer.DrawText(g, days[i], dayFont, r, Theme.TextSecondary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            // days
            var first = new DateTime(month.Year, month.Month, 1);
            int startOffset = ((int)first.DayOfWeek + 6) % 7; // make Monday=0
            int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
            int row = 0;
            int y0 = gridY + 20;
            for (int d = 1; d <= daysInMonth; d++)
            {
                int idx = startOffset + (d - 1);
                int col = idx % 7;
                row = idx / 7;
                var r = new Rectangle(col * cellW, y0 + row * 28, cellW, 28);

                var dt = new DateTime(month.Year, month.Month, d);
                bool isToday = dt.Date == DateTime.Today;
                bool isSelected = dt.Date == selected.Date;

                if (isSelected)
                {
                    using (var b = new SolidBrush(Theme.ButtonPrimary)) g.FillRectangle(b, r.X + 6, r.Y + 4, r.Width - 12, r.Height - 8);
                    TextRenderer.DrawText(g, d.ToString(), dayFont, r, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                else
                {
                    TextRenderer.DrawText(g, d.ToString(), dayFont, r, isToday ? Theme.StatusInfo : Theme.TextPrimary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            int w = this.ClientSize.Width;
            int cellW = w / 7;
            int gridY = 28 + 20;
            if (e.Y <= 28 && e.X <= 28 && e.X >= 0)
            {
                // left arrow
                month = month.AddMonths(-1);
                Invalidate();
                return;
            }
            if (e.Y <= 28 && e.X >= this.ClientSize.Width - 28)
            {
                month = month.AddMonths(1);
                Invalidate();
                return;
            }

            if (e.Y >= gridY)
            {
                int row = (e.Y - gridY) / 28;
                int col = e.X / cellW;
                int idx = row * 7 + col;
                var first = new DateTime(month.Year, month.Month, 1);
                int startOffset = ((int)first.DayOfWeek + 6) % 7;
                int day = idx - startOffset + 1;
                if (day >= 1 && day <= DateTime.DaysInMonth(month.Year, month.Month))
                {
                    selected = new DateTime(month.Year, month.Month, day);
                    DateSelected?.Invoke(this, selected);
                    Invalidate();
                }
            }
        }
    }
}
