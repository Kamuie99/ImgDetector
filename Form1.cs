using System;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using DPoint = System.Drawing.Point; //! Form 용 점 타입
using OPoint = OpenCvSharp.Point;    //! OpenCV용 점 타입

namespace ImgDetector
{
    public partial class Form1 : Form
    {
        private Mat originalImage;   //! 원본 이미지 저장용
        private Mat displayedImage;  //! 화면에 표시할 이미지 (처리된 결과 포함)

        public Form1()
        {
            InitializeComponent();

            //! 트랙바 값이 변경될 때마다 공통 이벤트 처리기 연결
            trackBarThreshold.Scroll += TrackBar_Scroll;
            trackBarBlur.Scroll += TrackBar_Scroll;
            trackBarApprox.Scroll += TrackBar_Scroll;

            //! 트렉바 초기값을 레이블에 반영
            UpdateLabels();
        }
        //! 이미지 불러오기 버튼 클릭 시 실행
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //! 이미지 불러오기 및 초기화
                originalImage = Cv2.ImRead(ofd.FileName);
                displayedImage = originalImage.Clone();

                //! PictureBox에 이미지 표시
                pictureBox1.Image = BitmapConverter.ToBitmap(displayedImage);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; //! 화면 크기에 맞게 표시
            }
        }
        //! '검출' 버튼 클릭 시 실행
        private void btnDetect_Click(object sender, EventArgs e)
        {
            ProcessImage();
        }
        //! 트랙바가 움직일 때 실행되는 공통 이벤트
        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            UpdateLabels();
            ProcessImage();
        }
        //! 트랙바 레이블 텍스트를 현재 값으로 업데이트
        private void UpdateLabels()
        {
            labelThreshold.Text = $"Threshold: {trackBarThreshold.Value}";
            labelBlur.Text = $"Blur Size: {trackBarBlur.Value}";
            labelApprox.Text = $"Approx Epsilon: {trackBarApprox.Value}";
        }
        //! 이미지 처리 로직 (그레이 변환, 블러, 임계값, 윤곽선, 근사 다각형)
        private void ProcessImage()
        {
            if (originalImage == null) return; //! 이미지 없으면 중단
            
            //! 트랙바 값 가져오기
            int thresholdVal = trackBarThreshold.Value;
            int blurSize = trackBarBlur.Value;
            if (blurSize % 2 == 0) blurSize += 1; //! 커널 사이즈는 홀수로 강제
            int approxEpsilon = trackBarApprox.Value;

            //! 1. 이미지 그레이스케일 변환
            Mat gray = new Mat();
            Cv2.CvtColor(originalImage, gray, ColorConversionCodes.BGR2GRAY);

            //! 2. 가우시안 블러 적용 (노이즈 제거)
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(blurSize, blurSize), 0);

            //! 3. 임계값 처리 (이진화)
            Mat thresh = new Mat();
            Cv2.Threshold(gray, thresh, thresholdVal, 255, ThresholdTypes.Binary);

            //! 4. 윤곽선 검출
            OPoint[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            //! 4-1. 윤곽선 없는 경우
            if (contours.Length == 0)
            {
                MessageBox.Show("자재를 찾지 못했습니다.");
                return;
            }

            //! 5. 가장 큰 윤곽선 (자재로 간주) 탐색
            double maxArea = 0;
            int maxIndex = -1;
            for (int i = 0; i < contours.Length; i++)
            {
                double area = Cv2.ContourArea(contours[i]);
                if (area > maxArea)
                {
                    maxArea = area;
                    maxIndex = i;
                }
            }

            if (maxIndex == -1)
            {
                MessageBox.Show("자재 윤곽선을 찾지 못했습니다.");
                return;
            }

            //! 6. 다각형 근사화 (윤곽선을 꼭짓점으로 단순화)
            var approx = Cv2.ApproxPolyDP(contours[maxIndex], approxEpsilon, true);

            //! 7. 원본 이미지 복사하여 결과 이미지로 사용
            displayedImage = originalImage.Clone();

            //! 8. 자재 윤곽선 그리기 (빨간색)
            Cv2.DrawContours(displayedImage, contours, maxIndex, new Scalar(0, 0, 255), 2);

            //! 9. 꼭짓점 위치에 초록색 원 표시
            foreach (var pt in approx)
            {
                Cv2.Circle(displayedImage, pt, 7, new Scalar(0, 255, 0), -1);
            }
            //! 10. 최종 이미지 PictureBox에 표시
            pictureBox1.Image = BitmapConverter.ToBitmap(displayedImage);
        }
    }
}
