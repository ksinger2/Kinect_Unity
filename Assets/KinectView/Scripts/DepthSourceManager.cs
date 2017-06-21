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
    private Texture2D _newTexture;
    public Texture2D GetDepthTexture()
    {
        return _newTexture;
    }


    private ushort[] _Data;


    public ushort[] GetDepthData()
    {
        return _Data;
    }


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
            Debug.Log("wid " + _DepthWidth + "   height " + _DepthHeight);
            _newTexture = new Texture2D(_DepthWidth, 384, TextureFormat.BGRA32, false);

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

                frame.Dispose();
                frame = null;

                _Texture.LoadRawTextureData(_rawData);

                /*
                for (int i = 0; i < _Texture.height; i++)
                {
                    for (int j = 0; j < _Texture.width; j++)
                    {
                        if (i > 394)
                        {
                            //Turn black
                            _Texture.SetPixel(j, i, Color.cyan);
                        }

                    }

                }*/
                _Texture.Apply();


                for (int i = 0; i < _newTexture.height; i++)
                {
                    for (int j = 0; j < _newTexture.width; j++)
                    {
                        //Turn black
                        _newTexture.SetPixel(j, i, _Texture.GetPixel(j, i + 424));

                    }

                }
                _newTexture.Apply();


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
