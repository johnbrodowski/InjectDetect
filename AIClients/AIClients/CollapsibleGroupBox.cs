using System.ComponentModel;

namespace AIClients;

/// <summary>
/// A GroupBox whose content collapses to just its title bar when the header is clicked.
/// An arrow glyph in the top-right corner shows the current state.
/// </summary>
public class CollapsibleGroupBox : GroupBox
{
    // ── Constants ─────────────────────────────────────────────────────────────
    private const int HeaderHeight = 22;   // px — height when collapsed
    private const int ClickZone    = 24;   // px from top that counts as a header click

    // ── State ─────────────────────────────────────────────────────────────────
    private int  _expandedHeight = -1;
    private bool _collapsed;

    // ── Public API ────────────────────────────────────────────────────────────
    [DefaultValue(false)]
    public bool Collapsed
    {
        get => _collapsed;
        set
        {
            if (_collapsed == value) return;
            _collapsed = value;

            SuspendLayout();
            if (_collapsed)
            {
                _expandedHeight = Height;
                foreach (Control c in Controls) c.Visible = false;
                Height = HeaderHeight;
            }
            else
            {
                if (_expandedHeight > HeaderHeight) Height = _expandedHeight;
                foreach (Control c in Controls) c.Visible = true;
            }
            ResumeLayout(false);

            CollapsedChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
    }

    /// <summary>Fired after every collapse / expand transition.</summary>
    public event EventHandler? CollapsedChanged;

    // ── Header click ──────────────────────────────────────────────────────────
    protected override void OnMouseClick(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && e.Y <= ClickZone)
            Collapsed = !Collapsed;
        else
            base.OnMouseClick(e);
    }

    // ── Cursor hint ───────────────────────────────────────────────────────────
    protected override void OnMouseMove(MouseEventArgs e)
    {
        Cursor = e.Y <= ClickZone ? Cursors.Hand : Cursors.Default;
        base.OnMouseMove(e);
    }

    // ── Arrow glyph in title bar ──────────────────────────────────────────────
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        string arrow = _collapsed ? "▶" : "▼";
        using var font = new Font(Font.FontFamily, 7.5f, FontStyle.Regular);
        var sz = TextRenderer.MeasureText(arrow, font);
        TextRenderer.DrawText(
            e.Graphics, arrow, font,
            new Point(Width - sz.Width - 6, 4),
            SystemColors.ControlDark);
    }
}
