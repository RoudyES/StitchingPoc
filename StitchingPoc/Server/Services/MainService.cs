using Emgu.CV;
using Emgu.CV.Stitching;
using Emgu.CV.Util;
using Microsoft.AspNetCore.SignalR.Client;
using StitchingPoc.Server.Hubs;

namespace StitchingPoc.Server.Services
{
    public class MainService
    {
        bool stopStream = false;
        HubConnection? hubConnection;
        VideoCapture cap;
        VectorOfMat images;
        public MainService()
        {
            StartSignalR();
        }

        private async void StartSignalR()
        {
            hubConnection = new HubConnectionBuilder()
                            .WithUrl("https://localhost:7028/signalhub")
                            .Build();

            hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await hubConnection.StartAsync();
            };

            hubConnection.On<int>("ReceiveSignal", async (message) =>
            {
                if (message == 0)
                    stopStream = true;

                else
                {
                    stopStream = false;
                    if (cap is not null)
                    {
                        Stitch();
                    }
                }
            });

            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public void SetURL(string url)
        {
            if (cap is not null)
                cap.Dispose();
            cap = new VideoCapture(url);
        }
        public async Task Stitch()
        {
            await Task.Run(async () =>
            {

                images = new VectorOfMat();
                Emgu.CV.Features2D.Brisk orb = new Emgu.CV.Features2D.Brisk();
                WarperCreator warper = new SphericalWarper();
                Stitcher stitcher = new Stitcher();
                stitcher.SetWarper(warper);
                stitcher.SetFeaturesFinder(orb);
                Mat frame = new Mat();
                int count = 1;
                while (!stopStream && cap.Read(frame))
                {
                    if (count % 10 == 0)
                    {
                        images.Push(frame.Clone());
                    }
                    count++;
                }
                Mat result = new Mat();

                var stitchRes = stitcher.Stitch(images, result);

                if (stitchRes != Stitcher.Status.Ok)
                {
                    CvInvoke.HConcat(images, result);
                    CvInvoke.Resize(result, result, new System.Drawing.Size(result.Width / images.Size, result.Height / images.Size));
                }

                byte[]? img = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    result.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    img = ms.ToArray();
                }
                if (img is not null)
                {
                    CvInvoke.Resize(result, result, new System.Drawing.Size(1920, 1080));
                    CvInvoke.Imshow("result", result);
                    CvInvoke.WaitKey(0);
                    await hubConnection?.SendAsync("SendImage", img);
                }

                CvInvoke.DestroyAllWindows();
                for (int i = 0; i < images.Size; i++)
                {
                    images[i].Dispose();
                }
                images.Dispose();
            });
        }
    }
}
