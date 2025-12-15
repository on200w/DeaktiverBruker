using System.Drawing;
using System.Windows.Forms;

static class Theme
{
    // Colors
    public static readonly Color Background = Color.FromArgb(24, 24, 24);
    public static readonly Color Surface = Color.FromArgb(32, 32, 32);
    public static readonly Color SurfaceLight = Color.FromArgb(45, 45, 48);
    public static readonly Color Header = Color.FromArgb(18, 18, 18);
    public static readonly Color HeaderBackground = Color.FromArgb(28, 28, 28);
    public static readonly Color TextPrimary = Color.FromArgb(255, 255, 255);
    public static readonly Color TextSecondary = Color.FromArgb(200, 200, 200);
    // Primary button color requested: #23A154 (35,161,84)
    public static readonly Color ButtonPrimary = Color.FromArgb(0x23, 0xA1, 0x54);
    public static readonly Color ButtonPrimaryHover = Color.FromArgb(55, 181, 104);
        public static readonly Color ButtonNeutral = Color.FromArgb(64, 64, 64);
        public static readonly Color ButtonNeutralHover = Color.FromArgb(80, 80, 80);
    public static readonly Color AccentBlue = ButtonPrimary;
    public static readonly Color AccentBlueHover = ButtonPrimaryHover;
    public static readonly Color AccentGreen = ButtonPrimary;
    public static readonly Color AccentGreenHover = ButtonPrimaryHover;
    public static readonly Color NeutralGray = Color.FromArgb(80, 80, 80);
    public static readonly Color NeutralGrayHover = Color.FromArgb(100, 100, 100);
    public static readonly Color BorderSubtle = Color.FromArgb(60, 60, 60);
    public static readonly Color StatusOk = Color.FromArgb(76, 175, 80);
    public static readonly Color StatusError = Color.FromArgb(244, 67, 54);
    public static readonly Color StatusInfo = Color.FromArgb(33, 150, 243);

    // Fonts
    public static readonly Font TitleFont = new Font("Segoe UI", 18F, FontStyle.Bold);
    public static readonly Font SubtitleFont = new Font("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font SectionFont = new Font("Segoe UI", 11F, FontStyle.Bold);
    public static readonly Font BodyFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
    public static readonly Font ButtonFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);

    // Spacing
    public const int Padding = 16;
    public const int PaddingSmall = 8;
    public const int PaddingLarge = 24;
    public const int CornerRadius = 6;

    public static void ApplyButtonStyle(Button btn, Color bgColor, Color hoverColor)
    {
        btn.BackColor = bgColor;
        btn.ForeColor = TextPrimary;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = ButtonFont;
        btn.Cursor = Cursors.Hand;
        
        btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
        btn.MouseLeave += (s, e) => btn.BackColor = bgColor;
    }

    public static Panel CreateCard(int x, int y, int width, int height)
    {
        var panel = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(width, height),
            BackColor = Surface,
            Padding = new Padding(Padding)
        };
        return panel;
    }
}
