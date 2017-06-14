using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class DepthSourceManager : MonoBehaviour
{


    #region Karen editing

    private ushort _minDepth;
    private ushort _maxDepth;
    private ushort _userDepth;
    private double _percentage = 0.0f;
    private ushort _depthDistance;

    private ushort _tempDepth;

    private ushort[] _newData;
    private byte[] _rawData;


    #endregion


    private KinectSensor _Sensor;
    private DepthFrameReader _Reader;

    private Texture2D _Texture;
    public Texture2D GetDepthTexture()
    {
        return _Texture;
    }

    private ushort[] _Data;

    private int _DepthWidth;
    public int GetDepthWidth()
    {
        return _DepthWidth;
    }

    private int _DepthHeight;
    public int GetDepthHeight()
    {
        return _DepthHeight;
    }

    void Start()
    {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.DepthFrameSource.OpenReader();

            var frameDesc = _Sensor.DepthFrameSource.FrameDescription;

            _DepthWidth = frameDesc.Width;
            _DepthHeight = frameDesc.Height;

            // Use ARGB4444 as there's no handier 16 bit texture format readily available
            _Texture = new Texture2D(_DepthWidth, _DepthHeight, TextureFormat.BGRA32, false);
           // _Data = new byte[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels * _Sensor.DepthFrameSource.FrameDescription.BytesPerPixel];

            _Data = new ushort[frameDesc.LengthInPixels];
            _rawData = new byte[frameDesc.LengthInPixels * 4];
            _minDepth = _Sensor.DepthFrameSource.DepthMinReliableDistance;
            _maxDepth = _Sensor.DepthFrameSource.DepthMaxReliableDistance;
            _depthDistance = (ushort)(_maxDepth - _minDepth);

        }
    }

    unsafe void Update()
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();

            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);

                int index = 0;

                for (int i = 0; i < _Data.Length; i++)
                {

                    if (_Data[i] >= _minDepth && _Data[i] <= _maxDepth)
                    {

                        _tempDepth = (ushort)(_Data[i] - (_minDepth));

                        _percentage = (double)((float)_tempDepth /(float)_depthDistance);


                        byte intensity = (byte)((long)((1 - _percentage) * 255));
                        _rawData[index++] = intensity;
                        _rawData[index++] = intensity;
                        _rawData[index++] = intensity;
                        _rawData[index++] = 255; // Alpha
                    }
                    else
                    {
                        byte intensity = (byte)((0));
                        _rawData[index++] = intensity;
                        _rawData[index++] = intensity;
                        _rawData[index++] = intensity;
                        _rawData[index++] = 255; // Alpha
                    }

                }
                /*
                foreach (var ir in _Data)
                {
                    byte intensity = (byte)();
                    _rawData[index++] = 255;
                    _rawData[index++] = 255;
                    _rawData[index++] = 255;
                    _rawData[index++] = 255; // Alpha
                }
                */
                /*fixed (byte* pData = _rawData)
                {
                    frame.CopyFrameDataToIntPtr(new System.IntPtr(pData), (uint)_Data.Length);
                }*/
                frame.Dispose();
                frame = null;

                _Texture.LoadRawTextureData(_rawData);
                _Texture.Apply();
            }
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
