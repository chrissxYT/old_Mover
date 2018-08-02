using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mover
{
    partial class GUI : Form
    {

        private Reflector refl;

        public GUI(Reflector refl)
        {
            InitializeComponent();
            this.refl = refl;
        }

        private static Panel GetPanel(int y, string fileName)
        {
            Panel p = new Panel();
            Label l = new Label();
            ProgressBar b = new ProgressBar();
            p.Controls.Add(b);
            p.Controls.Add(l);
            b.Location = new Point(10, 10);
            b.Size = new Size(680, 30);
            l.Location = new Point(10, 43);
            p.Location = new Point(10, y);
            p.Size = new Size(700, 56);
            l.Text = fileName;
            return p;
        }

        private int GetY()
        {
            return (64*this.Controls.Count)+10;
        }

        public void AddMove(string name)
        {
            Controls.Add(GetPanel(GetY(), name));
        }

        public void UpdatePer(string name, int per)
        {
            Panel panel = null;
            foreach(Panel p in Controls)
            {
                foreach(Label l in p.Controls)
                {
                    if(l.Text == name)
                    {
                        panel = p;
                    }
                }
            }
            if(panel == null)
            {
                if (refl.config.debug)
                    Util.Print("The panel is null(GUI:60)", refl, ConsoleColor.Red);
                return;
            }
            foreach (ProgressBar b in panel.Controls)
            {
                b.Value = per;
            }
        }

        public void RemoveMove(string name)
        {
            foreach (Panel p in Controls)
            {
                foreach (Label l in p.Controls)
                {
                    if (l.Text == name)
                    {
                        Controls.Remove(p);
                    }
                }
            }
        }
    }
}