using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace findPathFordBellman
{
    public partial class frmMain : Form
    {
        #region InitializeProgram
        public frmMain()
        {
            InitializeComponent();
        }

        private Graph graph;
        private DijkstraShortestPathAlg spAlg;
        private YenTopKShortestPathsAlg yenAlg;
        /// <summary>
        /// Kiểm tra đỉnh bắt đầu
        /// </summary>
        private int minVertex = 1000000;
        /// <summary>
        /// Max của độ dài các cạnh
        /// </summary>
        const int MAXINT = 1000000000;

        // Mảng lưu dữ liệu đồ thị
        int[,] A;
        // Mảng lưu đường đi
        int[] L;
        // Điểm đi, điểm đến
        int source, dest;
        // Số đỉnh của đồ thị
        int n;
        // Mảng đánh dấu các đỉnh đã đi qua
        int[] Tick;
        // Giá trị đường đi ngắn nhất
        int minLength = MAXINT;
        // Các đỉnh đã đi qua
        string minPath = "";

        #endregion

        #region Các hàm hỗ trợ
        void ReadData()
        {
            try
            {
                //string path = Application.StartupPath;
                string path = @"./Input";
                OpenFileDialog Odg = new OpenFileDialog()
                {
                    FileName = "Select a File",
                    Filter = "Select files (*.txt)|*.txt",
                    Title = "Open file"
                };

                Odg.InitialDirectory = path;

                if (Odg.ShowDialog() == DialogResult.OK)
                {
                    string filePath = Odg.FileName;

                    using (var reader = new StreamReader(filePath))
                    {
                        string data = reader.ReadLine();
                        n = Convert.ToInt32(data.Trim());
                        A = new int[n + 1, n + 1];
                        while ((data = reader.ReadLine()) != null)
                        {
                            data = data.Trim();
                            var s = data.Split('\t', ' ');
                            if (s.Length == 3)
                            {
                                int x = Convert.ToInt32(s[0]);
                                int y = Convert.ToInt32(s[1]);
                                int z = Convert.ToInt32(s[2]);
                                A[x, y] = z;
                                if (x < minVertex) minVertex = x;
                                else if (y < minVertex) minVertex = y;
                            }
                        }
                    }
                    if (graph != null)
                    {
                        graph = null;
                    }
                    graph = new VariableGraph(System.IO.Path.GetFileNameWithoutExtension(Odg.FileName) + ".txt");
                    MessageBox.Show("Đọc dữ liệu thành công", "Đường đi ngắn nhất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSource.Enabled = true;
                    txtDest.Enabled = true;
                    btnExecute.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi! \n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Reset()
        {
            Cursor.Current = Cursors.WaitCursor;
            txtDest.Text = "";
            txtSource.Text = "";
            rtbResult.Text = "";
            txtSource.Enabled = false;
            txtDest.Enabled = false;
            btnExecute.Enabled = false;
            graph = null;
            Cursor.Current = Cursors.Default;
        }
        #endregion

        #region Ford-Bellman
        int[] d;
        int[] Trace;
        void InitializeFord_Bellman()
        {
            d = new int[n + 1];
            Trace = new int[n + 1];
            for (int i = 1; i <= n; i++)
            {
                d[i] = MAXINT;
                for (int j = 1; j <= n; j++)
                {
                    if (A[i, j] == 0 && i != j)
                        A[i, j] = MAXINT;
                }
            }
            d[source] = 0;
        }

        void Restore()
        {
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (A[i, j] == MAXINT)
                        A[i, j] = 0;
                }
            }
        }

        void Ford_Bellman()
        {
            bool isStop;
            for (int i = 1; i < n; i++)
            {
                isStop = true;
                for (int u = 1; u <= n; u++)
                    for (int v = 1; v <= n; v++)
                    {
                        if (A[u, v] + d[u] < d[v])
                        {
                            d[v] = d[u] + A[u, v];
                            Trace[v] = u;
                            isStop = false;
                        }
                    }

                if (isStop) break;
            }
        }

        string PrintResult()
        {
            string res = "";
            if (d[dest] == MAXINT)
            {
                res = "Không có đường đi từ " + source + "đến " + dest;
            }
            else
            {
                int path = d[dest];
                while (dest != source)
                {
                    res += dest + " <-- ";
                    dest = Trace[dest];
                }
                res += source + ": " + path;
            }
            return res;
        }
        #endregion

        #region Các hàm xử lý sự kiện

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                source = Convert.ToInt16(txtSource.Text);
                dest = Convert.ToInt16(txtDest.Text);

                if (source >= minVertex && dest <= n)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    if (rdoFordBellman.Checked)
                    {
                        InitializeFord_Bellman();
                        Ford_Bellman();
                        rtbResult.Text = PrintResult();
                        Restore();
                    }

                    watch.Stop();
                    lbTime.Text = "Thời gian thực hiện chương trình: " + watch.Elapsed.TotalMilliseconds + " ms";
                }
                else
                {
                    MessageBox.Show("Điểm bắt đầu phải >= " + minVertex + " và Điểm đến phải <= " + n,
                        "Đường đi ngắn nhất", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mniExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mniOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                Reset();
                Cursor.Current = Cursors.WaitCursor;
                ReadData();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message, "Đường đi ngắn nhất", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
