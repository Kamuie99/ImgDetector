using System;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ImgDetector
{
    public partial class Form1 : Form
    {
        private Mat image; //! openCV 이미지 저장용

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //! 이미지 크기 자동 조절

                image = Cv2.ImRead(openFileDialog.FileName); //! openCV 이미지로 로드
                pictureBox1.Image = BitmapConverter.ToBitmap(image);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //! 이미지 크기 자동 조절

            }
        }

        private void btnDetectCorners_Click(object sender, EventArgs e)
        {
            if (image == null)
                return;

            //! 그레이스케일로 변환
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            //! 모서리 감지
            Point2f[] corners = Cv2.GoodFeaturesToTrack(
                gray,
                maxCorners: 4,
                qualityLevel: 0.01,
                minDistance: 20,
                mask: null,
                blockSize: 3,
                useHarrisDetector: false,
                k: 0.04
            );

            listBoxCorners.Items.Clear(); //! 기존 좌표 초기화
            
            if (corners != null)
            {
                foreach (var pt in corners)
                {
                    //! 각 모서리 위치에 네모 그리기
                    Cv2.Rectangle(image, new Rect((int)pt.X - 5, (int)pt.Y - 5, 10, 10), Scalar.Red, 2);

                    //! 좌표 출력
                    string pointStr = $"X: {pt.X:F1}, Y: {pt.Y:F1}";
                    listBoxCorners.Items.Add(pointStr);
                }

                pictureBox1.Image = BitmapConverter.ToBitmap(image); //! 네모 그린 후 이미지 표시
            }
            else
            {
                MessageBox.Show("코너 검출 불가");
            }

        }
    }
}
