using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorSourceManager : MonoBehaviour 
{
    public int ColorWidth { get; private set; }
    public int ColorHeight { get; private set; }

    private KinectSensor _Sensor;
    private ColorFrameReader _Reader;
    private Texture2D _Texture;
    private Texture2D _newTexture;
    private byte[] _Data;
    private byte[] _cutData;
    private Color _tempColor;


    public Texture2D GetColorTexture()
    {
        return _newTexture;
    }
    
    void Start()
    {
        _Sensor = KinectSensor.GetDefault ();


        if (_Sensor != null) 
        {
            _Reader = _Sensor.ColorFrameSource.OpenReader();
            
            var frameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorWidth = frameDesc.Width;
            ColorHeight = frameDesc.Height;
            
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
            _newTexture = new Texture2D(1570, 1000, TextureFormat.RGBA32, false);
            _Data = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];
            _cutData = new byte[_Data.Length];
            //Debug.Log("Data length = " + _Data.Length);

            Debug.Log("Width  ==  " + ColorWidth + "Length == " + ColorHeight);
			
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }
    
    unsafe void Update () 
    {
        if (_Reader != null) 
        {
            var frame = _Reader.AcquireLatestFrame();
            
            if (frame != null)
            {

                //Cutting texture to fit depth texture...

                fixed (byte* pData = _Data)
				{
                    
					frame.CopyConvertedFrameDataToIntPtr(new System.IntPtr(pData), (uint)_Data.Length, ColorImageFormat.Rgba);
				}
				frame.Dispose();
				frame = null;


                

                _Texture.LoadRawTextureData(_Data);
                _Texture.Apply();

                /*
                for (int i = 0; i < _Texture.height; i++)
                {
                    for (int j = 0; j < _Texture.width; j++)
                    {
                        if ((j < 190) || (j > (1760)) || (i < 80))
                        {
                            //Turn black
                            _Texture.SetPixel(j, i, Color.cyan);
                        }
                        
                    }

                }*/

                for (int i = 0; i < _newTexture.height; i++)
                {
                    for (int j = 0; j < _newTexture.width; j++)
                    {
                        _tempColor = _Texture.GetPixel(j + 160, i + 20);
                        _newTexture.SetPixel(j, i, _tempColor);

                    }

                }

                //_newTexture.LoadRawTextureData(_Data);
                _newTexture.Apply();
                //_Texture.Apply();
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
