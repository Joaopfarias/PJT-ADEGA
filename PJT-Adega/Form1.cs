using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PJT_Adega
{
    public partial class Form1 : Form
    {
        string vlUnit, vlAvulso;
        Thread nt;
        string connect = System.Net.Dns.GetHostName().ToUpper() + @"\SQLEXPRESS";
        DateTime hoje = DateTime.Today;
        string connect1 = System.Net.Dns.GetHostName().ToUpper();
        public string strSelect = "SELECT IDPRODUTO, NOMEPRODUTO, QUANTIDADE, SAIDAMENSAL, PRECOUNIT, PRECOAVULSO FROM PRODUTO WITH(NOLOCK)";
        SqlConnection cn = new SqlConnection("Data Source = " + System.Net.Dns.GetHostName().ToUpper() + @"\SQLEXPRESS; Integrated Security = True");
        SqlConnection cn1 = new SqlConnection("Data Source = " + System.Net.Dns.GetHostName().ToUpper() + " ; Integrated Security = True");
        SqlConnection cn2 = new SqlConnection("Data Source = " + System.Net.Dns.GetHostName().ToUpper() + @"\SQLEXPRESS; Integrated Security = True; Initial Catalog = ADEGA");
        SqlConnection cn3 = new SqlConnection("Data Source = " + System.Net.Dns.GetHostName().ToUpper() + " ; Integrated Security = True; Initial Catalog = ADEGA");
        SqlCommand objCommand = new SqlCommand();
        string retorno = TestarConexao();
        string script;
        bool mover = false;
        Point posicao_inicial;
        DataTable dt = new DataTable();

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

        public Form1()
        {
            InitializeComponent();
            CarregarColunas();
            LimparLinhas();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (cn.State == ConnectionState.Open)
                cn.Close();
            if (cn1.State == ConnectionState.Open)
                cn1.Close();
            if (cn2.State == ConnectionState.Open)
                cn2.Close();
            if (cn3.State == ConnectionState.Open)
                cn3.Close();
            try
            {
                if (retorno == connect)
                {
                    script = @"SELECT * FROM sys.databases WHERE name = 'ADEGA'";
                    DataSet dt = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(script, cn);
                    cn.Open();
                    da.Fill(dt);
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                        script = @"CREATE DATABASE ADEGA";
                        SqlCommand cm1 = new SqlCommand(script, cn);
                        cm1.ExecuteNonQuery();
                    }
                }
                else
                {
                    script = @"SELECT * FROM sys.databases WHERE name = 'ADEGA'";
                    DataSet dt = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(script, cn1);
                    cn1.Open();
                    da.Fill(dt);
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                        script = @"CREATE DATABASE ADEGA";
                        SqlCommand cm = new SqlCommand(script, cn1);
                        cm.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex);
            }

            finally
            {
                cn.Close();
                cn1.Close();
            }

            try
            {
                if (retorno == connect)
                {
                    script = @"select * from sysobjects where name='Produto' and xtype='U'";
                    DataSet dt = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(script, cn2);
                    cn2.Open();
                    da.Fill(dt);
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                        string script = @"CREATE TABLE PRODUTO(IDPRODUTO INT IDENTITY(1,1) PRIMARY KEY, NOMEPRODUTO VARCHAR(120) NOT NULL, QUANTIDADE INT NOT NULL, SAIDAMENSAL INT, PRECOUNIT FLOAT, PRECOAVULSO FLOAT)";
                        SqlCommand cm1 = new SqlCommand(script, cn2);
                        cm1.ExecuteNonQuery();
                    }
                }
                else
                {
                    script = @"select * from sysobjects where name='Produto' and xtype='U'";
                    DataSet dt = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(script, cn3);
                    cn3.Open();
                    da.Fill(dt);
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                        string script = @"CREATE TABLE PRODUTO(IDPRODUTO INT IDENTITY(1,1) PRIMARY KEY, NOMEPRODUTO VARCHAR(120) NOT NULL, QUANTIDADE INT NOT NULL, SAIDAMENSAL INT, PRECOUNIT FLOAT, PRECOAVULSO FLOAT)";
                        SqlCommand cm1 = new SqlCommand(script, cn3);
                        cm1.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex1)
            {
                MessageBox.Show("Erro: " + ex1);
            }

            finally
            {
                cn2.Close();
                cn3.Close();
            }

            try
            {
                if (retorno == connect)
                {
                    script = @"select * from sysobjects where name='SISTEMA' and xtype='U'";
                    DataSet dt = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(script, cn2);
                    cn2.Open();
                    da.Fill(dt);
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                        string script = @"CREATE TABLE SISTEMA(RESETOUTMENSAL INT)";
                        SqlCommand cm1 = new SqlCommand(script, cn2);
                        cm1.ExecuteNonQuery();
                    }
                }
                else
                {
                    script = @"select * from sysobjects where name='SISTEMA' and xtype='U'";
                    DataSet dt = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(script, cn3);
                    cn3.Open();
                    da.Fill(dt);
                    if (dt.Tables[0].Rows.Count == 0)
                    {
                        string script = @"CREATE TABLE SISTEMA(RESETOUTMENSAL INT)";
                        SqlCommand cm1 = new SqlCommand(script, cn3);
                        cm1.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex1)
            {
                MessageBox.Show("Erro: " + ex1);
            }

            finally
            {
                cn2.Close();
                cn3.Close();
            }

            try
            {
                string diaHoje = hoje.ToString().Substring(0, 2);
                if (diaHoje == "01" || diaHoje == "1/")
                {
                    if (retorno == connect)
                    {
                        script = @"select * from SISTEMA";
                        DataSet dt = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(script, cn2);
                        cn2.Open();
                        da.Fill(dt);
                        if (dt.Tables[0].Rows.Count == 0)
                        {
                            string script = @"INSERT INTO SISTEMA(RESETOUTMENSAL) VALUES(1)";
                            SqlCommand cm1 = new SqlCommand(script, cn2);
                            cm1.ExecuteNonQuery();
                            script = @"UPDATE PRODUTO SET SAIDAMENSAL = 0";
                            cm1 = new SqlCommand(script, cn2);
                            cm1.ExecuteNonQuery();

                        }
                    }
                    else
                    {
                        script = @"select * from SISTEMA";
                        DataSet dt = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(script, cn3);
                        cn3.Open();
                        da.Fill(dt);
                        if (dt.Tables[0].Rows.Count == 0)
                        {
                            string script = @"INSERT INTO SISTEMA(RESETOUTMENSAL) VALUES(1)";
                            SqlCommand cm1 = new SqlCommand(script, cn3);
                            cm1.ExecuteNonQuery();
                            script = @"UPDATE PRODUTO SET SAIDAMENSAL = 0";
                            cm1 = new SqlCommand(script, cn3);
                            cm1.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    if (retorno == connect)
                    {
                        cn2.Open();
                        string script = @"DELETE FROM SISTEMA";
                        SqlCommand cm1 = new SqlCommand(script, cn2);
                        cm1.ExecuteNonQuery();
                    }
                    else
                    {
                        cn3.Open();
                        string script = @"DELETE FROM SISTEMA";
                        SqlCommand cm1 = new SqlCommand(script, cn3);
                        cm1.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro: " + erro);
            }
            finally
            {
                cn2.Close();
                cn3.Close();
            }
        }
        private void FiltrarGrade()
        {

            if (Cbx_order.SelectedItem == null)
            {
                MessageBox.Show("Selecionar alguma opção de pesquisa");
                return;
            }
            if (Cbx_order.SelectedItem != null && !String.IsNullOrEmpty(Txt_pesquisa.Text))
            {
                if (Cbx_order.SelectedItem.ToString() == "Nome")
                {
                    dt.DefaultView.RowFilter = string.Format("[{0}] LIKE '%{1}%'", "Nome", Txt_pesquisa.Text);
                    Dgv_Produtos.DataSource = dt;
                }
                if (Cbx_order.SelectedItem.ToString() == "Código" && IsNumeric(Txt_pesquisa.Text))
                {
                    dt.DefaultView.RowFilter = string.Format("[{0}] = {1}", "Código", Txt_pesquisa.Text);
                    Dgv_Produtos.DataSource = dt;
                }
                if (Cbx_order.SelectedItem.ToString() == "Quantidade" && IsNumeric(Txt_pesquisa.Text))
                {
                    dt.DefaultView.RowFilter = string.Format("[{0}] = {1}", "Quantidade", Txt_pesquisa.Text);
                    Dgv_Produtos.DataSource = dt;
                }
            }
        }
        private void LimparLinhas()
        {
            Dgv_Produtos.Refresh();
            dt.Clear();
        }
        private void ListarProdutos()
        {
            if (cn2.State == ConnectionState.Open)
                cn2.Close();
            if (cn3.State == ConnectionState.Open)
                cn3.Close();
            SqlDataReader reader = null;
            try
            {
                if (retorno == connect1)
                {
                    cn3.Open();
                    SqlCommand cmd = new SqlCommand(strSelect, cn3);
                    // Executando o commando e obtendo o resultado
                    reader = cmd.ExecuteReader();
                    // Exibindo os registros
                    while (reader.Read())
                    {
                        var valor = reader["PRECOUNIT"].ToString();
                        if (valor == "")
                            vlUnit = "R$ 0,00";
                        else
                        {
                            var commaPosition = valor.ToString().IndexOf(",", StringComparison.Ordinal);
                            if (commaPosition == -1)
                                valor += ",00";
                            else
                            {
                                string vlr = valor + "0";
                                commaPosition = vlr.ToString().IndexOf(",", StringComparison.Ordinal);
                                string posVirgula = vlr.Substring(commaPosition, 3);
                                if (posVirgula.Substring(2, 1) == "0")
                                    valor = vlr;
                            }
                            commaPosition = valor.ToString().IndexOf(",", StringComparison.Ordinal);
                            string result = commaPosition + 3 > valor.ToString().Length ? valor : valor.ToString().Substring(0, commaPosition + 3);
                            vlUnit = "R$ " + result;
                        }

                        var valor1 = reader["PRECOAVULSO"].ToString();
                        if (valor1 == "")
                            vlAvulso = "R$ 0,00";
                        else
                        {
                            var commaPosition = valor1.ToString().IndexOf(",", StringComparison.Ordinal);
                            if (commaPosition == -1)
                                valor1 += ",00";
                            else
                            {
                                string vlr = valor1 + "0";
                                commaPosition = vlr.ToString().IndexOf(",", StringComparison.Ordinal);
                                string posVirgula = vlr.Substring(commaPosition, 3);
                                if (posVirgula.Substring(2, 1) == "0")
                                    valor1 = vlr;
                            }
                            commaPosition = valor1.ToString().IndexOf(",", StringComparison.Ordinal);
                            string result = commaPosition + 3 > valor1.ToString().Length ? valor1 : valor1.ToString().Substring(0, commaPosition + 3);
                            vlAvulso = "R$ " + result;
                        }
                        dt.Rows.Add(string.Format("{0}", Convert.ToInt32(reader["IDPRODUTO"])),
                                    string.Format("{0}", reader["NOMEPRODUTO"]),
                                    string.Format("{0}", Convert.ToInt32(reader["QUANTIDADE"])),
                                    string.Format("{0}", Convert.ToInt32(reader["SAIDAMENSAL"])),
                                    string.Format("{0}", vlUnit),
                                    string.Format("{0}", vlAvulso));
                        Dgv_Produtos.DataSource = dt;
                    }
                }
                else
                {
                    cn2.Open();
                    SqlCommand cmd = new SqlCommand(strSelect, cn2);
                    // Executando o commando e obtendo o resultado
                    reader = cmd.ExecuteReader();
                    // Exibindo os registros
                    while (reader.Read())
                    {
                        var valor = reader["PRECOUNIT"].ToString();
                        if (valor == "")
                            vlUnit = "R$ 0,00";
                        else
                        {
                            var commaPosition = valor.ToString().IndexOf(",", StringComparison.Ordinal);
                            if (commaPosition == -1)
                                valor += ",00";
                            else
                            {
                                string vlr = valor + "0";
                                commaPosition = vlr.ToString().IndexOf(",", StringComparison.Ordinal);
                                string posVirgula = vlr.Substring(commaPosition, 3);
                                if (posVirgula.Substring(2, 1) == "0")
                                    valor = vlr;
                            }
                            commaPosition = valor.ToString().IndexOf(",", StringComparison.Ordinal);
                            string result = commaPosition + 3 > valor.ToString().Length ? valor : valor.ToString().Substring(0, commaPosition + 3);
                            vlUnit = "R$ " + result;
                        }

                        var valor1 = reader["PRECOAVULSO"].ToString();
                        if (valor1 == "")
                            vlAvulso = "R$ 0,00";
                        else
                        {
                            var commaPosition = valor1.ToString().IndexOf(",", StringComparison.Ordinal);
                            if (commaPosition == -1)
                                valor1 += ",00";
                            else
                            {
                                string vlr = valor1 + "0";
                                commaPosition = vlr.ToString().IndexOf(",", StringComparison.Ordinal);
                                string posVirgula = vlr.Substring(commaPosition, 3);
                                if (posVirgula.Substring(2, 1) == "0")
                                    valor1 = vlr;
                            }
                            commaPosition = valor1.ToString().IndexOf(",", StringComparison.Ordinal);
                            string result = commaPosition + 3 > valor1.ToString().Length ? valor1 : valor1.ToString().Substring(0, commaPosition + 3);
                            vlAvulso = "R$ " + result;
                        }
                        dt.Rows.Add(string.Format("{0}", Convert.ToInt32(reader["IDPRODUTO"])),
                                    string.Format("{0}", reader["NOMEPRODUTO"]),
                                    string.Format("{0}", Convert.ToInt32(reader["QUANTIDADE"])),
                                    string.Format("{0}", Convert.ToInt32(reader["SAIDAMENSAL"])),
                                    string.Format("{0}", vlUnit),
                                    string.Format("{0}", vlAvulso));
                        Dgv_Produtos.DataSource = dt;
                    }
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erro: " + ex);
            }
            finally
            {
                // Fecha o datareader
                if (reader != null)
                {
                    reader.Close();
                }
                cn2.Close();
                cn3.Close();
            }
        }
        private void CarregarColunas()
        {
            //dt.Columns.Add("Código", typeof(int)).AutoIncrementSeed = /*Codigo do ultimo select no banco*/;
            dt.Columns.Add("Código", typeof(int));
            dt.Columns["Código"].ReadOnly = true;
            dt.Columns.Add("Nome", typeof(string));
            dt.Columns["Nome"].MaxLength = 50;
            dt.Columns.Add("Quantidade", typeof(int));
            dt.Columns.Add("Saida Mensal", typeof(int));
            dt.Columns.Add("Preço Unit.", typeof(string));
            dt.Columns.Add("Preço Avulso", typeof(string));
            dt.Columns.Add("Entrada", typeof(int));
            dt.Columns.Add("Saida", typeof(int));
            Dgv_Produtos.DataSource = dt;
            Dgv_Produtos.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv_Produtos.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Dgv_Produtos.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv_Produtos.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Dgv_Produtos.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv_Produtos.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Dgv_Produtos.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv_Produtos.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Dgv_Produtos.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv_Produtos.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Dgv_Produtos.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv_Produtos.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Dgv_Produtos.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv_Produtos.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Dgv_Produtos.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Dgv_Produtos.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        private bool IsNumeric(string valor)
        {
            return valor.All(char.IsNumber);
        }
        private void btn_sim_Click(object sender, EventArgs e)
        {
            var selectedRows = Dgv_Produtos.SelectedRows
            .OfType<DataGridViewRow>()
            .Where(row => !row.IsNewRow)
            .ToArray();
            if (cn2.State == ConnectionState.Open)
                cn2.Close();
            if (cn3.State == ConnectionState.Open)
                cn3.Close();
            if (MessageBox.Show("Tem certeza que deseja excluir o registro selecionado?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (cbx_exibir.SelectedItem.ToString() == "Consulta")
                {
                    try
                    {
                        if (retorno == connect1)
                        {
                            for (int i = 0; i < Dgv_Produtos.Rows.Count; i++)
                            {
                                if (Dgv_Produtos.CurrentRow == null)
                                {
                                    MessageBox.Show("Nenhuma lista selecionada para remoção de registro");
                                    return;
                                }
                                int linhaval = Dgv_Produtos.CurrentRow.Index;
                                string StrQuery;
                                StrQuery = "DELETE FROM PRODUTO WHERE IDPRODUTO = "
                                + Convert.ToInt32(Dgv_Produtos.Rows[linhaval].Cells["Código"].Value);
                                SqlCommand cm = new SqlCommand(StrQuery, cn3);
                                cn3.Open();
                                cm.ExecuteNonQuery();
                                cn3.Close();
                            }
                            MessageBox.Show("Produto(s) deletado(s) com sucesso");
                            String codv = @"" + Dgv_Produtos.CurrentRow.Cells["Código"].Value;
                            int codx = Dgv_Produtos.CurrentRow.Index;
                            Dgv_Produtos.Rows.RemoveAt(codx);
                            Dgv_Produtos.ReadOnly = true;
                            LimparLinhas();
                            ListarProdutos();
                            btn_inserir.Visible = false;
                            btn_editar.Visible = true;
                            btn_deletar.Visible = true;
                            btn_salvar.Visible = true;
                            btn_nao.Visible = false;
                            btn_sim.Visible = false;
                        }
                        else
                        {
                            for (int i = 0; i < Dgv_Produtos.Rows.Count; i++)
                            {
                                int linhaval = Dgv_Produtos.CurrentRow.Index;
                                string StrQuery;
                                StrQuery = "DELETE FROM PRODUTO WHERE IDPRODUTO = "
                                + Convert.ToInt32(Dgv_Produtos.Rows[linhaval].Cells["Código"].Value);
                                SqlCommand cm = new SqlCommand(StrQuery, cn2);
                                cn2.Open();
                                cm.ExecuteNonQuery();
                                cn2.Close();
                            }
                            MessageBox.Show("Produto(s) deletado(s) com sucesso");
                            String codv = @"" + Dgv_Produtos.CurrentRow.Cells["Código"].Value;
                            int codx = Dgv_Produtos.CurrentRow.Index;
                            Dgv_Produtos.Rows.RemoveAt(codx);
                            Dgv_Produtos.ReadOnly = true;
                            LimparLinhas();
                            ListarProdutos();
                            btn_inserir.Visible = false;
                            btn_editar.Visible = true;
                            btn_deletar.Visible = true;
                            btn_salvar.Visible = true;
                            btn_nao.Visible = false;
                            btn_sim.Visible = false;
                        }

                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Erro: " + ex);
                    }
                    finally
                    {
                        cn2.Close();
                        cn3.Close();
                    }
                }
                else
                {
                    if (Dgv_Produtos.CurrentRow == null)
                    {
                        MessageBox.Show("Nenhuma lista selecionada para remoção de registro");
                        return;
                    }
                    int codx = Dgv_Produtos.CurrentRow.Index;
                    Dgv_Produtos.Rows.RemoveAt(codx);
                }
            }
            btn_nao.Visible = false;
            btn_sim.Visible = false;
        }
        private void btn_nao_Click(object sender, EventArgs e)
        {
            btn_sim.Visible = false;
            btn_nao.Visible = false;
        }
        private void btn_salvar_Click(object sender, EventArgs e)
        {
            if (cn2.State == ConnectionState.Open)
                cn2.Close();
            if (cn3.State == ConnectionState.Open)
                cn3.Close();
            if (cbx_exibir.SelectedItem.ToString() == "Inserção")
            {
                for (int i = 0; i < Dgv_Produtos.Rows.Count - 1; i++)
                {
                    string nomeval = @"" + Dgv_Produtos.Rows[i].Cells["Nome"].Value;
                    string quantval = @"" + Dgv_Produtos.Rows[i].Cells["Quantidade"].Value;
                    string quantmensalval = @"" + Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value;
                    if (string.IsNullOrEmpty(nomeval) == true ||
                       string.IsNullOrEmpty(quantval) == true || IsNumeric(quantval) == false)
                    {
                        int index = i + 1;
                        MessageBox.Show("Preencha todos os dados da linha:" + index);
                        return;
                    }
                }
                try
                {
                    if (retorno == connect1)
                    {
                        for (int i = 0; i < Dgv_Produtos.Rows.Count - 1; i++)
                        {
                            if (Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value.ToString() == "")
                                Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value = 0;
                            if (Dgv_Produtos.Rows[i].Cells["Preço Unit."].Value.ToString() == "")
                                Dgv_Produtos.Rows[i].Cells["Preço Unit."].Value = 0;
                            if (Dgv_Produtos.Rows[i].Cells["Preço Avulso"].Value.ToString() == "")
                                Dgv_Produtos.Rows[i].Cells["Preço Avulso"].Value = 0;
                            string precoUnit = Dgv_Produtos.Rows[i].Cells["Preço Unit."].Value.ToString().Replace(',', '.');
                            string precoAvulso = Dgv_Produtos.Rows[i].Cells["Preço Avulso"].Value.ToString().Replace(',', '.'); string StrQuery;
                            StrQuery = "INSERT INTO PRODUTO (NOMEPRODUTO, QUANTIDADE, SAIDAMENSAL, PRECOUNIT, PRECOAVULSO) VALUES ('"
                            + Dgv_Produtos.Rows[i].Cells["Nome"].Value.ToString() + "', "
                            + Dgv_Produtos.Rows[i].Cells["Quantidade"].Value + ", "
                            + Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value + ", "
                            + precoUnit + ", "
                            + precoAvulso + ")";
                            SqlCommand cm = new SqlCommand(StrQuery, cn3);
                            cn3.Open();
                            cm.ExecuteNonQuery();
                            cn3.Close();
                        }
                        MessageBox.Show("Produto(s) cadastrado(s) com sucesso");
                        Dgv_Produtos.ReadOnly = true;
                        LimparLinhas();
                        btn_inserir.Visible = true;
                        btn_editar.Visible = false;
                        btn_deletar.Visible = true;
                        btn_salvar.Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < Dgv_Produtos.Rows.Count - 1; i++)
                        {
                            if (Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value.ToString() == "")
                                Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value = 0;
                            if (Dgv_Produtos.Rows[i].Cells["Preço Unit."].Value.ToString() == "")
                                Dgv_Produtos.Rows[i].Cells["Preço Unit."].Value = 0;
                            if (Dgv_Produtos.Rows[i].Cells["Preço Avulso"].Value.ToString() == "")
                                Dgv_Produtos.Rows[i].Cells["Preço Avulso"].Value = 0;
                            string precoUnit = Dgv_Produtos.Rows[i].Cells["Preço Unit."].Value.ToString().Replace(',', '.');
                            string precoAvulso = Dgv_Produtos.Rows[i].Cells["Preço Avulso"].Value.ToString().Replace(',', '.');
                            string StrQuery;
                            StrQuery = "INSERT INTO PRODUTO (NOMEPRODUTO, QUANTIDADE, SAIDAMENSAL, PRECOUNIT, PRECOAVULSO) VALUES ('"
                            + Dgv_Produtos.Rows[i].Cells["Nome"].Value.ToString() + "', "
                            + Dgv_Produtos.Rows[i].Cells["Quantidade"].Value + ", "
                            + Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value + ", "
                            + precoUnit + ", "
                            + precoAvulso + ")";
                            SqlCommand cm = new SqlCommand(StrQuery, cn2);
                            cn2.Open();
                            cm.ExecuteNonQuery();
                            cn2.Close();
                        }
                        MessageBox.Show("Produto(s) cadastrado(s) com sucesso");
                        Dgv_Produtos.ReadOnly = true;
                        LimparLinhas();
                        btn_inserir.Visible = true;
                        btn_editar.Visible = false;
                        btn_deletar.Visible = true;
                        btn_salvar.Visible = true;
                    }

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro: " + ex);
                }
                finally
                {
                    cn2.Close();
                    cn3.Close();
                }
            }
            else if (cbx_exibir.SelectedItem.ToString() == "Consulta")
            {
                for (int i = 0; i < Dgv_Produtos.Rows.Count; i++)
                {
                    string codval = @"" + Dgv_Produtos.Rows[i].Cells["Código"].Value;
                    string nomeval = @"" + Dgv_Produtos.Rows[i].Cells["Nome"].Value;
                    if (string.IsNullOrEmpty(codval) == true ||
                       string.IsNullOrEmpty(nomeval) == true)
                    {
                        int index = i + 1;

                        MessageBox.Show("Preencha todos os dados da linha:" + index);

                    }
                }
                try
                {
                    if (retorno == connect1)
                    {
                        for (int i = 0; i < Dgv_Produtos.Rows.Count; i++)
                        {
                            string quantval = @"" + Dgv_Produtos.Rows[i].Cells["Quantidade"].Value;
                            string entradaval = @"" + Dgv_Produtos.Rows[i].Cells["Entrada"].Value;
                            string saidaval = @"" + Dgv_Produtos.Rows[i].Cells["Saida"].Value;
                            string saidamensalval = @"" + Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value;
                            if (String.IsNullOrEmpty(entradaval))
                                entradaval = "0";
                            if (String.IsNullOrEmpty(saidaval))
                                saidaval = "0";
                            if (String.IsNullOrEmpty(quantval))
                                quantval = "0";
                            string precoUnit = Dgv_Produtos.Rows[i].Cells["Preço Unit."].Value.ToString().Replace(',', '.');
                            precoUnit = precoUnit.Replace("R$", "");
                            string precoAvulso = Dgv_Produtos.Rows[i].Cells["Preço Avulso"].Value.ToString().Replace(',', '.');
                            precoAvulso = precoAvulso.Replace("R$", "");
                            string total = Convert.ToString(Convert.ToInt32(quantval) - Convert.ToInt32(saidaval) + Convert.ToInt32(entradaval));
                            string saidamensaltotalval = Convert.ToString(Convert.ToInt32(saidamensalval) + Convert.ToInt32(saidaval));
                            string StrQuery;
                            StrQuery = "UPDATE PRODUTO SET NOMEPRODUTO ='"
                            + Dgv_Produtos.Rows[i].Cells["Nome"].Value.ToString() + "', QUANTIDADE = "
                            + total + ", SAIDAMENSAL = " + saidamensaltotalval + ", PRECOUNIT = " +
                            precoUnit + ", PRECOAVULSO = " +
                           precoAvulso
                            + " WHERE IDPRODUTO = "
                            + Dgv_Produtos.Rows[i].Cells["Código"].Value + "";
                            SqlCommand cm = new SqlCommand(StrQuery, cn3);
                            cn3.Open();
                            cm.ExecuteNonQuery();
                            cn3.Close();
                        }
                        MessageBox.Show("Produto(s) atualizado(s) com sucesso");
                        Dgv_Produtos.ReadOnly = true;
                        LimparLinhas();
                        ListarProdutos();
                        btn_inserir.Visible = false;
                        btn_editar.Visible = true;
                        btn_deletar.Visible = true;
                        btn_salvar.Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < Dgv_Produtos.Rows.Count; i++)
                        {
                            string quantval = @"" + Dgv_Produtos.Rows[i].Cells["Quantidade"].Value;
                            string entradaval = @"" + Dgv_Produtos.Rows[i].Cells["Entrada"].Value;
                            string saidaval = @"" + Dgv_Produtos.Rows[i].Cells["Saida"].Value;
                            string saidamensalval = @"" + Dgv_Produtos.Rows[i].Cells["Saida Mensal"].Value;
                            if (String.IsNullOrEmpty(entradaval))
                                entradaval = "0";
                            if (String.IsNullOrEmpty(saidaval))
                                saidaval = "0";
                            if (String.IsNullOrEmpty(quantval))
                                quantval = "0";
                            string precoUnit = Dgv_Produtos.Rows[i].Cells["Preço Unit."].Value.ToString().Replace(',', '.');
                            precoUnit = precoUnit.Replace("R$", "");
                            string precoAvulso = Dgv_Produtos.Rows[i].Cells["Preço Avulso"].Value.ToString().Replace(',', '.');
                            precoAvulso = precoAvulso.Replace("R$", "");
                            string total = Convert.ToString(Convert.ToInt32(quantval) - Convert.ToInt32(saidaval) + Convert.ToInt32(entradaval));
                            string saidamensaltotalval = Convert.ToString(Convert.ToInt32(saidamensalval) + Convert.ToInt32(saidaval));
                            string StrQuery;
                            StrQuery = "UPDATE PRODUTO SET NOMEPRODUTO ='"
                            + Dgv_Produtos.Rows[i].Cells["Nome"].Value.ToString() + "', QUANTIDADE = "
                            + total + ", SAIDAMENSAL = " + saidamensaltotalval + ", PRECOUNIT = " +
                            precoUnit + ", PRECOAVULSO = " +
                            precoAvulso
                            + " WHERE IDPRODUTO = "
                            + Dgv_Produtos.Rows[i].Cells["Código"].Value + "";
                            SqlCommand cm = new SqlCommand(StrQuery, cn2);
                            cn2.Open();
                            cm.ExecuteNonQuery();
                            cn2.Close();
                        }
                        MessageBox.Show("Produto(s) atualizado(s) com sucesso");
                        Dgv_Produtos.ReadOnly = true;
                        LimparLinhas();
                        ListarProdutos();
                        btn_inserir.Visible = false;
                        btn_editar.Visible = true;
                        btn_deletar.Visible = true;
                        btn_salvar.Visible = true;
                    }

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Erro: " + ex);
                }
                finally
                {
                    cn2.Close();
                    cn3.Close();
                }
            }
        }
        private void btn_editar_Click(object sender, EventArgs e)
        {
            btn_salvar.Visible = true;
            Dgv_Produtos.ReadOnly = false;
            Dgv_Produtos.AllowUserToAddRows = false;
            btn_sim.Visible = false;
            btn_nao.Visible = false;
            dt.Columns["Entrada"].ReadOnly = false;
            dt.Columns["Saida"].ReadOnly = false;
            dt.Columns["Quantidade"].ReadOnly = true;
            dt.Columns["Saida Mensal"].ReadOnly = true;
        }
        private void btn_inserir_Click(object sender, EventArgs e)
        {
            btn_salvar.Visible = true;
            Dgv_Produtos.AllowUserToAddRows = true;
            Dgv_Produtos.ReadOnly = false;
            dt.Rows.Add(new object[] { });
            dt.Columns["Entrada"].ReadOnly = true;
            dt.Columns["Saida"].ReadOnly = true;
            dt.Columns["Quantidade"].ReadOnly = false;
            dt.Columns["Saida Mensal"].ReadOnly = false;
        }
        private void btn_deletar_Click(object sender, EventArgs e)
        {
            btn_salvar.Visible = true;
            btn_sim.Visible = true;
            btn_nao.Visible = true;
        }
        private void btn_pesquisar_Click(object sender, EventArgs e)
        {
            FiltrarGrade();
        }
        private void cbx_order_SelectedIndexChanged(object sender, EventArgs e)
        {
            dt.DefaultView.RowFilter = null;
            Txt_pesquisa.Text = "";
        }
        private void cbx_exibir_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_salvar.Visible = false;
            LimparLinhas();
            if (cbx_exibir.SelectedItem.ToString() == "Inserção")
            {
                Dgv_Produtos.ReadOnly = true;
                LimparLinhas();
                btn_inserir.Visible = true;
                btn_editar.Visible = false;
                btn_deletar.Visible = true;
            }
            if (cbx_exibir.SelectedItem.ToString() == "Consulta")
            {
                Dgv_Produtos.ReadOnly = true;
                ListarProdutos();
                btn_inserir.Visible = false;
                btn_editar.Visible = true;
                btn_deletar.Visible = true;
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
        private void button1_Click(object sender, EventArgs e)
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
        private void button2_Click(object sender, EventArgs e)
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

        private void Dgv_Produtos_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            }
        }

        void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            string headerText = Dgv_Produtos.Columns[Dgv_Produtos.CurrentCell.ColumnIndex].HeaderText;
            if (cbx_exibir.SelectedItem.ToString() == "Inserção" && headerText == "Entrada")
            {
                e.Handled = true;
                return;
            }

            if (cbx_exibir.SelectedItem.ToString() == "Inserção" && headerText == "Saida")
            {
                e.Handled = true;
                return;
            }

            if (cbx_exibir.SelectedItem.ToString() == "Consulta" && headerText == "Quantidade")
            {
                e.Handled = true;
                return;
            }

            if (cbx_exibir.SelectedItem.ToString() == "Consulta" && headerText == "Saida Mensal")
            {
                e.Handled = true;
                return;
            }

            if (headerText.Equals("Preço Unit."))
            {
                if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != (char)46 && e.KeyChar != (char)44)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (headerText.Equals("Preço Avulso"))
            {
                if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != (char)46 && e.KeyChar != (char)44)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (headerText.Equals("Quantidade"))
            {
                if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != (char)46)
                {
                    e.Handled = true;
                    return;
                }
            }


            if (headerText.Equals("Saida Mensal"))
            {
                if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != (char)46)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (headerText.Equals("Entrada"))
            {
                if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != (char)46)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (headerText.Equals("Saida"))
            {
                if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != (char)46)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (headerText.Equals("Código"))
            {
                if (e.KeyChar != 99999)
                {
                    e.Handled = true;
                    return;
                }
            }

            if (headerText.Equals("Nome"))
            {
                if (e.KeyChar == (char)39)
                {
                    e.Handled = true;
                    return;
                }
                if (e.KeyChar == (char)34)
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
