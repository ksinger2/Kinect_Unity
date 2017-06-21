using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorDepthView : MonoBehaviour
{
    public int ColorWidth { get; private set; }
    public int ColorHeight { get; private set; }
    public long Together;

    private KinectSensor _Sensor;
    private ColorFrameReader _Reader;
    private Texture2D _Texture;
    private byte[] _Data;
    private ushort[] _DepthData;
    public DepthSourceManager depthManager;

    #region Coordinate Map Testing
    private CoordinateMapper _Mapper;
    private Color[] _Colors;





    #endregion

    public Texture2D GetColorTexture()
    {
        return _Texture;
    }

    void Start()
    {
        _Sensor = KinectSensor.GetDefault();

        _Mapper = _Sensor.CoordinateMapper;


        if (_Sensor != null)
        {
            _Reader = _Sensor.ColorFrameSource.OpenReader();

            var frameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorWidth = frameDesc.Width;
            ColorHeight = frameDesc.Height;

            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
            _Data = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];
            _DepthData = new ushort[frameDesc.LengthInPixels];
            _DepthData = depthManager.GetDepthData();

            Together = _Data.Length;
            
            Debug.Log(Together);
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
            
            _Colors = new Color[_Texture.width * _Texture.height];

            for (int i = 0; i < _Colors.Length; i++)
            {
                _Colors[i] = new Color32(255, 0, 255, 255);

            }
        }
    }

    unsafe void Update()
    {
        if (_Reader != null)
        {
            _DepthData = depthManager.GetDepthData();

            var frame = _Reader.AcquireLatestFrame();

            if (frame != null)
            {
                fixed (byte* pData = _Data)
                {

                    frame.CopyConvertedFrameDataToIntPtr(new System.IntPtr(pData), (uint)_Data.Length, ColorImageFormat.Rgba);
                }
                frame.Dispose();
                frame = null;
                /*int i = 100;
                if (i < 108)
                {
                    i++;
                    Debug.Log("jere " + _Data.Length + "the together leng " + Together);
                }


            */
                //Testing area/////////////////////////////////////////////////////////////////////

                var frameDesc = _Sensor.DepthFrameSource.FrameDescription;
                _Texture.LoadRawTextureData(_Data);
                //_Texture.SetPixels(_Colors);
                _Texture.Apply();


                
                //var imageDesc = _Sensor.ColorFrameSource.FrameDescription;
                var imageDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
                ColorSpacePoint[] colorSpace = new ColorSpacePoint[_DepthData.Length];
                CameraSpacePoint[] cameraSpace = new CameraSpacePoint[_DepthData.Length];

                _Mapper.MapDepthFrameToCameraSpace(_DepthData, cameraSpace);
                _Mapper.MapDepthFrameToColorSpace(_DepthData, colorSpace);

                int stride = (int)imageDesc.BytesPerPixel * imageDesc.Width;
                CameraSpacePoint p;
                int pointIndex = 0;
                uint colorsLength = imageDesc.BytesPerPixel * imageDesc.LengthInPixels - 3;
                //DepthSpacePoint d = new DepthSpacePoint();
                long bigIndexRow = 0;
                
                long index = 0;

                for (int y = 0; y < frameDesc.Height; y ++)
                {
                    if (pointIndex >= Together)
                        break;

                    long bigIndex = bigIndexRow;
                    Color32 c = new Color32();
                    c.a = 255;
                    for (int x = 0; x < frameDesc.Width; x ++)
                    {
                        if (pointIndex >= Together)
                            break;
                        //d.X = x;
                        //d.Y = y;

                        p = cameraSpace[bigIndex];


                        var colorSpacePoint = colorSpace[bigIndex];

                        if (colorSpacePoint.X > 0 && colorSpacePoint.Y > 0 && colorSpacePoint.Y < imageDesc.Height)
                        {
                            long colorI = ((int)colorSpacePoint.X * (int)imageDesc.BytesPerPixel) + ((int)colorSpacePoint.Y * stride);
                            if ( index < _Colors.Length)
                            {
                                c.r = _Data[index + 0];
                                c.g = _Data[index + 1];
                                c.b = _Data[index + 2];
                                index++;
                                _Colors[pointIndex] = c;

                            /*
                            if (pointIndex > 100 && pointIndex < 120)
                            {
                                Debug.Log("color = " + _Colors[pointIndex]);
                            }
                            */
                            }
                            index++;

                            pointIndex++;

                        }

                    bigIndex += 1;

                    }
                    bigIndexRow += frameDesc.Width;
                }

                Debug.Log(_Colors.Length);
                /////////////////////////////////////////////////////////////////////
                //_Texture.LoadRawTextureData(_Data);
                _Texture.SetPixels(_Colors);
                _Texture.Apply();
            }
            

            //}
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
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
