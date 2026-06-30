namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
  partial class HelpDlg
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if( disposing && ( components != null ) )
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( HelpDlg ) );
      this.richTextBox1 = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // richTextBox1
      // 
      this.richTextBox1.Anchor = ( ( System.Windows.Forms.AnchorStyles ) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                  | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.richTextBox1.BackColor = System.Drawing.Color.White;
      this.richTextBox1.ForeColor = System.Drawing.Color.Black;
      this.richTextBox1.Location = new System.Drawing.Point( 1, 1 );
      this.richTextBox1.Name = "richTextBox1";
      this.richTextBox1.ReadOnly = true;
      this.richTextBox1.Size = new System.Drawing.Size( 393, 273 );
      this.richTextBox1.TabIndex = 0;
      this.richTextBox1.Text = "";
      // 
      // HelpDlg
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 392, 273 );
      this.Controls.Add( this.richTextBox1 );
      this.Icon = ( ( System.Drawing.Icon ) ( resources.GetObject( "$this.Icon" ) ) );
      this.Name = "HelpDlg";
      this.Text = "Revit String Search Help";
      this.Load += new System.EventHandler( this.HelpDlg_Load );
      this.ResumeLayout( false );

    }

    #endregion

    private System.Windows.Forms.RichTextBox richTextBox1;
  }
}