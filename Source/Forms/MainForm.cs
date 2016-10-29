﻿using System;
using System.Drawing;
using System.Windows.Forms;

/* Lazy todo list
- Dialog.Prompt() should return string
- Make MainForm sizeable (fixed scale)
- Dialog.SayStatic(string) + Dialog.Update(string)
- Animation.Play(string[])
  - MaxFrames * Interval
*/

namespace FuckingClippy
{
    public partial class MainForm : TransparentForm
    {
        Timer tmrIdleSay = new Timer();
        Timer tmrIdleAni = new Timer();
        
        public MainForm() : base()
        {
            InitializeComponent();

            //TODO: Uncomment when translations are ready.
            //InitiateCulture();

            Utils.Log("MainForm initiated");

            // Use main icon.
            ShowIcon = false;

            Character.Initialize(this, picAssistant);
            
            picAssistant.Dock = DockStyle.Fill;

            tmrIdleAni.Interval = 120000; // 2 minutes
            tmrIdleSay.Interval = 300000; // 5 minutes

            // Grab the current Screen info and locate the character
            // at the bottom right with a margin of 30px.
            {
                Screen sc = Screen.FromControl(this);
                Location =
                    new Point(sc.WorkingArea.Width - (Width + 30),
                        sc.WorkingArea.Height - (Height + 30));
            }

            picAssistant.MouseDown += Assistant_MouseDown;
            picAssistant.MouseUp += Assistant_MouseUp;
            picAssistant.MouseMove += Assistant_MouseMove;

            tmrIdleAni.Tick += TmrIdleAni_Tick;
            tmrIdleSay.Tick += TmrIdleSay_Tick;

            TopMost = true; // Only hell now. :-)
#if DEBUG
            {
                ToolStripItem[] DebugItems = new ToolStripItem[2];

                DebugItems[0] = new ToolStripMenuItem();
                DebugItems[0].Text = "[Debug] Prompt";
                DebugItems[0].Click += (s, e) =>
                {
                    Character.Prompt();
                };

                DebugItems[1] = new ToolStripMenuItem();
                DebugItems[1].Text = "[Debug] Say (Random)";
                DebugItems[1].Click += (s, e) =>
                {
                    Character.SayRandom();
                };

                cmsCharacter.Items.AddRange(DebugItems);
            }
#endif
            cmsCharacter.ResumeLayout(false);
            ResumeLayout(true);
            
            Character.PlayAnimation(Animation.FadeIn);
            tmrIdleAni.Start();
            tmrIdleSay.Start();
        }

        /// <summary>
        /// Hey
        /// </summary>
        public void Exit()
        {
            Character.PlayAnimation(Animation.FadeOut);

            // Dirty solution
            Timer a = new Timer();
            a.Interval = 3 * 125; // temporary
            a.Tick += (s, e) => { Close(); };
            a.Start();
        }

#region Idle timers
        void TmrIdleSay_Tick(object sender, EventArgs e)
        {
            Character.SayRandom();
        }

        void TmrIdleAni_Tick(object sender, EventArgs e)
        {
            Character.PlayRandomAnimation();
        }
#endregion

#region Mouse events
        bool FormDown, IsPrompting;
        Point LastMouseLocation, LastFormLocation;

        void Assistant_MouseMove(object sender, MouseEventArgs e)
        {
            if (FormDown)
            {
                Location =
                    new Point((Location.X - LastMouseLocation.X) + e.X,
                    (Location.Y - LastMouseLocation.Y) + e.Y);

                Update();
            }
        }

        void Assistant_MouseUp(object sender, MouseEventArgs e)
        {
            FormDown = false;

            if (e.Button == MouseButtons.Left)
                if (LastFormLocation.X == Location.X &&
                    LastFormLocation.Y == Location.Y &&
                    !IsPrompting)
                {
                    IsPrompting = true;
                    Character.Prompt();
                }
        }

        void Assistant_MouseDown(object sender, MouseEventArgs e)
        {
            FormDown = true;
            IsPrompting = false;
            LastMouseLocation = e.Location;
            LastFormLocation = Location;
        }
#endregion

#region Context menu events
        void cmsiHide_Click(object sender, EventArgs e)
        {
            Exit();
        }

        void csmiOptions_Click(object sender, EventArgs e)
        { // Settings -> Options tab
            new SettingsForm(SettingsForm.Tab.Options).ShowDialog();
        }

        void cmsiChooseAssistant_Click(object sender, EventArgs e)
        { // Settings -> Assistant tab
            new SettingsForm(SettingsForm.Tab.Assistant).ShowDialog();
        }

        void cmsiAnimate_Click(object sender, EventArgs e)
        {
            Character.PlayRandomAnimation();
        }
#endregion
    }
}
