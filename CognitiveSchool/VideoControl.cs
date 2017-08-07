using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.ProjectOxford.Video;
using Microsoft.ProjectOxford.Video.Contract;

namespace CognitiveSchool
{
    class VideoControl
    {
        private string _originalFileString;

        private FileStream _originalFile;
        private FileStream _cognitiveFile;

        private Uri _originalUri;
        private Uri _cognitiveUri;

        private readonly string _videoAPISubscriptionKey = "";

        private static readonly TimeSpan _waitTime = TimeSpan.FromSeconds(20);

        private VideoServiceClient _videoServiceClient;

        public string OriginalFileString
        {
            set { _originalFileString = value; }
            get { return _originalFileString; }
        }

        public FileStream OriginalFile
        {
            private set { _originalFile = value; }
            get { return _originalFile; }
        }

        public FileStream CognitiveFile
        {
            private set { _cognitiveFile = value; }
            get { return _cognitiveFile; }
        }


        public VideoControl(string filename)
        {
            if (filename == null)
            {
                _originalFileString = null;
            }
            _videoServiceClient = new VideoServiceClient(_videoAPISubscriptionKey);
            _videoServiceClient.Timeout = TimeSpan.FromMinutes(10);
        }

        public VideoControl() : this(null)
        {
            
        }

        public void OpenFile(string filename)
        {
            _originalFileString = filename;
            _originalFile = new FileStream(_originalFileString, FileMode.Open, FileAccess.Read);
            _originalUri = new Uri(_originalFile.Name);
        }

        public async Task StabilizeVideo(MediaElement origVideo, MediaElement cognititveVideo)
        {
            using (_originalFile)
            {
                origVideo.Source = _originalUri;
                Operation operation = await _videoServiceClient.CreateOperationAsync(_originalFile,
                    new VideoStabilizationOperationSettings());
                OperationResult result = await _videoServiceClient.GetOperationResultAsync(operation);
                while (result.Status != OperationStatus.Succeeded && result.Status != OperationStatus.Failed)
                {
                    await Task.Delay(_waitTime);
                    result = await _videoServiceClient.GetOperationResultAsync(operation);
                }
                if (result.Status == OperationStatus.Succeeded)
                {
                    string tmpFile = System.IO.Path.GetTempFileName() + System.IO.Path.GetExtension(_originalFileString);
                    using (Stream resultStream = await _videoServiceClient.GetResultVideoAsync(result.ResourceLocation))
                    {
                        _cognitiveFile = new FileStream(tmpFile, FileMode.Create, FileAccess.ReadWrite);
                        _cognitiveUri = new Uri(_cognitiveFile.Name);
                        byte[] bytearray = new byte[2048];
                        int length = 0;
                        while ((length = await resultStream.ReadAsync(bytearray, 0, bytearray.Length)) > 0)
                        {
                            await _cognitiveFile.WriteAsync(bytearray, 0, length);
                        }
                        cognititveVideo.Source = _cognitiveUri;
                    }
                }
            }
        }
    }
}
