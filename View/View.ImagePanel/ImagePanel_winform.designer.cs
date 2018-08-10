namespace Vision.View.ImagePanel
{
    partial class ImagePanel_winform
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.contextMenuStrip_Zoom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItem_Zoom = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Zoom5 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Zoom10 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Zoom20 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Zoom50 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Zoom100 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Zoom150 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Zoom200 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_ZoomFullSize = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_ZoomFitSize = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_Zoom.SuspendLayout();
            this.SuspendLayout();
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(37, 146);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(107, 17);
            this.hScrollBar1.TabIndex = 0;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScrollBar_Scroll);
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(127, 26);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 98);
            this.vScrollBar1.TabIndex = 1;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScrollBar_Scroll);
            // 
            // contextMenuStrip_Zoom
            // 
            this.contextMenuStrip_Zoom.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_Zoom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Zoom,
            this.MenuItem_ZoomFullSize,
            this.MenuItem_ZoomFitSize});
            this.contextMenuStrip_Zoom.Name = "contextMenuStrip_Zoom";
            this.contextMenuStrip_Zoom.Size = new System.Drawing.Size(176, 104);
            // 
            // MenuItem_Zoom
            // 
            this.MenuItem_Zoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Zoom5,
            this.MenuItem_Zoom10,
            this.MenuItem_Zoom20,
            this.MenuItem_Zoom50,
            this.MenuItem_Zoom100,
            this.MenuItem_Zoom150,
            this.MenuItem_Zoom200});
            this.MenuItem_Zoom.Name = "MenuItem_Zoom";
            this.MenuItem_Zoom.Size = new System.Drawing.Size(175, 24);
            this.MenuItem_Zoom.Text = "Zoom";
            // 
            // MenuItem_Zoom5
            // 
            this.MenuItem_Zoom5.Name = "MenuItem_Zoom5";
            this.MenuItem_Zoom5.Size = new System.Drawing.Size(181, 26);
            this.MenuItem_Zoom5.Text = "5%";
            this.MenuItem_Zoom5.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // MenuItem_Zoom10
            // 
            this.MenuItem_Zoom10.Name = "MenuItem_Zoom10";
            this.MenuItem_Zoom10.Size = new System.Drawing.Size(181, 26);
            this.MenuItem_Zoom10.Text = "10%";
            this.MenuItem_Zoom10.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // MenuItem_Zoom20
            // 
            this.MenuItem_Zoom20.Name = "MenuItem_Zoom20";
            this.MenuItem_Zoom20.Size = new System.Drawing.Size(181, 26);
            this.MenuItem_Zoom20.Text = "20%";
            this.MenuItem_Zoom20.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // MenuItem_Zoom50
            // 
            this.MenuItem_Zoom50.Name = "MenuItem_Zoom50";
            this.MenuItem_Zoom50.Size = new System.Drawing.Size(181, 26);
            this.MenuItem_Zoom50.Text = "50%";
            this.MenuItem_Zoom50.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // MenuItem_Zoom100
            // 
            this.MenuItem_Zoom100.Name = "MenuItem_Zoom100";
            this.MenuItem_Zoom100.Size = new System.Drawing.Size(181, 26);
            this.MenuItem_Zoom100.Text = "100%";
            this.MenuItem_Zoom100.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // MenuItem_Zoom150
            // 
            this.MenuItem_Zoom150.Name = "MenuItem_Zoom150";
            this.MenuItem_Zoom150.Size = new System.Drawing.Size(181, 26);
            this.MenuItem_Zoom150.Text = "150%";
            this.MenuItem_Zoom150.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // MenuItem_Zoom200
            // 
            this.MenuItem_Zoom200.Name = "MenuItem_Zoom200";
            this.MenuItem_Zoom200.Size = new System.Drawing.Size(181, 26);
            this.MenuItem_Zoom200.Text = "200%";
            this.MenuItem_Zoom200.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // MenuItem_ZoomFullSize
            // 
            this.MenuItem_ZoomFullSize.Name = "MenuItem_ZoomFullSize";
            this.MenuItem_ZoomFullSize.Size = new System.Drawing.Size(175, 24);
            this.MenuItem_ZoomFullSize.Text = "Full Size";
            this.MenuItem_ZoomFullSize.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // MenuItem_ZoomFitSize
            // 
            this.MenuItem_ZoomFitSize.Name = "MenuItem_ZoomFitSize";
            this.MenuItem_ZoomFitSize.Size = new System.Drawing.Size(175, 24);
            this.MenuItem_ZoomFitSize.Text = "Fit Size";
            this.MenuItem_ZoomFitSize.Click += new System.EventHandler(this.MenuItem_Zoom_Click);
            // 
            // ImagePanel_winform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ContextMenuStrip = this.contextMenuStrip_Zoom;
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.hScrollBar1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ImagePanel_winform";
            this.Size = new System.Drawing.Size(193, 195);
            this.contextMenuStrip_Zoom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Zoom;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Zoom;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Zoom200;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Zoom150;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Zoom100;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_ZoomFitSize;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Zoom50;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_ZoomFullSize;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Zoom5;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Zoom10;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Zoom20;
    }
}
