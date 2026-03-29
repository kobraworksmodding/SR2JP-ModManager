// - [ mainForm.cs ] -
// Created by Uzis: 3/29/2026

using System;
using System.IO;
using System.Linq;
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

        public void FillModList() // Fill mod list based on loose.txt in game files, if it exists.
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
                foreach (ListViewItem item in listView1.Items)
                {
                    try
                    {
                        if (item.SubItems[0].Text.StartsWith("--"))
                        {
                            item.Checked = false;
                            item.SubItems[0].Text = item.SubItems[0].Text.Substring(2);

                        }
                        else
                        {
                            item.Checked = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public void PerformStartupThings()
        { // Do startup stuff, put this in its own function because the code is pasted twice, cleaner to just reference a function twice rather than create 4 lines of code twice.
            Startup.CheckForJuicedPatch();
            bHasReadFirstMessage = true;
            FillModList();
            GameLocation.Text = $"SR2 Location: {Global.SR2Location}";
        }

        public void LookForSR2() // Look for saints row 2 directory.
        {
            if (findSR2dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = findSR2dialog.SelectedPath;
                //MessageBox.Show(filePath);
                if (File.Exists($"{filePath}\\sr2_pc.exe"))
                {
                    if (!Directory.Exists(Global.appDataPath)) // Check if our settings path exists, if not, create it.
                    {
                        Directory.CreateDirectory(Global.appDataPath);
                    }
                    File.WriteAllText($"{Global.appDataPath}\\settings.txt", filePath); // Create a simple settings.txt with our SR2 directory path that we can also edit later.
                    Global.SR2Location = filePath;
                }
                else
                {
                    MessageBox.Show("This location selected does not have the\nSaints Row 2 executable binary in it (sr2_pc.exe)\n\nPlease select the right installation location.", "SR2JP Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LookForSR2();
                }
            }
            else
            {
                Application.Exit();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the Form Name w/ Previous Git Hash.
            this.Text = $"Saints Row 2: Juiced Patch Mod Manager {{prc:{GitInfo.Hash}}}";
            // Initialise settings and such for the mod manager.
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
                            MessageBoxIcon.Information)) == DialogResult.OK);
                        {
                            LookForSR2();
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
