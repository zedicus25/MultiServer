using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        
        private OpenFileDialog _fileDialog;
        private string _selectedFile = String.Empty;
        private string _userMessage = String.Empty;
        private ClientNetwork _network;
        public ClientForm()
        {
            _fileDialog = new OpenFileDialog();
            _fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _fileDialog.Title = "Select file";
            _network = new ClientNetwork();
            InitializeComponent();
            
        }

        private void SelectedFileBtnClick(object sender, EventArgs e)
        {
            if(_fileDialog.ShowDialog() == DialogResult.OK)
            {
                _selectedFile = _fileDialog.FileName;
                selectedFileL.Text = _selectedFile;
            }
        }

        private void messageTB_TextChanged(object sender, EventArgs e)
        {
            _userMessage = messageTB.Text;
        }

        private void sendMessageBtn_Click(object sender, EventArgs e)
        {
            if (!_userMessage.Equals(String.Empty))
            {
                _network.SendToServer(_userMessage);
            }
        }

        private void sendFileBtn_Click(object sender, EventArgs e)
        {
            if (_selectedFile.Equals(String.Empty))
                return;
            _network.SendToServer(_selectedFile, true);
        }
    }
}
