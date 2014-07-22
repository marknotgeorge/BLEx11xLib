using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLEProgramming
{
    /// <summary>
    /// Interaction logic for ConsoleOutputArea.xaml
    /// </summary>
    public partial class ConsoleOutputArea : UserControl
    {
        public ConsoleOutputArea()
        {
            InitializeComponent();
        }

        public void WriteLine(string output)
        {
            if (!string.IsNullOrEmpty(output))
                OutputBox.Text += output + Environment.NewLine;
        }
    }
}
