using ODModules;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace Component_Tests {
    public partial class Form1 : Form {
        List<StatusBlock> Block = new List<StatusBlock>();
        public Form1() {
            InitializeComponent();
            UserTest userTest1 = new UserTest();
            ContextMenuHost = new TemplateContextMenuHost(userTest1, this);

            Block.Add(new StatusBlock("Test", Status.Pending));
            Block.Add(new StatusBlock("Hello", Status.Off));
            //navigator1.LinkedList = Block;
        }
        TemplateContextMenuHost ContextMenuHost;
        private void Form1_Load(object sender, EventArgs e) {
            TrimBits("11101010100001");
            Debug.Print(byteValue);
        }
        string byteValue = "0";
        int bits = 4;
        private void TrimBits(string Input) {
            if (Input.Length > bits) {
                int Overflow = Input.Length - bits;
                byteValue = Input.Substring(Overflow, bits); ;

                //for (int i = Overflow; i < Input.Length; i++) {
                //    byteValue += Input[i];
                //}
            }
            else {
                byteValue = Input;
            }
        }
        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        private void tabHeader1_CloseButtonClicked(object sender, int Index) {
            this.Text = Index.ToString();
        }

        private void button1_Click(object sender, EventArgs e) {
            ListItem Li = new ListItem("TEST ITEM");
            Li.SubItems.Add(new ListSubItem("Hello"));
            //listControl1.LineInsertAtSelected(Li, false, false);
        }

        private void button2_Click(object sender, EventArgs e) {
            ContextMenuHost.ShowAndCentre(this);
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            bitToggle1.Value = textBox1.Text;
        }

        private void propertyGrid1_Click(object sender, EventArgs e) {

        }

        private void selectFORWARDToolStripMenuItem_Click(object sender, EventArgs e) {
            listControl1.SelectNextDropDown();
        }

        private void listControl1_DropDownClicked(object sender, DropDownClickedEventArgs e) {
            if (e.ParentItem == null) { return; }
            e.ParentItem[e.Column].Text = "aaaa";
        }
    }
    public class StatusBlock {
        string text = "";
        public string Text {
            get { return text; }
            set { text = value; }
        }
        Status currentStatus = Status.Off;
        public Status CurrentStatus {
            get { return currentStatus; }
            set { currentStatus = value; }
        }
        public StatusBlock(string Input, Status Stat) {
            text = Input;
            currentStatus = Stat;

        }
    }
    public enum Status {
        Off = 0x00,
        Pending = 0x01,
        On = 0x02
    }
}