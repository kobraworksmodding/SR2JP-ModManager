using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SR2JP_Mod_Manager
{
    public partial class Duplicates : Form
    {
        public Duplicates()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Duplicates_Load(object sender, EventArgs e)
        {
            string duplicatesText = FileChecker.GetDuplicateFilesSubfolders(Global.SR2Location + "//mods");

            richTextBox1.Text = duplicatesText;
            this.Text = "Conflict Checker: Conflicts: " + Global.numOfDupes;
            Global.numOfDupes = 0;
        }
    }
}
