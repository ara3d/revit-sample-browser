using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>
/// A simple custom-drawn flat progress bar.
/// </summary>
internal sealed class ModernProgressBar : Control
{
    public int Maximum { get; set; } = 100;

    public int Value
    {
        get;
        set { field = value; Invalidate(); }
    }
    public Color AccentColor { get; set; } = Color.FromArgb(0, 120, 212);

    public ModernProgressBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);

        Height = 14;
        BackColor = Color.FromArgb(235, 235, 235);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.Clear(BackColor);

        var rect = ClientRectangle;
        rect.Inflate(-1, -1);

        // Background track
        using (SolidBrush track = new(Color.FromArgb(235, 235, 235)))
            e.Graphics.FillRectangle(track, rect);

        // Progress
        var max = Math.Max(1, Maximum);
        var v = Math.Max(0, Math.Min(Value, max));
        var w = (int)Math.Round(rect.Width * (v / (double)max));
        if (w > 0)
        {
            Rectangle prog = new(rect.X, rect.Y, w, rect.Height);
            using SolidBrush brush = new(AccentColor);
            e.Graphics.FillRectangle(brush, prog);
        }

        // Subtle border
        using Pen pen = new(Color.FromArgb(210, 210, 210));
        e.Graphics.DrawRectangle(pen, rect);
    }
}