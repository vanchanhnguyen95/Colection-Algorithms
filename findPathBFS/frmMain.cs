using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace findPathBFS
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

        #endregion

        #region Các hàm hỗ trợ
        void ReadData()
        {
            try
            {
                string path = Application.StartupPath;
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

        #region BFS
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

        void InitializeBFS()
        {
            L = new int[n + 1];
            Tick = new int[n + 1];

            for (int i = 1; i <= n; i++)
            {
                Tick[i] = 0;
                L[i] = 0;
            }
            Tick[source] = 1;
            L[1] = source;
            minLength = MAXINT;
        }

        void SaveResult(int edge)
        {
            string path = source.ToString() + " -> ";
            int length = 0;
            for (int i = 2; i <= edge; i++)
            {
                path = path + L[i].ToString() + " -> ";
                length = length + A[L[i - 1], L[i]];
            }
            if (length < minLength)
            {
                minLength = length;
                minPath = path;
            }
        }

        // Hàm đệ quy tìm mọi đường đi ngắn nhất giữa hai đỉnh
        void TryBFS(int edge)
        {
            if (L[edge] == dest)
            {
                // Mảng L là mảng lưu kết quả đường đi
                SaveResult(edge);
            }
            else
            {
                for (int i = 1; i <= n; i++)
                    if (A[L[edge], i] > 0 && Tick[i] == 0)
                    {
                        L[edge + 1] = i;
                        Tick[i] = 1;
                        TryBFS(edge + 1);
                        L[edge + 1] = 0;
                        Tick[i] = 0;
                    }
            }
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
                    if (rdoBFS.Checked)
                    {
                        InitializeBFS();
                        TryBFS(source);
                        minPath = minPath.Substring(0, minPath.Length - 3);
                        rtbResult.Text = minPath + ": " + minLength;
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
        private void mniReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        #endregion
    }
}
