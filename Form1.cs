using System;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using DPoint = System.Drawing.Point; //! Form 용 점 타입
using OPoint = OpenCvSharp.Point;    //! OpenCV용 점 타입
using System.Linq;

namespace ImgDetector
{
    public partial class Form1 : Form
    {
        private Mat originalImage;   //! 원본 이미지 저장용
        private Mat displayedImage;  //! 화면에 표시할 이미지 (처리된 결과 포함)

        private float zoomFactor = 1.0f;
        private const float ZOOM_STEP = 0.1f;
        private const float ZOOM_MIN = 0.2f;
        private const float ZOOM_MAX = 5.0f;

        private bool isDragging = false;

        private System.Drawing.Point dragStartPoint;
        private System.Drawing.Point imageStartLocation;


        public Form1()
        {
            InitializeComponent();

            //! 트랙바 값이 변경될 때마다 공통 이벤트 처리기 연결
            trackBarThreshold.Scroll += TrackBar_Scroll;
            trackBarBlur.Scroll += TrackBar_Scroll;
            trackBarApprox.Scroll += TrackBar_Scroll;

            //! 트렉바 초기값을 레이블에 반영
            UpdateLabels();

            // 마우스 휠 이벤트 등록
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;

            // 마우스 포커스를 받아야 휠 이벤트 작동하므로
            pictureBox1.MouseEnter += (s, e) => pictureBox1.Focus();

            //! 마우스 이동시 사진 이동을 위한 이벤트 연결
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseUp += PictureBox1_MouseUp;

        }


        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                dragStartPoint = pictureBox1.PointToScreen(e.Location);
                imageStartLocation = pictureBox1.Location;
                isDragging = true;
                pictureBox1.Cursor = Cursors.Hand;
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var currentScreenPos = pictureBox1.PointToScreen(e.Location);
                int dx = currentScreenPos.X - dragStartPoint.X;
                int dy = currentScreenPos.Y - dragStartPoint.Y;

                pictureBox1.Location = new System.Drawing.Point(
                    imageStartLocation.X + dx,
                    imageStartLocation.Y + dy
                );
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                pictureBox1.Cursor = Cursors.Default;
            }
        }


        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (displayedImage == null) return;

            // 줌 인/아웃
            if (e.Delta > 0 && zoomFactor < ZOOM_MAX)
                zoomFactor += ZOOM_STEP;
            else if (e.Delta < 0 && zoomFactor > ZOOM_MIN)
                zoomFactor -= ZOOM_STEP;

            UpdateZoomedImage();
        }

        private void UpdateZoomedImage()
        {
            if (displayedImage == null) return;

            int newWidth = (int)(displayedImage.Width * zoomFactor);
            int newHeight = (int)(displayedImage.Height * zoomFactor);

            pictureBox1.Image = BitmapConverter.ToBitmap(displayedImage);
            pictureBox1.Size = new System.Drawing.Size(newWidth, newHeight);

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
        //! 감지 여부를 확인해서 on/off 가능하게 기능 구현
        private bool isDetected = false;

        //! '검출' 버튼 클릭 시 실행
        private void btnDetect_Click(object sender, EventArgs e)
        {
            if (!isDetected)
            {
                ProcessImage();
                isDetected = true;
                btnDetect.Text = "초기화";
            }
            else
            {
                //! 감지된 선과 점 제거 -> 원본으로 복원
                if (originalImage != null)
                {
                    displayedImage = originalImage.Clone();
                    pictureBox1.Image = BitmapConverter.ToBitmap(displayedImage);
                    isDetected = false;
                    btnDetect.Text = "모서리 감지";
                }
            }
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

        //! 꼭지점 저장용
        private Point[] detectedCorners = new Point[0];

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

            // 기존 ProcessImage() 중에서 approx 사용 부분 이후
            detectedCorners = approx.Select(p => new Point(p.X, p.Y)).ToArray();

            //! 7. 원본 이미지 복사하여 결과 이미지로 사용
            displayedImage = originalImage.Clone();

            //! 8. 자재 윤곽선 그리기 (빨간색), 두께는 제일 마지막 값
            Cv2.DrawContours(displayedImage, contours, maxIndex, new Scalar(0, 0, 255), 2);

            //! 9. 꼭짓점 위치에 초록색 원 표시
            foreach (var pt in approx)
            {
                //! 점 사이즈 변경은 pt 뒤에 있는 숫자
                Cv2.Circle(displayedImage, pt, 5, new Scalar(0, 255, 255), -1);
            }
            //! 10. 최종 이미지 PictureBox에 표시
            pictureBox1.Image = BitmapConverter.ToBitmap(displayedImage);
        }

        private void btnAnalyzeLongestEdge_Click(object sender, EventArgs e)
        {
            if (detectedCorners == null || detectedCorners.Length != 8)
            {
                MessageBox.Show("꼭짓점이 8개가 아닙니다. 먼저 자재를 정확히 검출해주세요.");
                return;
            }

            double maxDist = 0;
            Point pt1 = new Point();
            Point pt2 = new Point();

            // 인접한 8개 점의 쌍에서 거리 계산 (8개의 선분)
            for (int i = 0; i < detectedCorners.Length; i++)
            {
                Point p1 = detectedCorners[i];
                Point p2 = detectedCorners[(i + 1) % detectedCorners.Length]; // 다음 점 (순환)

                double dist = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                if (dist > maxDist)
                {
                    maxDist = dist;
                    pt1 = p1;
                    pt2 = p2;
                }
            }

            // 중심점 계산
            Point center = GetPolygonCenter(detectedCorners);

            // 가장 긴 변의 중심
            Point mid = new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);

            string position = "";
            if (mid.X < center.X && mid.Y < center.Y)
                position = "왼쪽 상단";
            else if (mid.X > center.X && mid.Y < center.Y)
                position = "오른쪽 상단";
            else if (mid.X < center.X && mid.Y > center.Y)
                position = "왼쪽 하단";
            else
                position = "오른쪽 하단";

            MessageBox.Show($"가장 긴 모서리는 {position}에 위치해 있습니다.\n길이: {maxDist:F2}");
        }

        private Point GetPolygonCenter(Point[] points)
        {
            int sumX = 0, sumY = 0;
            foreach (var pt in points)
            {
                sumX += pt.X;
                sumY += pt.Y;
            }
            return new Point(sumX / points.Length, sumY / points.Length);
        }
    }
}