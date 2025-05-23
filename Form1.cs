using System;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Collections.Generic;
using CvPoint = OpenCvSharp.Point;

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
                image = Cv2.ImRead(openFileDialog.FileName); //! openCV 이미지로 로드
                pictureBox1.Image = BitmapConverter.ToBitmap(image);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //! 이미지 크기 자동 조절

            }
        }

        private void btnDetectCorners_Click(object sender, EventArgs e)
        {
            if (image == null)
                return;

            //! 원본 이미지 보존, 표시용으로 복사본 생성
            Mat displayImg = image.Clone();

            //! 그레이스케일로 변환 (코너 검출에 더 적합)
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            //! 모서리 감지
            Point2f[] corners = Cv2.GoodFeaturesToTrack(
                gray,
                maxCorners: 8,              //! 최대 8개의 코너를 찾는다 (모깍인 꼭지점 4쌍 유도)
                qualityLevel: 0.01,         //! 코너 품질 임계값
                minDistance: 10,            //! 코너 간 최소 거리
                mask: null,                 //! 마스크 없음
                blockSize: 3,               //! 계산 윈도우 크기
                useHarrisDetector: false,   //! Harris 코너 검출기 사용 안 함
                k: 0.04                     //! Harris 계수 (사용 안하지만 기본값 필요)
            );

            //! 기존 좌표 표시박스 초기화
            listBoxCorners.Items.Clear();
            
            //! 코너가 최소 8개 이상 검출되었을 때 다음 작업 수행
            if (corners != null && corners.Length >= 8)
            {
                //! 거리 기준으로 가까운 두 점씩 묶기 위한 리스트와 사용 여부 배열
                List<Tuple<Point2f, Point2f>> edgePairs = new List<Tuple<Point2f, Point2f>>();
                bool[] used = new bool[corners.Length];

                //! 각 점에 대해 가장 가까운 점과 쌍을 만들기
                for (int i = 0; i < corners.Length; i++)
                {
                    if (used[i]) continue;

                    int closestIdx = -1;
                    double minDist = double.MaxValue;

                    //! 다른 점들과 거리 비교
                    for (int j = 0; j < corners.Length; j++)
                    {
                        if (i == j || used[j]) continue;

                        //! 유클리드 거리 직접 계산
                        double dx = corners[i].X - corners[j].X;
                        double dy = corners[i].Y - corners[j].Y;
                        double dist = Math.Sqrt(dx * dx + dy * dy);

                        if (dist < minDist)
                        {
                            minDist = dist;
                            closestIdx = j;
                        }
                    }

                    //! 가장 가까운 점이 있으면 쌍으로 묶고 처리
                    if (closestIdx >= 0)
                    {
                        used[i] = used[closestIdx] = true;
                        edgePairs.Add(Tuple.Create(corners[i], corners[closestIdx]));

                        // 점 1 찍기
                        Cv2.Circle(displayImg,
                            new OpenCvSharp.Point((int)corners[i].X, (int)corners[i].Y),
                            radius: 3,
                            color: Scalar.Red,
                            thickness: -1);

                        // 점 2 찍기
                        Cv2.Circle(displayImg,
                            new OpenCvSharp.Point((int)corners[closestIdx].X, (int)corners[closestIdx].Y),
                            radius: 3,
                            color: Scalar.Red,
                            thickness: -1);

                        // 두 점 사이 선 그리기
                        Cv2.Line(
                            displayImg,
                            new OpenCvSharp.Point((int)corners[i].X, (int)corners[i].Y),
                            new OpenCvSharp.Point((int)corners[closestIdx].X, (int)corners[closestIdx].Y),
                            Scalar.LimeGreen,
                            2
                        );


                        // 거리 정보 출력
                        string lineInfo = $"길이: {minDist:F1} (X1: {corners[i].X:F1}, Y1: {corners[i].Y:F1} - X2: {corners[closestIdx].X:F1}, Y2: {corners[closestIdx].Y:F1})";
                        listBoxCorners.Items.Add(lineInfo);

                    }
                }

                pictureBox1.Image = BitmapConverter.ToBitmap(displayImg); //! 네모 그린 후 이미지 표시
            }
            else
            {
                MessageBox.Show("코너 검출 불가");
            }

        }
    }
}
