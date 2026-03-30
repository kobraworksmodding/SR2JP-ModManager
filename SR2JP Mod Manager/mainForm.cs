// - [ mainForm.cs ] -
// Created by Uzis: 3/29/2026

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;



namespace SR2JP_Mod_Manager
{

    public partial class mainForm : Form
    {
        bool bHasReadFirstMessage = false; // Just a hack if necessary to make it so the message doesn't pop twice at once.
        private static Mutex mutex = null;


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
                listView1.Columns.Add("Load Order - Installed Mods", 9999); // Big width so list doesn't look awkward on most screens when maximized.
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
                        if (item.SubItems[0].Text.Contains("\\")) {
                            item.SubItems[0].Text = Global.NormalizePathFmt(item.SubItems[0].Text);
                        }
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
        [STAThread]
        private void Form1_Load(object sender, EventArgs e)
        {
            const string appName = "SR2JP_MOD_MANAGER";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("There is already an instance of SR2JP Mod Manager running.", "SR2JP Mod Manager");
                Application.Exit();
                return;
            }

            // Initialize the Form Name w/ Previous Git Hash.
            this.Text = $"Saints Row 2: Juiced Patch Mod Manager {{prc:{GitInfo.Hash}}}";
            // Initialise settings and such for the mod manager.
            GameLocation.Hide();
            ExtractingBox.Hide();
            listView1.AllowDrop = true;
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
            if (FindMod.ShowDialog() == DialogResult.OK)
            {
                ExtractingBox.Show();
                var folders = ArchiveExtractor.Process(FindMod.FileName);

                foreach (var f in folders)
                {
                    string folderText = f.ToString();

                    var newItem = listView1.Items.Insert(0, folderText);

                    newItem.Checked = true;
                }

                ExtractingBox.Hide();
                SaveLoadOrder();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (FindMod.ShowDialog() == DialogResult.OK)
            {
                ExtractingBox.Show();
                var folders = ArchiveExtractor.Process(FindMod.FileName);

                foreach (var f in folders)
                {
                    string folderText = f.ToString();

                    var newItem = listView1.Items.Insert(0, folderText);

                    newItem.Checked = true;
                }

                ExtractingBox.Hide();
                SaveLoadOrder();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            string curItem = selectedItem.Text;

            if (curItem.Equals("mods", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("You cannot remove your root mods directory.", "SR2JP Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ask for confirmation
            if (MessageBox.Show(
                "Are you sure you would like to remove this mod entirely?\n\nThis cannot be undone. All contents of the chosen mod will be permanently deleted.",
                "SR2JP Mod Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            // Extract the mod root folder (everything under "mods/ModName")
            string modsRootPath = Path.Combine(Global.SR2Location, "mods");
            string modRootName = curItem.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
            string modFullPath = Path.Combine(modsRootPath, modRootName);

            // Delete folder from disk (if it exists)
            if (Directory.Exists(modFullPath))
            {
                Directory.Delete(modFullPath, true); // recursive delete
            }

            // Remove all ListView items that belong to this mod
            for (int i = listView1.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = listView1.Items[i];
                string itemText = item.Text.Replace('\\', '/'); // normalize slashes

                if (itemText.StartsWith($"mods/{modRootName}/", StringComparison.OrdinalIgnoreCase) ||
                    itemText.Equals($"mods/{modRootName}", StringComparison.OrdinalIgnoreCase))
                {
                    listView1.Items.RemoveAt(i);
                }
            }

            SaveLoadOrder();
        }

        private void TitleEditsMade()
        {
            this.Text = $"Saints Row 2: Juiced Patch Mod Manager {{prc:{GitInfo.Hash}}} -- (Unsaved Changes)";
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            selectedItem.Checked = false;
            TitleEditsMade();
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Uzis as part of a Kobraworks Project.\n\n- 2026 -", "SR2JP Mod Manager");
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            int index = selectedItem.Index;

            if (index <= 0) return;

            listView1.Items.RemoveAt(index);

            listView1.Items.Insert(index - 1, selectedItem);

            selectedItem.Selected = true;
            selectedItem.Focused = true;
            TitleEditsMade();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            int index = selectedItem.Index;

            if (index >= listView1.Items.Count - 1) return;

            listView1.Items.RemoveAt(index);

            listView1.Items.Insert(index + 1, selectedItem);

            selectedItem.Selected = true;
            selectedItem.Focused = true;
            TitleEditsMade();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            ListViewItem selectedItem = listView1.SelectedItems[0];
            selectedItem.Checked = true;
            TitleEditsMade();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {

        }

        private void listView1_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {

        }

        public void SaveLoadOrder()
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("Cannot save! There is nothing in the load order!", "SR2JP Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (!Directory.Exists(Path.Combine(Global.SR2Location, "mods")))
                {
                    Directory.CreateDirectory(Path.Combine(Global.SR2Location, "mods"));
                }
                string LooseText = Global.SR2Location + "/loose.txt";
                try
                {
                    File.WriteAllText(LooseText, string.Empty);
                    foreach (ListViewItem item in listView1.Items)
                    {
                        if (!item.Checked)
                        {
                            item.Text = "--" + item.Text;
                        }
                        using (StreamWriter writer = new StreamWriter(LooseText, true))
                        {
                            writer.WriteLine(item.Text);
                        }
                        if (!item.Checked)
                        {
                            item.Text = item.Text.Substring(2);
                        }
                    }
                    MessageBox.Show("Load Order has saved successfully.", "SR2JP Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Text = $"Saints Row 2: Juiced Patch Mod Manager {{prc:{GitInfo.Hash}}}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Oops, Looks like we ran into an error.\n\n" + ex.Message + "\n\nPlease report this to a SR2JP Mod Manager Developer.", "SR2JP Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
        private void saveLoadOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLoadOrder();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void conflictCheckerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Duplicates form2 = new Duplicates();
            form2.ShowDialog(); 
        }

        private void scanForModsNotInLoadOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get existing items in ListView
            int numFolders = 0;
            string modsPath = Path.Combine(Global.SR2Location, "mods");

            HashSet<string> existingItems = new HashSet<string>();

            // Normalize existing ListView items
            foreach (ListViewItem item in listView1.Items)
            {
                existingItems.Add(Global.NormalizePathFmt(item.Text));
            }

            // Scan directories
            foreach (string folder in Directory.GetDirectories(modsPath))
            {
                string folderName = Path.GetFileName(folder);
                string displayPath = $"mods/{folderName}";

                string normalizedPath = Global.NormalizePathFmt(displayPath);

                if (!existingItems.Contains(normalizedPath))
                {
                    numFolders++;
                    listView1.Items.Insert(0, displayPath);
                    existingItems.Add(normalizedPath);
                }
            }
            if (numFolders < 1)
            {
                MessageBox.Show("No missing mods from your \"mods\" folder! No need to panic.", "SR2JP Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Found " + numFolders + " missing mods.\n\n They've now been added to your load order.", "SR2JP Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                TitleEditsMade();
            }
        }

        private ListViewItem draggedItem;
        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            draggedItem = (ListViewItem)e.Item;
            listView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Move;
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            Point cp = listView1.PointToClient(new Point(e.X, e.Y));
            ListViewItem targetItem = listView1.GetItemAt(cp.X, cp.Y);

            if (targetItem == null || draggedItem == null)
                return;

            int targetIndex = targetItem.Index;
            int draggedIndex = draggedItem.Index;

            if (draggedIndex == targetIndex)
                return;

            if (cp.Y > targetItem.Bounds.Top + targetItem.Bounds.Height / 2)
            {
                targetIndex++;
            }
            // Remove and insert at new position
            listView1.Items.Remove(draggedItem);
            listView1.Items.Insert(targetIndex, draggedItem);
            draggedItem.Selected = true;
            TitleEditsMade();
        }

        private void listView1_DragOver(object sender, DragEventArgs e)
        {
            Point cp = listView1.PointToClient(new Point(e.X, e.Y));
            ListViewItem hoverItem = listView1.GetItemAt(cp.X, cp.Y);

            if (hoverItem != null)
            {
                hoverItem.Selected = true;
            }

        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                if (listView1.SelectedItems.Count == 0) return;

                ListViewItem selectedItem = listView1.SelectedItems[0];
                int index = selectedItem.Index;

                if (index <= 0) return;

                listView1.Items.RemoveAt(index);

                listView1.Items.Insert(index - 1, selectedItem);

                selectedItem.Selected = true;
                selectedItem.Focused = true;
                TitleEditsMade();
            }

            if (e.KeyCode == Keys.S)
            {
                if (listView1.SelectedItems.Count == 0) return;

                ListViewItem selectedItem = listView1.SelectedItems[0];
                int index = selectedItem.Index;

                if (index >= listView1.Items.Count - 1) return;

                listView1.Items.RemoveAt(index);

                listView1.Items.Insert(index + 1, selectedItem);

                selectedItem.Selected = true;
                selectedItem.Focused = true;
                TitleEditsMade();
            }

            if (e.KeyCode == Keys.Enter)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                if (selectedItem.Checked == true)
                {
                    if (listView1.SelectedItems.Count == 0) return;
                    selectedItem.Checked = false;
                    TitleEditsMade();
                }
                else
                {
                    if (listView1.SelectedItems.Count == 0) return;
                    selectedItem.Checked = true;
                    TitleEditsMade();
                }
            }
        }
    }
}
