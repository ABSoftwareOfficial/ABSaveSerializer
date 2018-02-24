using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABSaveSerializer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TestClass test = new TestClass();
            textBox1.Text = ABSoftware.ABSave.ABSaveConvert.SerializeABSave(test);
        }
    }

    public class TestClass
    {
        public string str = "Oh, Hello!";
        public int i = 365;

        public NextClass nextCl = new NextClass();

        public List<string> lstOfStr = new List<string>()
        {
            "FirstStr",
            "SecondStr"
        };        
    }

    public class NextClass
    {
        public bool yoy = false;
    }
}
