using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NAudio.Wave;

namespace 根本悠希_インターン課題_20250507
{
    public partial class Form1 : Form
    {
        const float Byte = 1024.0f;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private string currentFilePath = null; // 現在の再生ファイルパスを追跡

        public Form1()
        {
            InitializeComponent();
            //ドラック＆ドロップをテキストボックスで有効にする
            textBox2.AllowDrop = true;
            // ドラッグされたファイルを受け入れた際のイベントハンドラーを追加
            textBox2.DragEnter += TextBox2_DragEnter;
            textBox2.DragDrop += TextBox2_DragDrop;
        }

        private void Reference_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "音楽ファイルを選択してください";
            openFileDialog1.Filter = "音楽ファイル (*.mp3;*.wav)|*.mp3;*.wav|すべてのファイル (*.*)|*.*";
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            openFileDialog1.Multiselect = true; // 複数選択を許可

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {


                foreach (string filePath in openFileDialog1.FileNames)
                {
                    FileInfo fi = new FileInfo(filePath);
                    // ファイルサイズをキロバイトに変換
                    double fileSizeKB = fi.Length / Byte;

                    ListViewItem item = new ListViewItem(fi.Name);
                    item.SubItems.Add(fi.Extension);
                    item.SubItems.Add(fi.CreationTime.ToString("yyyy-MM-dd HH:mm"));
                    item.Tag = fi.FullName; // パスを保存
                    double fileSizeMB = fileSizeKB / Byte;
                    item.SubItems.Add(fileSizeMB.ToString("F1") + " MB"); // 小数点以下2桁で表示

                    listView1.Items.Add(item);
                }

            }
        }


        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("再生するファイルを選択してください。");
                return;
            }

            string selectedFilePath = listView1.SelectedItems[0].Tag.ToString();

            // 同じファイルが選ばれている場合：再生・一時停止の切り替え
            if (currentFilePath == selectedFilePath)
            {
                if (outputDevice != null)
                {
                    if (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        outputDevice.Pause();
                        PlayButton.Text = "再開";
                        return;
                    }
                    else if (outputDevice.PlaybackState == PlaybackState.Paused)
                    {
                        outputDevice.Play();
                        PlayButton.Text = "一時停止";
                        return;
                    }
                }
            }
            else
            {
                // ファイルが変更された → 前の再生を停止・解放
                if (outputDevice != null)
                {
                    outputDevice.Stop();
                    outputDevice.Dispose();
                    outputDevice = null;
                }
                if (audioFile != null)
                {
                    audioFile.Dispose();
                    audioFile = null;
                }

                try
                {
                    audioFile = new AudioFileReader(selectedFilePath);
                    outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    PlayButton.Text = "一時停止";
                    currentFilePath = selectedFilePath; // 現在のパスを更新
                }
                catch (Exception ex)
                {
                    MessageBox.Show("再生エラー: " + ex.Message);
                }
            }
        }
        //Form1のコンストラクタでテキストボックスのドラック＆ドロップを設定
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
         

          
        }
        private void TextBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect=DragDropEffects.Copy;// ドロップ時にコピー操作
            }
            else
            {
                e.Effect=DragDropEffects.None;
            }
        }
        private void TextBox2_DragDrop(object sender, DragEventArgs e)
        {
            //ドラッグされたファイルのパスを取得
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filePath in filePaths)
            {
                //ファイルが音楽ファイルか確認
                if(filePath.EndsWith(".mp3",StringComparison.OrdinalIgnoreCase)||filePath.EndsWith(".wav",StringComparison.OrdinalIgnoreCase))
                {
                    //ファイルの大きさ取得
                    FileInfo fi=new FileInfo(filePath);
                    double fileSizeKB=fi.Length/Byte;
                    double fileSizeMB = fileSizeKB / Byte;
                    ListViewItem item = new ListViewItem(fi.Name);
                    item.SubItems.Add(fi.Extension);
                    item.SubItems.Add(fi.CreationTime.ToString("yyyy-MM-dd HH:mm"));
                    item.SubItems.Add(fileSizeMB.ToString("F1") + "MB");
                    item.Tag = fi.FullName;

                    listView1.Items.Add(item);

                }
            }
        }
        //削除ボタン
        private void deletion_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("削除する項目を選択してください。");
                return;
            }
            foreach (ListViewItem selectedItem in listView1.SelectedItems)
            {
                listView1.Items.Remove(selectedItem);
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void form1BindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
