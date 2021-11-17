using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PJT_Adega
{
    public partial class Form2 : Form
    {
        bool mover = false;
        Point posicao_inicial;
        Thread nt;
        string connect = System.Net.Dns.GetHostName().ToUpper() + @"\SQLEXPRESS";
        string connect1 = System.Net.Dns.GetHostName().ToUpper();
        string retorno = TestarConexao();
        //define o caminho para o backup/restore
        private string DBpath = @"C:\Backup";
        string script;
        SqlConnection cn = new SqlConnection("Data Source = " + System.Net.Dns.GetHostName().ToUpper() + @"\SQLEXPRESS; Integrated Security = True; Initial Catalog = ADEGA");
        SqlConnection cn1 = new SqlConnection("Data Source = " + System.Net.Dns.GetHostName().ToUpper() + " ; Integrated Security = True; Initial Catalog = ADEGA");
        //define o objeto do tipo Server
        private static Server servidor;
        public static string TestarConexao()
        {
            try
            {
                SqlConnection cn = new SqlConnection("Data Source = " + System.Net.Dns.GetHostName().ToUpper() + @"\SQLEXPRESS; Integrated Security = True");
                cn.Open();
                string conectar = string.Empty;
                conectar = System.Net.Dns.GetHostName().ToUpper() + @"\SQLEXPRESS";
                return conectar;
            }
            catch (Exception)
            {
                string conectar = string.Empty;
                conectar = System.Net.Dns.GetHostName().ToUpper();
                return conectar;
            }
        }

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (cn.State == ConnectionState.Open)
                cn.Close();
            if (cn1.State == ConnectionState.Open)
                cn1.Close();
            string nomeServidor = string.Empty;
            try
            {
                if(retorno == connect)
                {
                    script = @"PRINT 'TESTE'";
                    SqlCommand cm = new SqlCommand(script, cn);
                    cn.Open();
                    cm.ExecuteNonQuery();
                    nomeServidor = System.Net.Dns.GetHostName().ToUpper() + @"\SQLEXPRESS";
                }
                else
                {
                    script = @"PRINT 'TESTE'";
                    SqlCommand cm = new SqlCommand(script, cn1);
                    cn1.Open();
                    cm.ExecuteNonQuery();
                    nomeServidor = System.Net.Dns.GetHostName().ToUpper();
                }
            }
            catch(SqlException ex)
            {
                MessageBox.Show("Erro: " + ex);
            }
            finally
            {
                cn.Close();
                cn1.Close();
            }
            textServidor.Text = nomeServidor;
            txtDatabase.Text = "ADEGA";
            this.Cursor = Cursors.Default;
            WindowState = FormWindowState.Normal;
            textServidor.Enabled = false;
            txtDatabase.Enabled = false;
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            // Cria uma nova conexão com o servidor selecionado
            ServerConnection srvConn = new ServerConnection(textServidor.Text.ToString());
            servidor = new Server(srvConn);

            if (!Directory.Exists(DBpath))
                Directory.CreateDirectory(DBpath);

            //se o objeto servidor for diferente de null temos uma conexão
            if (servidor != null)
            {
                try
                {
                    //desabilita os botões
                    btnBackup.Enabled = false;
                    btnRestore.Enabled = false;

                    //Este codigo é usado se você já criou o arquivo de backup.
                    File.Delete(DBpath + "\\backup.bak");
                    this.Cursor = Cursors.WaitCursor;
                    // se o usuário escolheu um caminho onde salvar o backup
                    // Cria uma nova operação de backup
                    Backup bkpDatabase = new Backup();
                    // Define o tipo de backup type para o database
                    bkpDatabase.Action = BackupActionType.Database;
                    // Define o database que desejamos fazer o backup
                    bkpDatabase.Database = txtDatabase.Text;
                    // Define o dispositivo do backup para : file
                    BackupDeviceItem bkpDevice = new BackupDeviceItem(DBpath + "\\Backup.bak", DeviceType.File);
                    // Adiciona o dispositivo de backup ao backup
                    bkpDatabase.Devices.Add(bkpDevice);
                    // Realiza o backup
                    bkpDatabase.SqlBackup(servidor);
                    MessageBox.Show("Backup do Database " + txtDatabase.Text + " criado com sucesso no diretório " + DBpath, "Servidor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception x)
                {
                    MessageBox.Show("ERRO: Ocorreu um erro durante o BACKUP do DataBase" + x, "Erro no Servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Arrow;
                    //habilita os botões
                    btnBackup.Enabled = true;
                    btnRestore.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("ERRO: Não foi estabelecida uma conexão com o SQL Server", "Servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (cn.State == ConnectionState.Open)
                cn.Close();
            if (cn1.State == ConnectionState.Open)
                cn1.Close();
            //desabilita os botões
            btnBackup.Enabled = false;
            btnRestore.Enabled = false;

            this.Cursor = Cursors.WaitCursor;


            // Cria uma nova conexão com o servidor selecionado
            ServerConnection srvConn = new ServerConnection(textServidor.Text.ToString());
            servidor = new Server(srvConn);

            if (!Directory.Exists(DBpath))
                Directory.CreateDirectory(DBpath);

            if (!File.Exists(DBpath + "\\Backup.bak"))
            {
                MessageBox.Show("Não encontrado arquivo de Backup para restauração.");
                return;
            }

            try
            {
                if(retorno == connect)
                {
                    cn.Open();
                    script = "DELETE FROM PRODUTO;";
                    script += "USE MASTER;";
                    script += "ALTER DATABASE " + txtDatabase.Text + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    SqlCommand cm = new SqlCommand(script, cn);
                    cm.ExecuteNonQuery();
                    script = string.Empty;
                    script = "USE MASTER;";
                    script += "RESTORE DATABASE ADEGA FROM DISK = '" + DBpath + "//Backup.bak' WITH MOVE 'ADEGA' TO '" + DBpath + "//ADEGA.mdf', MOVE 'ADEGA_LOG' TO'" + DBpath + "//Adega_log.ldf', REPLACE";
                    cm = new SqlCommand(script, cn);
                    cm.CommandTimeout = 500;
                    cm.ExecuteNonQuery();
                    MessageBox.Show("Restauração realizada");
                    script = string.Empty;
                    script = "USE MASTER;";
                    script += "ALTER DATABASE " + txtDatabase.Text + " SET MULTI_USER WITH ROLLBACK IMMEDIATE";
                    cm = new SqlCommand(script, cn);
                    cm.ExecuteNonQuery();
                }
                else
                {
                    cn1.Open();
                    script = "DELETE FROM PRODUTO;";
                    script += "USE MASTER;";
                    script += "ALTER DATABASE " + txtDatabase.Text + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    SqlCommand cm = new SqlCommand(script, cn1);
                    cm.ExecuteNonQuery();
                    script = string.Empty;
                    script = "USE MASTER;";
                    script += "RESTORE DATABASE ADEGA FROM DISK = '" + DBpath + "//Backup.bak' WITH MOVE 'ADEGA' TO '" + DBpath + "//ADEGA.mdf', MOVE 'ADEGA_LOG' TO'" + DBpath + "//Adega_log.ldf', REPLACE";
                    cm = new SqlCommand(script, cn1);
                    cm.CommandTimeout = 500;
                    cm.ExecuteNonQuery();
                    MessageBox.Show("Restauração realizada");
                    script = string.Empty;
                    script = "USE MASTER;";
                    script += "ALTER DATABASE " + txtDatabase.Text + " SET MULTI_USER WITH ROLLBACK IMMEDIATE";
                    cm = new SqlCommand(script, cn1);
                    cm.ExecuteNonQuery();
                }
                
            }
            catch(SqlException ex)
            {
                MessageBox.Show("Erro: " + ex);
            }
            finally
            {
                cn.Close();
                cn1.Close();
                this.Cursor = Cursors.Arrow;
                //habilita os botões
                btnBackup.Enabled = true;
                btnRestore.Enabled = true;
            }
        }

        private void btnTelaBackRes_Click(object sender, EventArgs e)
        {
            this.Close();
            nt = new Thread(novoForm2);
            nt.SetApartmentState(ApartmentState.STA);
            nt.Start();
        }

        private void novoForm2()
        {
            Application.Run(new Form2());
        }

        private void btnTelaMain_Click(object sender, EventArgs e)
        {
            this.Close();
            nt = new Thread(novoForm);
            nt.SetApartmentState(ApartmentState.STA);
            nt.Start();
        }

        private void novoForm()
        {
            Application.Run(new Form1());
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mover = true;
            posicao_inicial = new Point(e.X, e.Y);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mover = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mover)
            {
                Point novo = PointToScreen(e.Location);
                Location = new Point(novo.X - posicao_inicial.X, novo.Y - posicao_inicial.Y);
            }
        }

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
