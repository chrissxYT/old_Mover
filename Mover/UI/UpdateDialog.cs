using System.Windows.Forms;

namespace Mover
{
    public partial class UpdateDialog : Form
    {
        private string version;

        public UpdateDialog()
        {
            InitializeComponent();
        }

        public void SetVersion(ulong verNum)
        {
            char[] c = verNum.ToString().ToCharArray();
            string s = c[0].ToString();
            for (int i = 1; i < c.Length; i++)
            {
                s += c[i];
            }
            version = s;
        }

        public void SetCount(int c)
        {
            label1.Text = "Updating to v"+version+" in "+c+" seconds...";
        }
    }
}