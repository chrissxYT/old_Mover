using System.Drawing;
using System.Windows.Forms;

namespace Mover
{
    public partial class MovingDialog : Form
    {
        public static readonly new Size Size = new Size(549, 119);

        public MovingDialog(int x, int y)
        {
            InitializeComponent();
            this.Location = new Point(x, y);
        }

        public void setProgressBarPercentage(int per)
        {
            this.progressBar1.Value = per;
        }

        public void setFilename(string filename)
        {
            this.filename.Text = filename;
        }
    }
}
