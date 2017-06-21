using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class DepthColorView : MonoBehaviour {
    //public Material mat;

    public GameObject MultiSourceManager;


    private KinectSensor _Sensor;
    private CoordinateMapper _Mapper;
    private Color[] _Colors;

    private Texture2D _Texture;


    // Only works at 2 or 4 right now
    private int _DownsampleSize = 2;
    private float _DepthScale = 1f;

    private MultiSourceManager _MultiManager;


    void Start()
    {

        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Mapper = _Sensor.CoordinateMapper;
            var frameDesc = _Sensor.DepthFrameSource.FrameDescription;

            // Downsample to lower resolution
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
            LoadColors(frameDesc.Width, frameDesc.Height);
        }

    }

    void LoadColors(int width, int height)
    {
   
        _Colors = new Color[8000];

        for (int i = 0; i < 8000; i++)
        {

            _Colors[i] = new Color32(255, 0, 255, 255);

        }

        //GetComponent<Renderer>().material = mat;

    }

    public void Update()
    {

        if (_Sensor == null)
        {
            return;
        }


        if (MultiSourceManager == null)
        {
            return;
        }

        _MultiManager = MultiSourceManager.GetComponent<MultiSourceManager>();
        if (_MultiManager == null)
        {
            return;
        }

        if (!_MultiManager.isFresh)
        {
            return;
        }

        //gameObject.GetComponent<Renderer>().material.mainTexture = _MultiManager.GetColorTexture();

        RefreshData(_MultiManager.GetDepthData(),
                    _MultiManager.GetColorData());

        _MultiManager.isFresh = false;
    }


    unsafe void RefreshData(ushort[] depthData, byte[] colorData)
    {
        Color32 emptyColor = new Color32(0, 0, 0, 0);
        var frameDesc = _Sensor.DepthFrameSource.FrameDescription;
        //var imageDesc = _Sensor.ColorFrameSource.FrameDescription;
        var imageDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
        ColorSpacePoint[] colorSpace = new ColorSpacePoint[depthData.Length];
        CameraSpacePoint[] cameraSpace = new CameraSpacePoint[depthData.Length];

        _Mapper.MapDepthFrameToCameraSpace(depthData, cameraSpace);
        _Mapper.MapDepthFrameToColorSpace(depthData, colorSpace);

        int stride = (int)imageDesc.BytesPerPixel * imageDesc.Width;
        long bigIndexRow = 0;
        long frameWidth = (frameDesc.Width / _DownsampleSize);

        Vector3 hidden = new Vector3(9999.0f, 9999.0f, 9999.0f);

        CameraSpacePoint p;
        int pointIndex = 0;
        uint colorsLength = imageDesc.BytesPerPixel * imageDesc.LengthInPixels - 3;
        //DepthSpacePoint d = new DepthSpacePoint();

        for (int y = 0; y < frameDesc.Height; y += 2)
        {
            if (pointIndex >= 8000)
                break;

            long bigIndex = bigIndexRow;
            Color32 c = new Color32();
            c.a = 255;
            for (int x = 0; x < frameDesc.Width; x += 2)
            {

                if (pointIndex >= 8000)
                    break;

                //d.X = x;
                //d.Y = y;

                p = cameraSpace[bigIndex];
                var colorSpacePoint = colorSpace[bigIndex];

                if (colorSpacePoint.X > 0 && colorSpacePoint.Y > 0 && colorSpacePoint.Y < imageDesc.Height)
                {

                    long colorI = ((int)colorSpacePoint.X * (int)imageDesc.BytesPerPixel) + ((int)colorSpacePoint.Y * stride);
                    if (colorI < colorsLength)
                    {
                        c.r = colorData[colorI + 0];
                        c.g = colorData[colorI + 1];
                        c.b = colorData[colorI + 2];

                        _Colors[pointIndex] = c;
                        _Texture.SetPixel(x, y, c);

                    }

                    pointIndex++;

                }
                bigIndex += 2;


            }
            bigIndexRow += frameDesc.Width * 2;

        }




        gameObject.GetComponent<Renderer>().material.mainTexture = _Texture;


        /*
        for (int i = pointIndex; i < MaxPoints; i++)
        {
            _Colors[i] = Color.black;
        }*/

    }

    private bool isEdge(ushort[] depthData, int x, int y, int width, int height)
    {
        if (x < 1 || x > width - 1)
            return false;
        if (y < 1 || y > height - 1)
            return false;

        int offRow = ((y - 1) * width);
        for (int y1 = y - 1; y1 < y + 1; y1 += 1)
        {

            for (int x1 = x - 1; x1 < x + 1; x1 += 1)
            {
                int fullIndex = offRow + x1;

                if (depthData[fullIndex] == 0)
                    return true;


            }
            offRow += width;
        }

        return false;
    }


    private float GetAvg(ushort[] depthData, int x, int y, int width, int height)
    {
        float sum = 0.0f;

        if (x < 1 || x >= width - 1)
            return 0.0f;
        if (y < 1 || y >= height - 1)
            return 0.0f;

        for (int y1 = y - 1; y1 < y + 1; y1++)
        {
            for (int x1 = x - 1; x1 < x + 1; x1++)
            {
                int fullIndex = (y1 * width) + x1;

                if (depthData[fullIndex] == 0)
                    sum += 4500f;
                else
                    sum += depthData[fullIndex];

            }
        }

        return sum / 4;
    }

    void OnApplicationQuit()
    {
        if (_Mapper != null)
        {
            _Mapper = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}
