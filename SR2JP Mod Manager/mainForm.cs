// - [ mainForm.cs ] -
// Created by Uzis: 3/29/2026

using System;
using System.IO;
using System.Windows.Forms;


namespace SR2JP_Mod_Manager
{
    public partial class mainForm : Form
    {
        bool bHasReadFirstMessage = false; // Just a hack if necessary to make it so the message doesn't pop twice at once.
        public mainForm()
        {
            InitializeComponent();
        }

        public void FillModList()
        {
            string looseTxt = Path.Combine(Global.SR2Location, "loose.txt");
            if (File.Exists(looseTxt))
            {
                string[] looseMods = File.ReadAllLines(looseTxt);
                listView1.Columns.Add("Mods", listView1.ClientSize.Width);
                foreach (string mod in looseMods)
                {
                    if (!string.IsNullOrEmpty(mod))
                    {
                        listView1.Items.Add(mod);
                    }
                }
            }
        }

        public void PerformStartupThings()
        {
            Startup.CheckForJuicedPatch();
            bHasReadFirstMessage = true;
            FillModList();
            GameLocation.Text = $"SR2 Location: {Global.SR2Location}";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the Form Name w/ Previous Git Hash.
            this.Text = $"Saints Row 2: Juiced Patch Mod Manager {{prc:{GitInfo.Hash}}}";
            GameLocation.Hide();
            if (Directory.Exists(Global.appDataPath) && File.Exists($"{Global.appDataPath}\\settings.txt"))
            {
                Global.SR2Location = File.ReadAllText($"{Global.appDataPath}\\settings.txt");
                if (!string.IsNullOrEmpty(Global.SR2Location))
                {
                    PerformStartupThings();
                }
            }
            else
            {
                if (bHasReadFirstMessage == false)
                {
                    if (string.IsNullOrEmpty(Global.SR2Location))
                    {
                        if ((MessageBox.Show("To get started, first off we need to know where your Saints Row 2 Game Directory is located!\n\n" +
                            "If you're using the Steam version its typically in\n\n" +
                            "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Saints Row 2",
                            "Welcome to SR2JP Mod Manager",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)) == DialogResult.OK) ;
                        {
                            if (findSR2dialog.ShowDialog() == DialogResult.OK)
                            {
                                string filePath = findSR2dialog.SelectedPath;
                                //MessageBox.Show(filePath);
                                if (!Directory.Exists(Global.appDataPath)) // Check if our settings path exists, if not, create it.
                                {
                                    Directory.CreateDirectory(Global.appDataPath);
                                }
                                File.WriteAllText($"{Global.appDataPath}\\settings.txt", filePath); // Create a simple settings.txt with our SR2 directory path that we can also edit later.
                                Global.SR2Location = filePath;
                            }
                        }
                        PerformStartupThings();
                    }
                }
            }
            GameLocation.Show();

        }

        private void importModToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }
    }
}
