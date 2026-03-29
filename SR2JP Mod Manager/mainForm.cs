// - [ mainForm.cs ] -
// Created by Uzis: 3/29/2026

using System;
using System.Windows.Forms;


namespace SR2JP_Mod_Manager
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the Form Name w/ Git Hash.
            this.Text = "Saints Row 2: Juiced Patch Mod Manager {prc:" + GitInfo.Hash + "}";
        }
    }
}
