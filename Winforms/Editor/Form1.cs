
#region Using Statements
using Monorail.Platform;
using SampleGame;
using SDL2;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

#endregion

public partial class Form1 : Form
{
    private Panel gamePanel;
 
   
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowPos(
        IntPtr handle,
        IntPtr handleAfter,
        int x,
        int y,
        int cx,
        int cy,
        uint flags
    );
    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr child, IntPtr newParent);
    [DllImport("user32.dll")]
    private static extern IntPtr ShowWindow(IntPtr handle, int command);


    IntPtr gameHwnd;

    static GameWindow gameWindow;

    public Form1()
    {
        InitializeComponent();


        ThreadPool.QueueUserWorkItem((c) =>
        {
            using (var window = new GameWindow("Test", 1280, 720))
            {
                gameWindow = window;
                window.RunGame(new MySampleGame());
            }
        });


        // This button just helps with some debugging
        Button button = new Button();
        button.Text = "Whatever";
        button.Location = new Point(
            (Size.Width / 2) - (button.Size.Width / 2),
            10
        );

        button.Click += (o, s) =>
        {

            var point = splitContainer1.Panel2.PointToScreen(new Point());
            var w = splitContainer1.Panel2.Width;
            var h = splitContainer1.Panel2.Height;

            // Move the SDL2 window to 0, 0
            SetWindowPos(
                gameWindow.Win32Ptr,
                this.Handle,
                point.X,
                point.Y,
                w,
                h,
                0x0000
            );

            ShowWindow(this.gameHwnd, 1); // SHOWNORMAL
        };

        splitContainer1.Panel2.Controls.Add(button);
    }


    private StatusStrip statusStrip1;
    private ToolStrip toolStrip1;

    private void InitializeComponent()
    {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 518);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1037, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1037, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Size = new System.Drawing.Size(1037, 493);
            this.splitContainer1.SplitterDistance = 345;
            this.splitContainer1.TabIndex = 2;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1037, 540);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load_2);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    private void Form1_Load_2(object sender, EventArgs e)
    {

    }

    private SplitContainer splitContainer1;

    private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
    {

    }
}