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




            #region 1. Good FeaturesToTrack 알고리즘 (5/8 인식)
            //Point2f[] corners = Cv2.GoodFeaturesToTrack(
            //    gray,
            //    maxCorners: 8,              //! 최대 8개의 코너를 찾는다 (모깍인 꼭지점 4쌍 유도)
            //    qualityLevel: 0.002,         //! 코너 품질 임계값
            //    minDistance: 20,            //! 코너 간 최소 거리 (보통 거리가 38.61픽셀 정도 나오니까 0~30 사이 설정 가능)
            //    mask: null,                 //! 마스크 없음
            //    blockSize: 3,               //! 계산 윈도우 크기
            //    useHarrisDetector: false,   //! Harris 코너 검출기 사용 안 함
            //    k: 0.04                     //! Harris 계수 (사용 안하지만 기본값 필요)
            //);

            #endregion

            #region 2. Contour + ApproxPolyDP 알고리즘
            //! 이진화
            Mat binary = new Mat();
            Cv2.Threshold(gray, binary, 100, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            // 윤곽선 찾기
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // 꼭짓점 후보를 저장할 리스트
            List<Point2f> cornerList = new List<Point2f>();

            foreach (var contour in contours)
            {
                // 다각형 근사
                OpenCvSharp.Point[] approx = Cv2.ApproxPolyDP(contour, epsilon: 10, closed: true);

                if (approx.Length >= 4 && approx.Length <= 10)
                {
                    foreach (var pt in approx)
                    {
                        cornerList.Add(new Point2f(pt.X, pt.Y));
                    }
                }
            }

            // 꼭짓점 배열 생성
            Point2f[] corners = cornerList.ToArray();
            #endregion






            //! 기존 좌표 표시박스 초기화
            listBoxCorners.Items.Clear();
            

            for (int i = 0; i < corners.Length; i++)
            {
                var point = new OpenCvSharp.Point((int)corners[i].X, (int)corners[i].Y);

                //! 빨간색 점 그리기
                Cv2.Circle(displayImg, point, radius: 3, color: Scalar.Red, thickness: -1);

                //! 인덱스 텍스트 그리기
                Cv2.PutText(
                    displayImg,
                    (i+1).ToString(),                             // 텍스트 내용: 인덱스
                    new CvPoint(point.X + 5, point.Y - 5),      // 위치: 점 옆 (조금 위, 오른쪽)
                    HersheyFonts.HersheySimplex,                // 폰트
                    1,                                          // 폰트 크기
                    Scalar.Yellow,                              // 글자 색
                    2                                           // 두께
                );

                //! 좌표 리스트에도 추가
                listBoxCorners.Items.Add($"{i+1}, {corners[i]}");
            }



            pictureBox1.Image = BitmapConverter.ToBitmap(displayImg); //! 네모 그린 후 이미지 표시

        }
    }
}
