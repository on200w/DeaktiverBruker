using System;
using System.Drawing;
using System.Windows.Forms;

namespace DeaktiverBruker.Controls
{
    public class ThemedTimePicker : UserControl
    {
        private MaskedTextBox timeText;

        public ThemedTimePicker()
        {
            this.Height = 24;

            // masked textbox with fixed ':' so colon cannot be deleted
            timeText = new MaskedTextBox()
            {
                Mask = "00:00",
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(0),
                PromptChar = '_',
                BeepOnError = false
            };

            timeText.BackColor = base.BackColor;
            timeText.ForeColor = base.ForeColor;

            this.Controls.Add(timeText);

            // events
            timeText.KeyDown += TimeText_KeyDown;
            timeText.Leave += TimeText_Leave;
            timeText.MouseWheel += TimeText_MouseWheel;
            timeText.MaskInputRejected += (s, e) => { /* ignore */ };

            Value = DateTime.Now;
        }

        private void TimeText_MouseWheel(object? sender, MouseEventArgs e)
        {
            AdjustMinutes(e.Delta > 0 ? 1 : -1);
        }

        private void TimeText_Leave(object? sender, EventArgs e)
        {
            if (DateTime.TryParse(timeText.Text, out var d))
                Value = d;
            else
                timeText.Text = Value.ToString("HH:mm");
        }

        private void TimeText_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (DateTime.TryParse(timeText.Text, out var d)) Value = d;
                else timeText.Text = Value.ToString("HH:mm");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                AdjustMinutes(1);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                AdjustMinutes(-1);
                e.Handled = true;
            }
        }

        private void AdjustMinutes(int delta)
        {
            if (!DateTime.TryParse(timeText.Text, out var cur)) cur = DateTime.Now;
            var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, cur.Hour, cur.Minute, 0).AddMinutes(delta);
            timeText.Text = dt.ToString("HH:mm");
        }

        public DateTime Value
        {
            get
            {
                var now = DateTime.Now;
                if (DateTime.TryParse(timeText.Text, out var parsed))
                {
                    return new DateTime(now.Year, now.Month, now.Day, parsed.Hour, parsed.Minute, 0);
                }
                return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            }
            set
            {
                timeText.Text = value.ToString("HH:mm");
            }
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                if (timeText != null) timeText.BackColor = value;
            }
        }

        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                if (timeText != null) timeText.ForeColor = value;
            }
        }
    }
}
