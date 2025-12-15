using System;
using System.Drawing;
using System.Windows.Forms;

namespace DeaktiverBruker.Controls
{
    public class ThemedDatePicker : UserControl
    {
        private TextBox txt;
        private Button btn;
        private ThemedCalendar cal;
        private ToolStripDropDown drop;

        public ThemedDatePicker()
        {
            this.Height = 24;
            txt = new TextBox() { BorderStyle = BorderStyle.FixedSingle, ReadOnly = false, Dock = DockStyle.Fill };
            btn = new Button() { Text = "▾", Dock = DockStyle.Right, Width = 24, FlatStyle = FlatStyle.Flat };
            btn.FlatAppearance.BorderSize = 0;
            this.Controls.Add(txt);
            this.Controls.Add(btn);

            cal = new ThemedCalendar();
            var host = new ToolStripControlHost(cal) { Margin = Padding.Empty, Padding = Padding.Empty };
            drop = new ToolStripDropDown();
            drop.Padding = Padding.Empty;
            drop.Items.Add(host);

            btn.Click += (s, e) => {
                if (!drop.Visible)
                {
                    cal.SelectionStart = Value.Date;
                    cal.SetMonth(Value);
                    var pt = this.PointToScreen(new Point(0, this.Height));
                    drop.Show(pt);
                }
                else drop.Close();
            };

            cal.DateSelected += (s, dt) => {
                Value = dt;
                drop.Close();
            };

            txt.Leave += (s, e) => { DateTime.TryParse(txt.Text, out var d); Value = d == default ? DateTime.Today : d; };
            Value = DateTime.Today;
        }

        public DateTime Value
        {
            get
            {
                if (DateTime.TryParse(txt.Text, out var d)) return d.Date;
                return DateTime.Today;
            }
            set
            {
                txt.Text = value.ToString("dd.MM.yyyy");
            }
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                if (txt != null) txt.BackColor = value;
            }
        }

        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                if (txt != null) txt.ForeColor = value;
            }
        }
    }
}
